using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Orders;
using PushSharp.Apple;
using PushSharp.Core;


namespace Nop.Services.Messages
{
    // Added by Alexandar Rajavel on 25-June-2019
    public class NotificationService : INotificationService
    {
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly ICustomerService _customerService;
        private ApnsServiceBroker _apnsBroker;
        private readonly IRepository<QueuedNotification> _queuedRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly NotificationSettings _notificationSettings;
        private IList<AppleNotificationResponse> appleResponses;
        private readonly OrderSettings _orderSettings;
        private readonly IOrderService _orderService;

        private const string SUCCESS = "Success";
        private const string INTERNAL_ERROR = "Internal server error";
        private const string SIM_CARD_IS_NOT_IN_DEVICE = "Sim card is not in current device";

        public NotificationService(ILocalizationService localizationService,
                                   IStoreContext storeContext,
                                   ISettingService settingService,
                                   ICustomerService customerService,
                                   IRepository<QueuedNotification> queuedRepository,
                                   IEventPublisher eventPublisher,
                                   NotificationSettings notificationSettings,
                                   OrderSettings orderSettings,
                                   IOrderService orderService)
        {
            _localizationService = localizationService;
            _storeContext = storeContext;
            _settingService = settingService;
            _customerService = customerService;
            _queuedRepository = queuedRepository;
            _eventPublisher = eventPublisher;
            _notificationSettings = notificationSettings;
            _orderSettings = orderSettings;
            _orderService = orderService;
        }


        public void SendNotication(QueuedNotification notification)
        {
            //if (notification.DeviceType == DeviceType.IPhone)
            //{
            //    #region ios
            //    try
            //    {
            //        ApnsIntialiaztion();
            //        var customObjectModel = new AppleApsCustomObject
            //        {
            //            NotificationTypeId = notification.NotificationTypeId,
            //            NotificationType = GetNotificationType(notification.NotificationTypeId),
            //            ItemId = notification.ItemId,

            //        };
            //        var aps = new AppleAps()
            //        {
            //            Alert = notification.Message,
            //            Badge = 0,
            //            Sound = "sound.caf",
            //            CustomObject = customObjectModel,
            //            Subject = notification.Subject
            //        };
            //        var apsNotification = new AppleNotification()
            //        {
            //            Aps = aps
            //        };
            //        var payload = JsonConvert.SerializeObject(apsNotification);

            //        _apnsBroker.QueueNotification(new ApnsNotification()
            //        {
            //            DeviceToken = notification.SubscriptionId,
            //            Payload = JObject.Parse(payload),
            //            Tag = notification.Id
            //        });
            //    }
            //    catch (Exception ex)
            //    {
            //        notification.ErrorLog = ex.Message;
            //    }
            //    finally
            //    {
            //        notification.SentTries += 1;
            //        UpdateQueuedNotification(notification);
            //    }
            //    #endregion
            //}
            //else 
            if (notification.DeviceType == DeviceType.Android || notification.DeviceType == DeviceType.IPhone)
            {
                #region android
                var storeScope = _storeContext.ActiveStoreScopeConfiguration;
                var settings = _settingService.LoadSetting<NotificationSettings>(storeScope);
                var fcmModel = new FcmModel
                {
                    To = notification.SubscriptionId,
                    Data =
                    {
                        NotificationTypeId = notification.NotificationTypeId,
                        ItemId = notification.ItemId
                    }
                };
                fcmModel.Notification.Title = fcmModel.Data.Title = notification.Subject;
                fcmModel.Notification.Body = fcmModel.Data.Body = notification.Message;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://fcm.googleapis.com/fcm/send");
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Key", "=" + settings.GoogleConsoleAPIAccess_KEY);
                    var consoleAPIAccessKey = notification.DeviceType == DeviceType.Android ? _notificationSettings.GoogleConsoleAPIAccess_KEY : _notificationSettings.AppleCertFileNameWithPath;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Key", "=" + consoleAPIAccessKey);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<FcmModel>("send", fcmModel).Result;
                }
                #endregion
            }
            else
            {
                #region others

                throw new Exception("Unknown Device");

                #endregion
            }
        }

        #region Send OTP 
        public VerifyMobileNumberResponse SendOTP(VerifyMobileNumberRequest model)
        {
            if (model == null || string.IsNullOrEmpty(model.DestinationNumber) || string.IsNullOrEmpty(model.Application))
                return null;
            var otp = GetOTP();
            //added by Sunil Kumar at 02-05-19
            if (model.BuildType.ToLower() == "debug" && model.DeviceTypeId == (int)DeviceType.Android)
            {
                otp = otp + " " + _localizationService.GetResource("VerifyMobileNumber.BuildType.Debug");
            }
            else if (model.BuildType.ToLower() == "release" && model.DeviceTypeId == (int)DeviceType.Android)
            {
                otp = otp + " " + _localizationService.GetResource("VerifyMobileNumber.BuildType.Release");
            }
            StringBuilder mobileNumberFormated = new StringBuilder(model.DestinationNumber);
            mobileNumberFormated = mobileNumberFormated.Remove(0, 4).Insert(0, "0");
            var smsRequest = new SMSRequest();
            smsRequest.Application = model.Application;
            smsRequest.DestinationNumber = mobileNumberFormated.ToString();
            smsRequest.Message = model.DeviceTypeId == (int)DeviceType.IPhone ? string.Format(_localizationService.GetResource("SMS.OtpFormatiOS"), otp) : string.Format(_localizationService.GetResource("SMS.OtpFormat"), otp);

            var result = new VerifyMobileNumberResponse() { DestinationNumber = model.DestinationNumber, AppVersionName = model.AppVersionName };
            var customer = _customerService.GetCustomerByUsername(model.DestinationNumber);

            //if (model.DeviceTypeId == (int)DeviceType.IPhone)
            //{
            //    var isNotValidNumber = true;
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_localizationService.GetResource("SimBankFetchSMSURL"));
            //        //HTTP GET
            //        var clientResulst = client.GetAsync("").Result;
            //        if (clientResulst.IsSuccessStatusCode)
            //        {
            //            var smsDetails = clientResulst.Content.ReadAsAsync<ReadSMS>().Result;
            //            if (smsDetails.data.Any())
            //            {
            //                for (int i = 0; i < smsDetails.data.Length; i++)
            //                {
            //                    var senderNumber = smsDetails.data[i][3].ToString();
            //                    if (model.DestinationNumber.Remove(0, 2) == senderNumber)
            //                    {
            //                        isNotValidNumber = false;
            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    if (isNotValidNumber)
            //    {
            //        result.Message = SIM_CARD_IS_NOT_IN_DEVICE;
            //        result.StatusCode = 400;
            //        return result;
            //    }
            //}

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_localizationService.GetResource("SMSURL"));

                //HTTP POST
                var postTask = client.PostAsync("", new StringContent(JsonConvert.SerializeObject(smsRequest), Encoding.UTF8, "application/json")).Result;
                if (postTask.IsSuccessStatusCode)
                {
                    //var smsResponse = postTask.Content.ReadAsAsync<SMSResponse>().Result;
                    //added by Sunil Kumar at 02-05-19
                    if (model.BuildType.ToLower() == "debug")
                    {
                        result.HashKey = _localizationService.GetResource("VerifyMobileNumber.BuildType.Debug");
                    }
                    else if (model.BuildType.ToLower() == "release")
                    {
                        result.HashKey = _localizationService.GetResource("VerifyMobileNumber.BuildType.Release");
                    }
                    result.Message = SUCCESS;
                    result.OTP = otp;
                    result.IsUserRegistered = customer == null ? false : true;
                    result.StatusCode = 200;
                }
                else
                {
                    result.Message = INTERNAL_ERROR;
                    result.StatusCode = 500;
                }
            }
            return result;
        }
        #endregion

      



        #region Send SMS
        // Added by Alexandar Rajavel on 15-Feb-2019
        public bool SendSMS(SMSRequest model)
        {
            if (model == null || string.IsNullOrEmpty(model.DestinationNumber) || string.IsNullOrEmpty(model.Application))
                return false;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_localizationService.GetResource("SMSURL"));
                //HTTP POST
                var postTask = client.PostAsync("", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json")).Result;
                if (postTask.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        #endregion

        private string GetOTP()
        {
            Random r = new Random();
            int randNum = r.Next(1000000);
            return randNum.ToString("D6");
        }

        private void ApnsIntialiaztion()
        {
            var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();

            var appleCert = File.ReadAllBytes(fileProvider.MapPath("~/Plugins/NopStation.MobileApp/App_Data/" + _notificationSettings.AppleCertFileNameWithPath));
            ApnsConfiguration.ApnsServerEnvironment environment;
            if (_notificationSettings.IsAppleProductionMode)
            {
                environment = ApnsConfiguration.ApnsServerEnvironment.Production;
            }
            else
            {
                environment = ApnsConfiguration.ApnsServerEnvironment.Sandbox;
            }
            var config = new ApnsConfiguration(environment,
                appleCert, _notificationSettings.ApplePassword);

            // Create a new broker
            _apnsBroker = new ApnsServiceBroker(config);
            _apnsBroker.OnNotificationSucceeded += NotificationSent;
            _apnsBroker.OnNotificationFailed += NotificationFailed;
            _apnsBroker.Start();
        }

        private string GetNotificationType(int notificationId)
        {
            try
            {
                var notificationType = Enum.GetName(typeof(NotificationType), notificationId);
                return notificationType;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public void UpdateQueuedNotification(QueuedNotification notification)
        {
            if (notification == null)
                throw new ArgumentNullException("notification");
            _queuedRepository.Update(notification);
            _eventPublisher.EntityUpdated(notification);
        }

        //this even raised when a notification is successfully sent
        void NotificationSent(INotification notification)
        {
            int id = 0;
            int.TryParse(notification.Tag.ToString(), out id);
            if (id != 0)
            {
                var response = new AppleNotificationResponse();
                response.Id = id;
                response.Success = true;
                response.SentOnUtc = DateTime.UtcNow;
                appleResponses.Add(response);
            }
        }

        //this is raised when a notification is failed due to some reason
        void NotificationFailed(INotification notification, Exception notificationFailureException)
        {

            int id = 0;
            int.TryParse(notification.Tag.ToString(), out id);
            if (id != 0)
            {
                var response = new AppleNotificationResponse();
                response.Success = false;
                response.Error = notificationFailureException.InnerException != null && notificationFailureException.InnerException.Message != null ? notificationFailureException.InnerException.Message : "";
                appleResponses.Add(response);
            }
            //_logger.Error(String.Format("NotificationFailed"), notificationFailureException);
        }
        /// <summary>
        /// Send Otp for Huwaii Device
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public VerifyMobileNumberResponse SendOTPHuwai(VerifyMobileNumberRequest model)
        {
            if (model == null || string.IsNullOrEmpty(model.DestinationNumber) || string.IsNullOrEmpty(model.Application))
                return null;
            var otp = GetOTP();
            //added by Sunil Kumar at 02-05-19
            if (model.BuildType.ToLower() == "debug" && model.DeviceTypeId == (int)DeviceType.Android)
            {
                otp = otp + " " + _localizationService.GetResource("VerifyMobileNumber.BuildType.Huawei.Debug");
            }
            else if (model.BuildType.ToLower() == "release" && model.DeviceTypeId == (int)DeviceType.Android)
            {
                otp = otp + " " + _localizationService.GetResource("VerifyMobileNumber.BuildType.Huawei.Release");
            }
            StringBuilder mobileNumberFormated = new StringBuilder(model.DestinationNumber);
            mobileNumberFormated = mobileNumberFormated.Remove(0, 4).Insert(0, "0");
            var smsRequest = new SMSRequest();
            smsRequest.Application = model.Application;
            smsRequest.DestinationNumber = mobileNumberFormated.ToString();
            smsRequest.Message = model.DeviceTypeId == (int)DeviceType.IPhone ? string.Format(_localizationService.GetResource("SMS.OtpFormatiOS"), otp) : string.Format(_localizationService.GetResource("SMS.OtpFormat"), otp);

            var result = new VerifyMobileNumberResponse() { DestinationNumber = model.DestinationNumber, AppVersionName = model.AppVersionName };
            var customer = _customerService.GetCustomerByUsername(model.DestinationNumber);

            //if (model.DeviceTypeId == (int)DeviceType.IPhone)
            //{
            //    var isNotValidNumber = true;
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_localizationService.GetResource("SimBankFetchSMSURL"));
            //        //HTTP GET
            //        var clientResulst = client.GetAsync("").Result;
            //        if (clientResulst.IsSuccessStatusCode)
            //        {
            //            var smsDetails = clientResulst.Content.ReadAsAsync<ReadSMS>().Result;
            //            if (smsDetails.data.Any())
            //            {
            //                for (int i = 0; i < smsDetails.data.Length; i++)
            //                {
            //                    var senderNumber = smsDetails.data[i][3].ToString();
            //                    if (model.DestinationNumber.Remove(0, 2) == senderNumber)
            //                    {
            //                        isNotValidNumber = false;
            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    if (isNotValidNumber)
            //    {
            //        result.Message = SIM_CARD_IS_NOT_IN_DEVICE;
            //        result.StatusCode = 400;
            //        return result;
            //    }
            //}

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_localizationService.GetResource("SMSURL"));

                //HTTP POST
                var postTask = client.PostAsync("", new StringContent(JsonConvert.SerializeObject(smsRequest), Encoding.UTF8, "application/json")).Result;
                if (postTask.IsSuccessStatusCode)
                {
                    //var smsResponse = postTask.Content.ReadAsAsync<SMSResponse>().Result;
                    //added by Sunil Kumar at 02-05-19
                    if (model.BuildType.ToLower() == "debug")
                    {
                        result.HashKey = _localizationService.GetResource("VerifyMobileNumber.BuildType.Huawei.Debug");
                    }
                    else if (model.BuildType.ToLower() == "release")
                    {
                        result.HashKey = _localizationService.GetResource("VerifyMobileNumber.BuildType.Huawei.Release");
                    }
                    result.Message = SUCCESS;
                    result.OTP = otp;
                    result.IsUserRegistered = customer == null ? false : true;
                    result.StatusCode = 200;
                }
                else
                {
                    result.Message = INTERNAL_ERROR;
                    result.StatusCode = 500;
                }
            }
            return result;
        }



        //Commented by Sunil Kumar at 26-03-2020 no need for Notification to WHM
        //public void SendNoticationToWHM(Order order)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        try
        //        {
        //            client.BaseAddress = new Uri(_orderSettings.ApiUrlforWHM);
        //            client.DefaultRequestHeaders.Add("AdminToken", _orderSettings.AdminTokenforWHM);

        //            //HTTP POST
        //            var postTask = client.PostAsJsonAsync("", new { Value = order.Id }).Result;
        //            if (postTask.IsSuccessStatusCode)
        //            {
        //                order.OrderNotes.Add(new OrderNote
        //                {
        //                    Note = "Order Id has been sent to WHM",
        //                    DisplayToCustomer = false,
        //                    CreatedOnUtc = DateTime.UtcNow
        //                });
        //                _orderService.UpdateOrder(order);
        //            }
        //        }
        //        catch (Exception)
        //        {

        //        }
        //    }
        //}
    }
}

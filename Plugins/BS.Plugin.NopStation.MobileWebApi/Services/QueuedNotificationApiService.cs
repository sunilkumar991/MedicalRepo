using System;
using Nop.Core;
using Newtonsoft.Json;
using Nop.Services.Customers;
using Nop.Services.Messages;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using Nop.Services.Catalog;
using Nop.Services.Media;
using BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Models;
using Nop.Services.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Customer;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Customer;
using System.Text;
using Nop.Services.Localization;
using BS.Plugin.NopStation.MobileWebApi.Models.DeliveryBoy;
using System.Linq;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    // Created by Alexandar Rajavel
    public class QueuedNotificationApiService : IQueuedNotificationApiService
    {

        private readonly IWebHelper _webHelper;
        private readonly IDeviceService _deviceService;
        private static QueuedNotification _cacheQueuedNotification;
        private readonly Nop.Services.Logging.ILogger _logger;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly ITokenizer _tokenizer;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly ICustomerService _customerService;
        private readonly IStoreContext _storeContext;
        private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private const string SUCCESS = "Success";
        private const string INTERNAL_ERROR = "Internal server error";
        private const string SIM_CARD_IS_NOT_IN_DEVICE = "Sim card is not in current device";

        #region Constructor
        public QueuedNotificationApiService(
                       IWebHelper webHelper,
            IDeviceService deviceService,
            Nop.Services.Logging.ILogger logger, IMessageTokenProvider messageTokenProvider,
            ITokenizer tokenizer, IQueuedEmailService queuedEmailService,
            ICustomerService customerService, IStoreContext storeContext,
            IProductService productService,
            IPictureService pictureService,
            ISettingService settingService,
            ILocalizationService localizationService)
        {

            this._webHelper = webHelper;
            this._deviceService = deviceService;

            this._logger = logger;
            _messageTokenProvider = messageTokenProvider;
            _tokenizer = tokenizer;
            _queuedEmailService = queuedEmailService;
            _customerService = customerService;
            _storeContext = storeContext;
            this._productService = productService;
            this._pictureService = pictureService;
            _settingService = settingService;
            _localizationService = localizationService;
        }
        #endregion
        #region push notification
        public void SendNotication(NotificationRequest notification)
        {

            if (notification.DeviceType == DeviceType.IPhone)
            {
                #region ios
                //try
                //{
                //    ApnsIntialiaztion();
                //    var customObjectModel = new AppleApsCustomObject
                //    {
                //        NotificationTypeId = notification.NotificationTypeId,
                //        NotificationType = GetNotificationType(notification.NotificationTypeId),
                //        ItemId = notification.ItemId,

                //    };
                //    var aps = new AppleAps()
                //    {
                //        Alert = notification.Message,
                //        Badge = 0,
                //        Sound = "sound.caf",
                //        CustomObject = customObjectModel,
                //        Subject = notification.Subject
                //    };
                //    var apsNotification = new AppleNotification()
                //    {
                //        Aps = aps
                //    };
                //    var payload = JsonConvert.SerializeObject(apsNotification);

                //    _apnsBroker.QueueNotification(new ApnsNotification()
                //    {
                //        DeviceToken = notification.SubscriptionId,
                //        Payload = JObject.Parse(payload),
                //        Tag = notification.Id
                //    });


                //}
                //catch (Exception ex)
                //{

                //    notification.ErrorLog = ex.Message;
                //}
                //finally
                //{
                //    notification.SentTries += 1;
                //    UpdateQueuedNotification(notification);
                //}
                #endregion

            }
            else if (notification.DeviceType == DeviceType.Android)
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

                //var data = JsonConvert.SerializeObject(fcmModel);
                //var response = FcmExtension.RetriveData<FcmErrorObject>("https://fcm.googleapis.com/fcm/send", settings.GoogleConsoleAPIAccess_KEY, data, 1);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://fcm.googleapis.com/fcm/send");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Key", "=" + settings.GoogleConsoleAPIAccess_KEY);
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
        #endregion

        #region Send OTP 
        public VerifyMobileNumberResponse SendOTP(VerifyMobileNumberRequest model)
        {
            if (model == null || string.IsNullOrEmpty(model.DestinationNumber) || string.IsNullOrEmpty(model.Application))
                return null;
            var otp = GetOTP();
            //added by Rajesh at 02-05-19
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

            if (model.DeviceTypeId == (int)DeviceType.IPhone)
            {
                var isNotValidNumber = true;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_localizationService.GetResource("SimBankFetchSMSURL"));
                    //HTTP GET
                    var clientResulst = client.GetAsync("").Result;
                    if (clientResulst.IsSuccessStatusCode)
                    {
                        var smsDetails = clientResulst.Content.ReadAsAsync<ReadSMS>().Result;
                        if (smsDetails.data.Any())
                        {
                            for (int i = 0; i < smsDetails.data.Length; i++)
                            {
                                var senderNumber = smsDetails.data[i][3].ToString();
                                if (model.DestinationNumber.Remove(0, 2) == senderNumber)
                                {
                                    isNotValidNumber = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (isNotValidNumber)
                {
                    result.Message = SIM_CARD_IS_NOT_IN_DEVICE;
                    result.StatusCode = 400;
                    return result;
                }
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_localizationService.GetResource("SMSURL"));

                //HTTP POST
                var postTask = client.PostAsync("", new StringContent(JsonConvert.SerializeObject(smsRequest), Encoding.UTF8, "application/json")).Result;
                if (postTask.IsSuccessStatusCode)
                {
                    //var smsResponse = postTask.Content.ReadAsAsync<SMSResponse>().Result;
                    //added by Rajesh at 02-05-19
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
            //var data = JsonConvert.SerializeObject(smsRequest);
            //var response = FcmExtension.RetriveData<SMSRequest>("http://120.50.43.157:8092/api/sms/SendSMSVMart", "", data, 1);
            //if (response != null && response.Status.Contains("Success"))
            //{
            //    result.Message = SUCCESS;
            //    result.OTP = otp;
            //    result.IsUserRegistered = customer == null ? false : true;
            //    result.StatusCode = 200;
            //}
            //else
            //{
            //    result.Message = INTERNAL_ERROR;
            //    result.StatusCode = 500;
            //}
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
    }
}

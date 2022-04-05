using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileApp.Domain;
using Nop.Core.Data;
using BS.Plugin.NopStation.MobileApp.Models;
using Nop.Services.Events;
using Nop.Core;
using Nop.Services.Logging;
using PushSharp;

using System.IO;
using Nop.Core.Infrastructure;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Nop.Services.Customers;
using Nop.Services.Messages;
using BS.Plugin.NopStation.MobileWebApi.Services;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using PushSharp.Apple;
using PushSharp.Core;
using Newtonsoft.Json.Linq;
using BS.Plugin.NopStation.MobileApp.Extensions;
using Nop.Services.Catalog;
using Nop.Services.Media;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BS.Plugin.NopStation.MobileApp.Services
{
    public class QueuedNotificationService : IQueuedNotificationService
    {

        private readonly IRepository<QueuedNotification> _queuedRepository;

        private readonly IEventPublisher _eventPublisher;
        private readonly IWebHelper _webHelper;
        private readonly NotificationSettings _notificationSettings;
        private readonly IDeviceService _deviceService;
        private readonly IScheduledNotificationService _scheduledNotificationService;
        private readonly ISmartGroupService _smartGroupService;

        //private static QueuedNotification _cacheQueuedNotification;
        private readonly Nop.Services.Logging.ILogger _logger;

        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly ITokenizer _tokenizer;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly ICustomerService _customerService;
        private readonly IStoreContext _storeContext;
        private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
        private readonly INotificationMessageTemplateService _notificationMessageTemplateService;
        private ApnsServiceBroker _apnsBroker;
        private IList<AppleNotificationResponse> appleResponses;

        #region static field

        //private static read only ILoggerNotification _LoggerNotification =
        //    EngineContext.Current.Resolve<ILoggerNotification>();
        #endregion


        public QueuedNotificationService(
            IRepository<QueuedNotification> queuedRepository,
            IEventPublisher eventPublisher,
            IWebHelper webHelper,
            NotificationSettings notificationSettings,
            IDeviceService deviceService,
            IScheduledNotificationService scheduledNotificationService,
            ISmartGroupService smartGroupService,
            Nop.Services.Logging.ILogger logger, IMessageTokenProvider messageTokenProvider,
            ITokenizer tokenizer, IQueuedEmailService queuedEmailService,
            ICustomerService customerService, IStoreContext storeContext,
            IProductService productService,
            IPictureService pictureService,
            INotificationMessageTemplateService notificationMessageTemplateService)
        {
            this._queuedRepository = queuedRepository;
            this._eventPublisher = eventPublisher;
            this._webHelper = webHelper;
            this._notificationSettings = notificationSettings;
            this._deviceService = deviceService;
            this._scheduledNotificationService = scheduledNotificationService;
            this._smartGroupService = smartGroupService;
            this._logger = logger;
            _messageTokenProvider = messageTokenProvider;
            _tokenizer = tokenizer;
            _queuedEmailService = queuedEmailService;
            _customerService = customerService;
            _storeContext = storeContext;
            this._productService = productService;
            this._pictureService = pictureService;
            _notificationMessageTemplateService = notificationMessageTemplateService;
        }

        #region Utilities

        #region push notification events

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

        #endregion

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
        #endregion

        #region methods

        #region CROUD
        public IPagedList<QueuedNotification> GetAllQueuedNotification(int pageIndex, int pageSize)
        {
            var query = (from u in _queuedRepository.Table
                         orderby u.SentOnUtc descending
                         select u);
            var notifications = new PagedList<QueuedNotification>(query, pageIndex, pageSize);
            return notifications;
        }


        public List<QueuedNotification> GetAllQueuedNotification()
        {
            var query = (from u in _queuedRepository.Table
                         orderby u.SentOnUtc descending
                         select u);
            var notifications = query.ToList();
            return notifications;
        }

        public QueuedNotification InsertQueuedNotification(QueuedNotification notification)
        {
            if (notification == null)
                throw new ArgumentNullException("notification");

            _queuedRepository.Insert(notification);
            //event notification
            _eventPublisher.EntityInserted(notification);

            return notification;
        }

        public QueuedNotification GetQueuedNotificationById(int id)
        {
            return _queuedRepository.GetById(id);
        }

        public void UpdateQueuedNotification(QueuedNotification notification)
        {
            if (notification == null)
                throw new ArgumentNullException("notification");
            _queuedRepository.Update(notification);
            _eventPublisher.EntityUpdated(notification);
        }

        public void DeleteQueuedNotification(QueuedNotification notification)
        {
            _queuedRepository.Delete(notification);
            _eventPublisher.EntityDeleted(notification);
        }

        #endregion

        #region  insert queued notification according to schedule

        public void InsertQueuedNoticationsAccordingToScheduleByTime(DateTime time)
        {
            try
            {
                #region code
                var schedules = _scheduledNotificationService.GetAllScheduleByTime(time).ToList();

                foreach (var scheduledNotification in schedules)
                {
                    var queuedCount = 0;
                    var customers = _smartGroupService.GetAllContactsAtOnce(scheduledNotification.GroupId).ToList();
                    foreach (var customer in customers)
                    {
                        var devices = _deviceService.GetDevicesByCustomerId(customer.CustomerId).ToList();
                        foreach (var device in devices)
                        {
                            var entity = new QueuedNotification();
                            entity.ToCustomerId = customer.CustomerId;

                            #region device property
                            entity.DeviceType = (Domain.DeviceType)(int)device.DeviceType;
                            entity.DeviceTypeId = device.DeviceTypeId;
                            entity.SubscriptionId = device.SubscriptionId;

                            #endregion

                            #region schedule property
                            entity.GroupId = scheduledNotification.GroupId;
                            var tokens = new List<Token>();

                            var notifiedCustomer = _customerService.GetCustomerById(customer.CustomerId);
                            if (notifiedCustomer != null)
                            {
                                _messageTokenProvider.AddCustomerTokens(tokens, notifiedCustomer);

                            }
                            var messageTemplate =
                                _notificationMessageTemplateService.GetNotificationMessageTemplateById(
                                    scheduledNotification.NotificationMessageTemplateId);
                            if (messageTemplate != null)
                            {
                                string subject = _tokenizer.Replace(messageTemplate.Subject, tokens, false);
                                string body = _tokenizer.Replace(messageTemplate.Body, tokens, true);
                                entity.Message = body;
                                entity.Subject = subject;

                            }
                            else
                            {
                                entity.Message = scheduledNotification.Message;
                            }

                            #endregion

                            entity.CreatedOnUtc = DateTime.UtcNow;

                            //insert
                            entity.ItemId = scheduledNotification.ItemId;
                            InsertQueuedNotification(entity);
                            queuedCount++; //count 
                        }
                    }
                    #region update schedule

                    scheduledNotification.IsQueued = true;
                    scheduledNotification.QueuedCount = queuedCount;
                    _scheduledNotificationService.UpdateSchedule(scheduledNotification);
                    #endregion

                }
                #endregion
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        #endregion

        #region push notification


        public void SendAllUnSendQueuedNotication(int maxTries = 5)
        {
            var query = _queuedRepository.Table;
            query = query.Where(x => !x.IsSent);
            query = query.Where(x => x.SentTries <= maxTries);
            query = query.Take(1000);
            //var notificationList = query.ToList();

            int count = 0;
            //var androidNotificationList = query.Where(x => x.DeviceType == DeviceType.Android).ToList();
            //var iosNotificationList = query.Where(x => x.DeviceType == DeviceType.iPhone).ToList();
            var androidNotificationList = query.Where(x => x.DeviceTypeId == 10).ToList();
            var iosNotificationList = query.Where(x => x.DeviceTypeId == 5).ToList();
            try
            {
                foreach (var queuedNotification in androidNotificationList)
                {
                    SendQueuedNotication(queuedNotification);
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) _logger.Error(ex.InnerException.Message);
            }
            ApnsIntialiaztion();
            foreach (var queuedNotification in iosNotificationList)
            {
                if (count <= 100)
                {
                    SendQueuedNotication(queuedNotification);
                    count++;
                }
            }

            try
            {
                _apnsBroker.Stop();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) _logger.Error(ex.InnerException.Message);
            }
            finally
            {
                //int i = 0;
                foreach (var appleResponse in appleResponses)
                {
                    var notification = GetQueuedNotificationById(appleResponse.Id);
                    if (notification == null)
                        continue;
                    if (appleResponse.Success)
                    {

                        notification.IsSent = true;
                        notification.SentOnUtc = appleResponse.SentOnUtc;
                        UpdateQueuedNotification(notification);
                    }
                    else
                    {
                        notification.IsSent = false;
                        notification.ErrorLog = appleResponse.Error;
                        UpdateQueuedNotification(notification);
                    }

                }
            }
        }

        public void SendQueuedNotication(QueuedNotification notification)
        {

            if (notification.DeviceType == Domain.DeviceType.iPhone)
            {
                #region ios
                try
                {

                    var customObjectModel = new AppleApsCustomObject
                    {
                        NotificationTypeId = notification.NotificationTypeId,
                        NotificationType = GetNotificationType(notification.NotificationTypeId),
                        ItemId = notification.ItemId,

                    };
                    var aps = new AppleAps()
                    {
                        Alert = notification.Message,
                        Badge = 0,
                        Sound = "sound.caf",
                        CustomObject = customObjectModel,
                        Subject = notification.Subject
                    };
                    var apsNotification = new AppleNotification()
                    {
                        Aps = aps
                    };
                    var payload = JsonConvert.SerializeObject(apsNotification);

                    _apnsBroker.QueueNotification(new ApnsNotification()
                    {
                        DeviceToken = notification.SubscriptionId,
                        Payload = JObject.Parse(payload),
                        Tag = notification.Id
                    });


                }
                catch (Exception ex)
                {

                    notification.ErrorLog = ex.Message;
                }
                finally
                {
                    notification.SentTries += 1;
                    UpdateQueuedNotification(notification);
                }
                #endregion
            }
            else if (notification.DeviceType == Domain.DeviceType.Android)
            {
                #region android
                if (string.IsNullOrEmpty(notification.SubscriptionId))
                {
                    notification.IsSent = true;
                    notification.SentTries = +1;
                    UpdateQueuedNotification(notification);
                    return;
                }

                var product = notification.ItemId > 0 ? _productService.GetProductById(notification.ItemId) : null;
                var pictures = product != null ? _pictureService.GetPicturesByProductId(product.Id) : null;
                var defaultPicture = pictures != null ? _pictureService.GetPictureUrl(pictures.FirstOrDefault()) : null;

                var fcmModel = new FcmModel
                {
                    To = notification.SubscriptionId,
                    Data =
                    {
                        NotificationTypeId = notification.NotificationTypeId,
                        ItemId = notification.ItemId,
                        FcmDataTitle = product!=null ? product.Name:"",
                        Summery = product!=null ? product.ShortDescription:"",
                        BigPicture = defaultPicture ?? "" ,


                    }
                };
                fcmModel.Notification.Title = fcmModel.Data.Title = notification.Subject;
                fcmModel.Notification.Body = fcmModel.Data.Body = notification.Message;

                var data = JsonConvert.SerializeObject(fcmModel);

                var response = FcmExtension.RetriveData<FcmErrorObject>("https://fcm.googleapis.com/fcm/send",
                    _notificationSettings.GoogleConsoleAPIAccess_KEY, data, 1);


                if (response != null)
                {
                    notification.SentTries = +1;
                    if (response.Success == 1)
                    {
                        notification.SentOnUtc = DateTime.UtcNow;
                        notification.IsSent = true;
                    }
                    else
                    {
                        if (response.Results != null)
                        {
                            notification.ErrorLog = response.Results.Aggregate(string.Empty, (current, res) => current + res.Error);
                        }

                    }
                    UpdateQueuedNotification(notification);
                }


                #endregion
            }
            else
            {
                #region others

                notification.ErrorLog = "Unknown Device ";
                UpdateQueuedNotification(notification);

                #endregion
            }
        }

        #region test push
        public void SendTestNotication(QueuedNotification notification)
        {

            //if (notification.DeviceType == Domain.DeviceType.iPhone)
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
            if (notification.DeviceType == Domain.DeviceType.Android || notification.DeviceType == Domain.DeviceType.iPhone)
            {
                #region android
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
                //var response = FcmExtension.RetriveData<FcmErrorObject>("https://fcm.googleapis.com/fcm/send", _notificationSettings.GoogleConsoleAPIAccess_KEY, data, 1);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://fcm.googleapis.com/fcm/send");
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Key", "=" + _notificationSettings.GoogleConsoleAPIAccess_KEY);
                    var consoleAPIAccessKey = notification.DeviceType == Domain.DeviceType.Android ? _notificationSettings.GoogleConsoleAPIAccess_KEY : _notificationSettings.AppleCertFileNameWithPath;
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
        #endregion
        #endregion
        #endregion
    }


}

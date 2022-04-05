using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Web.Models.AdminNotification;

namespace Nop.Web.Areas.Admin.Factories
{
    public class AdminPanelNotificationService : IAdminPanelNotificationService
    {
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        public AdminPanelNotificationService(IStoreContext storeContext, ISettingService settingService)
        {
            _storeContext = storeContext;
            _settingService = settingService;
        }
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
    }
}

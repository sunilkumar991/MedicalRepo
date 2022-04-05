using Newtonsoft.Json;

namespace Nop.Web.Models.AdminNotification
{
    public class FcmModel
    {
        public FcmModel()
        {
            Data = new FcmDataModel();
            Notification = new FcmNotificationModel();
        }
        [JsonProperty(PropertyName = "to")]
        public string To { get; set; }
        [JsonProperty(PropertyName = "data")]
        public FcmDataModel Data { get; set; }
        [JsonProperty(PropertyName = "notification")]
        public FcmNotificationModel Notification { get; set; }
    }
}

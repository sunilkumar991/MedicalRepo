using Newtonsoft.Json;

namespace Nop.Web.Models.AdminNotification
{
    public class FcmNotificationModel
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "body")]
        public string Body { get; set; }
    }
}

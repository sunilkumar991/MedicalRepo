using Newtonsoft.Json;

namespace BS.Plugin.NopStation.MobileWebApi.Models
{
    public class FcmNotificationModel
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "body")]
        public string Body { get; set; }
    }
}

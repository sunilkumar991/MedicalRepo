using Newtonsoft.Json;

namespace BS.Plugin.NopStation.MobileWebApi.Models
{
    public class AppleNotification
    {
        [JsonProperty(PropertyName = "aps")]
        public AppleAps Aps { get; set; }
    }
}

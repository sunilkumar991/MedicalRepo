using Newtonsoft.Json;

namespace BS.Plugin.NopStation.MobileWebApi.Models
{
    public class FcmSingleResult
    {
        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }
    }
}

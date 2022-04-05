using Newtonsoft.Json;

namespace Nop.Core.Domain.Messages
{
    public class AppleAps
    {
        public AppleAps()
        {
            CustomObject = new AppleApsCustomObject();
        }
        [JsonProperty(PropertyName = "badge")]
        public int Badge { get; set; }
        [JsonProperty(PropertyName = "alert")]
        public string Alert { get; set; }
        [JsonProperty(PropertyName = "sound")]
        public string Sound { get; set; }
        [JsonProperty(PropertyName = "customObject")]
        public AppleApsCustomObject CustomObject { get; set; }
        public string Subject { get; set; }
    }
}

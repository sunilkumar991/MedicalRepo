using Newtonsoft.Json;

namespace Nop.Core.Domain.Messages
{
    public class AppleNotification
    {
        [JsonProperty(PropertyName = "aps")]
        public AppleAps Aps { get; set; }
    }
}

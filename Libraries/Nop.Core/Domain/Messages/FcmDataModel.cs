using Newtonsoft.Json;

namespace Nop.Core.Domain.Messages
{
    public class FcmDataModel : FcmNotificationModel
    {
        [JsonProperty(PropertyName = "notificationTypeId")]
        public int NotificationTypeId { get; set; }
        [JsonProperty(PropertyName = "itemId")]
        public int ItemId { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string FcmDataTitle { get; set; }
        [JsonProperty(PropertyName = "summery")]
        public string Summery { get; set; }
        [JsonProperty(PropertyName = "bigPicture")]
        public string BigPicture { get; set; }
    }
}

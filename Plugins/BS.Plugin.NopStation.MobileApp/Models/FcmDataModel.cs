using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileApp.Models
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

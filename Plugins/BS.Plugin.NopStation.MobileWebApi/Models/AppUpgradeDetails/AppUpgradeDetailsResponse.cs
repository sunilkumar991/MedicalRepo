using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models.AppUpgradeDetails
{
    public class AppUpgradeDetailsResponse : BaseResponse
    {
        public int AppID { get; set; }
        public string AppLatestVersionName { get; set; }
        public int AppLatestVersionCode { get; set; }
        public string AppURL { get; set; }
        public bool IsUpdateRequired { get; set; }
        public bool IsBackgroundDownload { get; set; }
        public string message { get; set; }
        public bool IsProduction { get; set; }
        public string playStoreUrl { get; set; }
        public string apkName { get; set; }
        public bool IsUpdateForceRequired { get; set; }

        public int DeviceId { get; set; }
    }
}

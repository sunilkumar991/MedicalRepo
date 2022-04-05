using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models.AppUpgradeDetails
{
    public class IOSSMSNumber : BaseResponse
    {
        public string MobileNumber { get; set; }
    }
}

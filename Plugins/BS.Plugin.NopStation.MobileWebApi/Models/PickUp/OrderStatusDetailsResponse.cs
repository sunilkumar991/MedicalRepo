using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models.PickUp
{
    public class OrderStatusDetailsResponse : BaseResponse
    {
        public int OrderStatusId { get; set; }

        public string DeviceId { get; set; }
    }
}

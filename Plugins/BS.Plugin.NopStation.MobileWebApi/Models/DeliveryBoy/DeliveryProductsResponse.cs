using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DeliveryBoy
{
    public class DeliveryProductsResponse : BaseResponse
    {
        public DeliveryProductsResponse()
        {
            OrderDetails = new List<DeliveryOrderDetail>();
        }

        public List<DeliveryOrderDetail> OrderDetails { get; set; }
    }

}

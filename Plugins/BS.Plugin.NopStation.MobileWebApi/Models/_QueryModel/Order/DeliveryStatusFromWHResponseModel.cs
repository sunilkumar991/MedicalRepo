using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Order
{
    public class DeliveryStatusFromWHResponseModel
    {
        public string OrderID { get; set; }
        public string SentToDelivery { get; set; }
    }
}

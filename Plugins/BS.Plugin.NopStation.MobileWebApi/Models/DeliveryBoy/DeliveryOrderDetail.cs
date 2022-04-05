using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DeliveryBoy
{
    public class DeliveryOrderDetail
    {
        public string CustomerName { get; set; }
        public string MobileNumber { get; set; }
        public string OrderNumber { get; set; }
        public string[] ProductIds { get; set; }
        public string Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string DeliveryCode { get; set; }
    }
}

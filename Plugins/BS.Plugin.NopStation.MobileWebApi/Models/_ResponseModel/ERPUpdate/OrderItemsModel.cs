using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.ERPUpdate
{
    public class OrderItemsModel
    {
        public string id { get; set; }
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public string Quantity { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.ERPUpdate
{
    public class OrderDetailsModel
    {
        public List<OrderItemsModel> orderItems { get; set; }
        public string id { get; set; }
        public string orderGuid { get; set; }
        public string customerId { get; set; }
        public string orderTotal { get; set; }

    }
}

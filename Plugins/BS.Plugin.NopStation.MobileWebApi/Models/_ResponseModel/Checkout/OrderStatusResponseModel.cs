using System.Collections.Generic;
using Nop.Web.Models.Checkout;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Order;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Checkout
{
    public class OrderStatusResponseModel : BaseResponse
    {
        public string OrderGroupNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public string ShippingAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string ProductAmount { get; set; }
        public string TotalTax { get; set; }
        public string ShippingCharges { get; set; }
        public string TotalPaidAmount { get; set; }
        public string OrderStatus { get; set; }
        //public List<CheckoutOrderDetailJsonModel> Orders { get; set; }
        public List<OrderDetailsResponseModel> Orders { get; set; }
        public string DiscountAmount { get; set; }

    }
}

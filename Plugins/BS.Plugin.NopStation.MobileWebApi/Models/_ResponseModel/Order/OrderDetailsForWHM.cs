using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Order
{
    public class OrderDetailsForWHM
    {
        public OrderDetailsForWHM()
        {
            ProductList = new List<ProductList>();
            DeliveryAddress = new DeliveryAddress();
        }
        public int OrderId { get; set; }
        public string CustomOrderNumber { get; set; }
        public List<ProductList> ProductList { get; set; }
        //public PickupAddress PickupAddress { get; set; }
        public DeliveryAddress DeliveryAddress { get; set; }

    }

    public class BaseCommonFields
    {
        public string MobileNumber { get; set; }
        public string Address { get; set; }
    }

    public class DeliveryAddress : BaseCommonFields
    {
        public string CustomerName { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public class PickupAddress : BaseCommonFields
    {
        public string SupplierName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public class ProductList
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Price { get; set; }
        public string PriceWithDiscount { get; set; }
        public int Quantity { get; set; }
        public string AttributeInfo { get; set; }
        public string ImageUrl { get; set; }
        public string Category { get; set; }
        //public string SubCategory { get; set; }
        public PickupAddress PickupAddress { get; set; }
        public bool IsOneStopMartVendor { get; set; }
        public bool IsSpecialRequirement { get; set; }
        public string IsSpecialRequirementDesc { get; set; }
    }
}

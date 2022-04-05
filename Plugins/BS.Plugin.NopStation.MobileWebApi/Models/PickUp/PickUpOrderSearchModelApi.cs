using BS.Plugin.NopStation.MobileWebApi.Model;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models.PickUp
{
    public partial class PickUpOrderSearchModelApi : BaseResponse
    {
        #region Ctor

        public PickUpOrderSearchModelApi()
        {
            //OrderStatusIds = new List<int>();
            PaymentStatusIds = new List<int>();
            ShippingStatusIds = new List<int>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Orders.List.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        //[NopResourceDisplayName("Admin.Orders.List.OrderStatus")]
        //public List<int> OrderStatusIds { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.PaymentStatus")]
        public List<int> PaymentStatusIds { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.ShippingStatus")]
        public List<int> ShippingStatusIds { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.PaymentMethod")]
        public string PaymentMethodSystemName { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.Store")]
        public int StoreId { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.Vendor")]
        public int VendorId { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.Warehouse")]
        public int WarehouseId { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.Product")]
        public int ProductId { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.BillingEmail")]
        public string BillingEmail { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.BillingPhone")]
        public string BillingPhone { get; set; }

        public bool BillingPhoneEnabled { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.BillingLastName")]
        public string BillingLastName { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.BillingCountry")]
        public int BillingCountryId { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.OrderNotes")]
        public string OrderNotes { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.CustomOrderNumber")]
        public string CustomOrderNumber { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.GoDirectlyToNumber")]
        public string GoDirectlyToCustomOrderNumber { get; set; }

        public bool IsLoggedInAsVendor { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.DeviceId")]
        public string DeviceId { get; set; }


        [NopResourceDisplayName("Admin.Orders.List.EditDateTime")]
        [UIHint("DateNullable")]
        public DateTime? EditDateTime { get; set; }

        
        [NopResourceDisplayName("Admin.Orders.List.EditEndDate")]
        [UIHint("DateNullable")]
        public DateTime? EditEndDate { get; set; }

        

        #endregion
    }
}

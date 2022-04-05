using System;
using System.Collections.Generic;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using Nop.Core.Domain.Orders;
using Nop.Web.Framework.Models;
using PictureModel = BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel.PictureModel;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Order
{
    public partial class CustomerOrderListResponseModel : BaseResponse
    {
        public CustomerOrderListResponseModel()
        {
            Orders = new List<OrderDetailsModel>();
            RecurringOrders = new List<RecurringOrderModel>();
            CancelRecurringPaymentErrors = new List<string>();
        }

        public IList<OrderDetailsModel> Orders { get; set; }
        public IList<RecurringOrderModel> RecurringOrders { get; set; }
        public IList<string> CancelRecurringPaymentErrors { get; set; }

        #region Nested classes

        public partial class OrderDetailsModel : BaseNopEntityModel
        {
            public string ExpectedDeliveryDate { get; set; }
            public string CustomOrderNumber { get; set; }
            public string OrderDeliveryCode { get; set; }
            public string OrderTotal { get; set; }
            public bool IsReturnRequestAllowed { get; set; }
            public OrderStatus OrderStatusEnum { get; set; }
            public string OrderStatus { get; set; }
            public string PaymentStatus { get; set; }
            public string ShippingStatus { get; set; }

            public string PaymentMethodFee { get; set; }
            public DateTime CreatedOn { get; set; }
            public IList<OrderItemModel> Items { get; set; }
        }

        public partial class RecurringOrderModel : BaseNopEntityModel
        {
            public string StartDate { get; set; }
            public string CycleInfo { get; set; }
            public string NextPayment { get; set; }
            public int TotalCycles { get; set; }
            public int CyclesRemaining { get; set; }
            public int InitialOrderId { get; set; }
            public bool CanRetryLastPayment { get; set; }
            public string InitialOrderNumber { get; set; }
            public bool CanCancel { get; set; }
        }

        public partial class OrderItemModel : BaseNopEntityModel
        {

            public Guid OrderItemGuid { get; set; }
            public string Sku { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string ProductSeName { get; set; }
            public string UnitPrice { get; set; }
            public string SubTotal { get; set; }
            public int Quantity { get; set; }
            public string AttributeInfo { get; set; }
            public string RentalInfo { get; set; }
            public PictureModel Picture { get; set; }
            //downloadable product properties
            public int DownloadId { get; set; }
            public int LicenseId { get; set; }
        }

        #endregion
    }
}
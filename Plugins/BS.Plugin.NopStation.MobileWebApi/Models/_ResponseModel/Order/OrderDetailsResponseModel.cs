using System;
using System.Collections.Generic;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel;
using Nop.Web.Framework.Models;

using PictureModel = BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel.PictureModel;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Order
{
    public partial class OrderDetailsResponseModel : BaseResponse
    {
        public OrderDetailsResponseModel()
        {
            TaxRates = new List<TaxRate>();
            GiftCards = new List<GiftCard>();
            Items = new List<OrderItemModel>();
            OrderNotes = new List<OrderNote>();
            Shipments = new List<ShipmentBriefModel>();

            BillingAddress = new AddressModel();
            ShippingAddress = new AddressModel();
            PickupAddress = new AddressModel();

            CustomValues = new Dictionary<string, object>();
        }
        public int Id { get; set; }
        public bool PrintMode { get; set; }
        public bool PdfInvoiceDisabled { get; set; }
        public string ReferenceNumber { get; set; }//Added By Sunil Kumar on 5th Jan 2019
        public string CustomOrderNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ExpectedDeliveryDate { get; set; }  //Added By Ankur Shrivastava on 18th October 2018



        public string OrderStatus { get; set; }

        public bool IsReOrderAllowed { get; set; }

        public bool IsReturnRequestAllowed { get; set; }

        public bool IsShippable { get; set; }
        public bool PickUpInStore { get; set; }
        public string ShippingStatus { get; set; }
        public AddressModel ShippingAddress { get; set; }
        public AddressModel PickupAddress { get; set; }
        public string ShippingMethod { get; set; }
        public IList<ShipmentBriefModel> Shipments { get; set; }

        public AddressModel BillingAddress { get; set; }

        public string VatNumber { get; set; }

        public string PaymentMethod { get; set; }

        public string PaymentMethodSubType { get; set; }
        public string PaymentMethodStatus { get; set; }
        public bool CanRePostProcessPayment { get; set; }
        public Dictionary<string, object> CustomValues { get; set; }

        public string OrderSubtotal { get; set; }
        public string OrderSubTotalDiscount { get; set; }
        public string OrderShipping { get; set; }
        public string PaymentMethodAdditionalFee { get; set; }
        public string CheckoutAttributeInfo { get; set; }

        public bool PricesIncludeTax { get; set; }
        public bool DisplayTaxShippingInfo { get; set; }
        public string Tax { get; set; }
        public IList<TaxRate> TaxRates { get; set; }
        public bool DisplayTax { get; set; }
        public bool DisplayTaxRates { get; set; }

        public string OrderTotalDiscount { get; set; }
        public int RedeemedRewardPoints { get; set; }
        public string RedeemedRewardPointsAmount { get; set; }
        public string OrderTotal { get; set; }

        public IList<GiftCard> GiftCards { get; set; }

        public bool ShowSku { get; set; }
        public IList<OrderItemModel> Items { get; set; }

        public IList<OrderNote> OrderNotes { get; set; }

        #region Nested Classes

        public partial class OrderItemModel : BaseNopEntityModel
        {
            public OrderItemModel()
            {
                UserRating = new UsersRating();
            }
            public Guid OrderItemGuid { get; set; }
            public string Sku { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string ProductSeName { get; set; }
            public string UnitPrice { get; set; }
            //Added By Sunil Kumar at 23-01-19
            public string OldPrice { get; set; }
            public string SubTotal { get; set; }
            public int Quantity { get; set; }
            public string AttributeInfo { get; set; }
            public string RentalInfo { get; set; }
            public PictureModel Picture { get; set; }
            //downloadable product properties
            public int DownloadId { get; set; }
            public int LicenseId { get; set; }
            //Added By Sunil Kumar at 28-11-18
            public ProductOverViewModelApi.ProductReviewOverviewModel ProductReviewOverview { get; set; }
            public UsersRating UserRating { get; set; }
            public int DiscountPercentage { get; set; }
            public string DiscountOfferName { get; set; }
            public string DiscountAmount { get; set; }
            public string PriceAfterDiscount { get; set; }
            public string ActualPrice { get; set; }
        }

        public partial class TaxRate : BaseNopModel
        {
            public string Rate { get; set; }
            public string Value { get; set; }
        }

        public partial class GiftCard : BaseNopModel
        {
            public string CouponCode { get; set; }
            public string Amount { get; set; }
        }

        public partial class OrderNote : BaseNopEntityModel
        {
            public bool HasDownload { get; set; }
            public string Note { get; set; }
            //public string CreatedOn { get; set; }
            public DateTime CreatedOn { get; set; }
        }

        public partial class ShipmentBriefModel : BaseNopEntityModel
        {
            public string TrackingNumber { get; set; }
            public DateTime? ShippedDate { get; set; }
            public DateTime? DeliveryDate { get; set; }
        }

        //Added by Alexandar Rajavel on 17-Mar-2019
        public partial class UsersRating
        {
            public string Comments { get; set; }
            public int Rating { get; set; }
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel;
using Nop.Core.Domain.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Models;
using PictureModel = BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel.PictureModel;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.ShoppingCart
{
    public class ShoppingCartResponseModel : BaseResponse
    {
        public ShoppingCartResponseModel()
        {
            Items = new List<ShoppingCartItemModel>();
            Warnings = new List<string>();
            CheckoutAttributes = new List<CheckoutAttributeModel>();
            DiscountBox = new DiscountBoxModel();
            GiftCardBox = new GiftCardBoxModel();
            OrderReviewData = new OrderReviewDataModel();
            OrderTotalResponseModel = new OrderTotalsResponseModel();
        }

        public bool OnePageCheckoutEnabled { get; set; }
        public bool ShowSku { get; set; }
        public bool ShowProductImages { get; set; }
        public IList<ShoppingCartItemModel> Items { get; set; }
        public string CheckoutAttributeInfo { get; set; }
        public IList<CheckoutAttributeModel> CheckoutAttributes { get; set; }
        public IList<string> Warnings { get; set; }
        public OrderReviewDataModel OrderReviewData { get; set; }
        public int Count { get; set; }
        public DiscountBoxModel DiscountBox { get; set; }
        public GiftCardBoxModel GiftCardBox { get; set; }
        public bool TermsOfServiceOnShoppingCartPage { get; set; }
        public bool TermsOfServiceOnOrderConfirmPage { get; set; }
        public OrderTotalsResponseModel OrderTotalResponseModel { get; set; }
        public bool CanContinue { get; set; }



        #region Nested Classes

        public partial class ShoppingCartItemModel : BaseNopEntityModel
        {
            public ShoppingCartItemModel()
            {
                Picture = new PictureModel();
                AllowedQuantities = new List<SelectListItem>();
                Warnings = new List<string>();
            }
            public string Sku { get; set; }

            public PictureModel Picture { get; set; }

            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public string ProductSeName { get; set; }

            public string UnitPrice { get; set; }

            public string SubTotal { get; set; }

            public string Discount { get; set; }

            //Added by Sunil Kumar At 25-02-17
            public string OldPrice { get; set; }
            public string Price { get; set; }
            public int DiscountPercentage { get; set; }
            public string DiscountOfferName { get; set; }

            public int? MaximumDiscountedQty { get; set; }

            public int Quantity { get; set; }
            public int AvailableQuantity { get; set; }//added by Sunil Kumar at 5/1/19
            public int MaxItemQuantity { get; set; }//added by Sunil at 28-05-2020
            public List<SelectListItem> AllowedQuantities { get; set; }

            public string AttributeInfo { get; set; }

            public string RecurringInfo { get; set; }

            public string RentalInfo { get; set; }

            public bool AllowItemEditing { get; set; }

            public bool DisableRemoval { get; set; }

            public IList<string> Warnings { get; set; }

            public string DeliveryDays { get; set; } //Added by Ankur on 15-OCT-2018

            public string Weight { get; set; }//Added by Sunil Kumar on 13-01-2020

            public string ShippingCharge { get; set; }//Added by Sunil Kumar on 13-01-2020

            //public string KgCharge { get; set; }//Added by Sunil Kumar on 25-02-2020

            //public string DeliveryCharge { get; set; }//Added by Sunil Kumar on 25-02-2020

            public string CGMItemID { get; set; }

        }

        public partial class CheckoutAttributeModel : BaseNopEntityModel
        {
            public CheckoutAttributeModel()
            {
                AllowedFileExtensions = new List<string>();
                Values = new List<CheckoutAttributeValueModel>();
            }

            public string Name { get; set; }

            public string DefaultValue { get; set; }

            public string TextPrompt { get; set; }

            public bool IsRequired { get; set; }

            /// <summary>
            /// Selected day value for datepicker
            /// </summary>
            public int? SelectedDay { get; set; }
            /// <summary>
            /// Selected month value for datepicker
            /// </summary>
            public int? SelectedMonth { get; set; }
            /// <summary>
            /// Selected year value for datepicker
            /// </summary>
            public int? SelectedYear { get; set; }

            /// <summary>
            /// Allowed file extensions for customer uploaded files
            /// </summary>
            public IList<string> AllowedFileExtensions { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<CheckoutAttributeValueModel> Values { get; set; }
        }

        public partial class CheckoutAttributeValueModel : BaseNopEntityModel
        {
            public string Name { get; set; }

            public string ColorSquaresRgb { get; set; }

            public string PriceAdjustment { get; set; }

            public bool IsPreSelected { get; set; }
        }

        public partial class OrderReviewDataModel : BaseNopModel
        {
            public OrderReviewDataModel()
            {
                this.BillingAddress = new AddressModel();
                this.ShippingAddress = new AddressModel();
                this.PickupAddress = new AddressModel();
                this.CustomValues = new Dictionary<string, object>();
            }
            public bool Display { get; set; }

            public AddressModel BillingAddress { get; set; }

            public bool IsShippable { get; set; }
            public AddressModel ShippingAddress { get; set; }
            public AddressModel PickupAddress { get; set; }
            public bool SelectedPickUpInStore { get; set; }
            public string ShippingMethod { get; set; }

            public string PaymentMethod { get; set; }

            public Dictionary<string, object> CustomValues { get; set; }
        }

        public partial class DiscountBoxModel : BaseNopModel
        {
            public DiscountBoxModel()
            {
                AppliedDiscountsWithCodes = new List<DiscountInfoModel>();
                Messages = new List<string>();
            }

            public List<DiscountInfoModel> AppliedDiscountsWithCodes { get; set; }
            public bool Display { get; set; }
            public List<string> Messages { get; set; }
            public bool IsApplied { get; set; }

            public class DiscountInfoModel : BaseNopEntityModel
            {
                public string CouponCode { get; set; }
            }
        }

        public partial class GiftCardBoxModel : BaseNopModel
        {
            public bool Display { get; set; }
            public string Message { get; set; }
            public bool IsApplied { get; set; }
        }
        #endregion
    }
}

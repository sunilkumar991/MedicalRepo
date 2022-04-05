using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Services.Seo;
using Nop.Services.Customers;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.ShoppingCart;
using BS.Plugin.NopStation.MobileWebApi.Infrastructure.Cache;
using BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Security;
using Nop.Services.Shipping.Date;
using Microsoft.Extensions.Configuration;

namespace BS.Plugin.NopStation.MobileWebApi.Factories
{
    /// <summary>
    /// Represents the shopping cart model factory
    /// </summary>
    public partial class ShoppingCartModelFactoryApi : IShoppingCartModelFactoryApi
    {
        #region Fields
        private IConfiguration _configuration;
        private readonly IAddressModelFactoryApi _addressModelFactoryApi;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IDiscountService _discountService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IShippingService _shippingService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly IPaymentService _paymentService;
        private readonly IPermissionService _permissionService;
        private readonly IDownloadService _downloadService;
        private readonly ICacheManager _cacheManager;
        private readonly IWebHelper _webHelper;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly MediaSettings _mediaSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly OrderSettings _orderSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly AddressSettings _addressSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerService _customerService;
        private readonly IGiftCardService _giftCardService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IProductService _productService;
        private readonly IDateRangeService _dateRangeService;
        private readonly IAddressService _addressService;
        private readonly ICityService _cityService;

        #endregion

        #region Ctor

        public ShoppingCartModelFactoryApi(IAddressModelFactoryApi addressModelFactoryApi,
            IStoreContext storeContext,
            IWorkContext workContext,
            IShoppingCartService shoppingCartService,
            IPictureService pictureService,
            ILocalizationService localizationService,
            IProductAttributeFormatter productAttributeFormatter,
            IProductAttributeParser productAttributeParser,
            ITaxService taxService, ICurrencyService currencyService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            IOrderProcessingService orderProcessingService,
            IDiscountService discountService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IShippingService shippingService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ICheckoutAttributeService checkoutAttributeService,
            IPaymentService paymentService,
            IPermissionService permissionService,
            IDownloadService downloadService,
            ICacheManager cacheManager,
            IWebHelper webHelper,
            IGenericAttributeService genericAttributeService,
            MediaSettings mediaSettings,
            ShoppingCartSettings shoppingCartSettings,
            CatalogSettings catalogSettings,
            OrderSettings orderSettings,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings,
            CaptchaSettings captchaSettings,
            AddressSettings addressSettings,
            RewardPointsSettings rewardPointsSettings,
            CustomerSettings customerSettings,
            ICustomerService customerService,
            IGiftCardService giftCardService,
            IUrlRecordService urlRecordService,
            IProductService productService,
            IDateRangeService dateRangeService,
            IAddressService addressService,
            ICityService cityService,
            IConfiguration iconfigNew)
        {
            this._addressModelFactoryApi = addressModelFactoryApi;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._shoppingCartService = shoppingCartService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._productAttributeFormatter = productAttributeFormatter;
            this._productAttributeParser = productAttributeParser;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._checkoutAttributeFormatter = checkoutAttributeFormatter;
            this._orderProcessingService = orderProcessingService;
            this._discountService = discountService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._shippingService = shippingService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._checkoutAttributeService = checkoutAttributeService;
            this._paymentService = paymentService;
            this._permissionService = permissionService;
            this._downloadService = downloadService;
            this._cacheManager = cacheManager;
            this._webHelper = webHelper;
            this._genericAttributeService = genericAttributeService;
            this._mediaSettings = mediaSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._catalogSettings = catalogSettings;
            this._orderSettings = orderSettings;
            this._shippingSettings = shippingSettings;
            this._taxSettings = taxSettings;
            this._captchaSettings = captchaSettings;
            this._addressSettings = addressSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._customerSettings = customerSettings;
            _customerService = customerService;
            _giftCardService = giftCardService;
            _urlRecordService = urlRecordService;
            _productService = productService;
            _dateRangeService = dateRangeService;
            _addressService = addressService;
            this._cityService = cityService;
            this._configuration = iconfigNew;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare the checkout attribute models
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <returns>List of the checkout attribute model</returns>
        protected virtual IList<ShoppingCartResponseModel.CheckoutAttributeModel> PrepareCheckoutAttributeModels(IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            var model = new List<ShoppingCartResponseModel.CheckoutAttributeModel>();
            var requiresShipping = _shoppingCartService.ShoppingCartRequiresShipping(cart);

            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, !requiresShipping);
            foreach (var attribute in checkoutAttributes)
            {
                var attributeModel = new ShoppingCartResponseModel.CheckoutAttributeModel
                {
                    Id = attribute.Id,
                    Name = _localizationService.GetLocalized(attribute, x => x.Name),
                    TextPrompt = _localizationService.GetLocalized(attribute, x => x.TextPrompt),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    DefaultValue = attribute.DefaultValue
                };
                if (!String.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
                {
                    attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _checkoutAttributeService.GetCheckoutAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new ShoppingCartResponseModel.CheckoutAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = _localizationService.GetLocalized(attributeValue, x => x.Name),
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb,
                            IsPreSelected = attributeValue.IsPreSelected,
                        };
                        attributeModel.Values.Add(attributeValueModel);

                        //display price if allowed
                        if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                        {
                            decimal priceAdjustmentBase = _taxService.GetCheckoutAttributePrice(attributeValue);
                            decimal priceAdjustment = _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, _workContext.WorkingCurrency);
                            if (priceAdjustmentBase > decimal.Zero)
                                attributeValueModel.PriceAdjustment = "+" + _priceFormatter.FormatPrice(priceAdjustment);
                            else if (priceAdjustmentBase < decimal.Zero)
                                attributeValueModel.PriceAdjustment = "-" + _priceFormatter.FormatPrice(-priceAdjustment);
                        }
                    }
                }



                //set already selected attributes
                var selectedCheckoutAttributes = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer,
                    NopCustomerDefaults.CheckoutAttributes, _storeContext.CurrentStore.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                    case AttributeControlType.ColorSquares:
                        {
                            if (!String.IsNullOrEmpty(selectedCheckoutAttributes))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(selectedCheckoutAttributes);
                                foreach (var attributeValue in selectedValues)
                                    foreach (var item in attributeModel.Values)
                                        if (attributeValue.Id == item.Id)
                                            item.IsPreSelected = true;
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //do nothing
                            //values are already pre-set
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            if (!String.IsNullOrEmpty(selectedCheckoutAttributes))
                            {
                                var enteredText = _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
                                if (enteredText.Any())
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            //keep in mind my that the code below works only in the current culture
                            var selectedDateStr = _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
                            if (selectedDateStr.Any())
                            {
                                DateTime selectedDate;
                                if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture,
                                                       DateTimeStyles.None, out selectedDate))
                                {
                                    //successfully parsed
                                    attributeModel.SelectedDay = selectedDate.Day;
                                    attributeModel.SelectedMonth = selectedDate.Month;
                                    attributeModel.SelectedYear = selectedDate.Year;
                                }
                            }

                        }
                        break;
                    default:
                        break;
                }

                model.Add(attributeModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare the shopping cart item model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="sci">Shopping cart item</param>
        /// <returns>Shopping cart item model</returns>
        protected virtual ShoppingCartResponseModel.ShoppingCartItemModel PrepareShoppingCartItemModel(IList<ShoppingCartItem> cart, ShoppingCartItem sci)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (sci == null)
                throw new ArgumentNullException("sci");


            var cartItemModel = new ShoppingCartResponseModel.ShoppingCartItemModel
            {
                Id = sci.Id,
                Sku = _productService.FormatSku(sci.Product, sci.AttributesXml),
                ProductId = sci.Product.Id,
                ProductName = _localizationService.GetLocalized(sci.Product, x => x.Name),
                //ProductSeName = _urlRecordService.GetSeName(sci.Product),
                ProductSeName = "",
                Quantity = sci.Quantity,
                //Added by Sunil Kumar at 25-02-19
                OldPrice = _priceFormatter.FormatPrice(sci.Product.OldPrice),
                Price = _priceFormatter.FormatPrice(sci.Product.Price),
                AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml),
                CGMItemID = sci.Product.CGMItemID
            };
            //Sunil kumar // on 25-07-2020//
            ////Added By Ankur for displaying delivery date in shopping cart list for each product..
            //var deliveryDate = _dateRangeService.GetDeliveryDateById(sci.Product.DeliveryDateId);
            //if (deliveryDate != null)
            //{
            //    cartItemModel.DeliveryDays = _localizationService.GetLocalized(deliveryDate, dd => dd.Name);
            //}


            ////Added By Ankur for displaying delivery date in shopping cart list for each product..
            ////var deliveryDate = _dateRangeService.GetDeliveryDateById(sci.Product.DeliveryDateId);
            ////if (deliveryDate != null)
            ////{


            //string AddHoursNew = _configuration.GetValue<string>("ShippingHours:Hours");


            //cartItemModel.DeliveryDays = AddHoursNew;
            ////}
            ///
              //Added By Sunil kumar for displaying delivery date in shopping cart list for each product.. on 29.10.2020
            var deliveryDate = _dateRangeService.GetDeliveryDateById(sci.Product.DeliveryDateId);
            if (deliveryDate != null)
            {
                cartItemModel.DeliveryDays = _localizationService.GetLocalized(deliveryDate, dd => dd.Name);
            }



            //allow editing?
            //1. setting enabled?
            //2. simple product?
            //3. has attribute or gift card?
            //4. visible individually?
            cartItemModel.AllowItemEditing = _shoppingCartSettings.AllowCartItemEditing &&
                                             sci.Product.ProductType == ProductType.SimpleProduct &&
                                             (!String.IsNullOrEmpty(cartItemModel.AttributeInfo) ||
                                              sci.Product.IsGiftCard) &&
                                             sci.Product.VisibleIndividually;

            //disable removal?
            //1. do other items require this one?

            //Sunil kumar // on 25-07-2020//
           // cartItemModel.DisableRemoval = cart.Any(item => item.Product.RequireOtherProducts && _productService.ParseRequiredProductIds(item.Product).Contains(sci.ProductId));

            //allowed quantities
            //var allowedQuantities = _productService.ParseAllowedQuantities(sci.Product);
            //foreach (var qty in allowedQuantities)
            //{
            //    cartItemModel.AllowedQuantities.Add(new SelectListItem
            //    {
            //        Text = qty.ToString(),
            //        Value = qty.ToString(),
            //        Selected = sci.Quantity == qty
            //    });
            //}

            //recurring info
            //if (sci.Product.IsRecurring)
            //    cartItemModel.RecurringInfo = string.Format(_localizationService.GetResource("ShoppingCart.RecurringPeriod"),
            //            sci.Product.RecurringCycleLength, _localizationService.GetLocalizedEnum(sci.Product.RecurringCyclePeriod));

            ////rental info
            //if (sci.Product.IsRental)
            //{
            //    var rentalStartDate = sci.RentalStartDateUtc.HasValue
            //        ? _productService.FormatRentalDate(sci.Product, sci.RentalStartDateUtc.Value)
            //        : "";
            //    var rentalEndDate = sci.RentalEndDateUtc.HasValue
            //        ? _productService.FormatRentalDate(sci.Product, sci.RentalEndDateUtc.Value)
            //        : "";
            //    cartItemModel.RentalInfo =
            //        string.Format(_localizationService.GetResource("ShoppingCart.Rental.FormattedDate"),
            //            rentalStartDate, rentalEndDate);
            //}

            //unit prices
            if (sci.Product.CallForPrice)
            {
                cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
            }
            else
            {
                decimal taxRate;
                decimal shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetUnitPrice(sci), out taxRate);
                decimal shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                cartItemModel.UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
            }
            //subtotal, discount
            if (sci.Product.CallForPrice)
            {
                cartItemModel.SubTotal = _localizationService.GetResource("Products.CallForPrice");
            }
            else
            {
                //sub total
                List<DiscountForCaching> scDiscounts;
                int? maximumDiscountQty;
                decimal shoppingCartItemDiscountBase;
                decimal taxRate;
                decimal shoppingCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetSubTotal(sci, true, out shoppingCartItemDiscountBase, out scDiscounts, out maximumDiscountQty), out taxRate);
                decimal shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, _workContext.WorkingCurrency);
                cartItemModel.SubTotal = _priceFormatter.FormatPrice(shoppingCartItemSubTotalWithDiscount);
                cartItemModel.MaximumDiscountedQty = maximumDiscountQty;

                //display an applied discount amount
                if (shoppingCartItemDiscountBase > decimal.Zero)
                {
                    shoppingCartItemDiscountBase = _taxService.GetProductPrice(sci.Product, shoppingCartItemDiscountBase, out taxRate);
                    if (shoppingCartItemDiscountBase > decimal.Zero)
                    {
                        decimal shoppingCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemDiscountBase, _workContext.WorkingCurrency);
                        cartItemModel.Discount = _priceFormatter.FormatPrice(shoppingCartItemDiscount);
                    }
                }
            }
            //Added by Sunil Kumar at 25-02-19
            cartItemModel.DiscountOfferName = string.Empty;
            if (sci.Product.HasDiscountsApplied)
            {
                //Added by Sunil Kumar at 28-02-19
                var discount = _discountService.GetDiscountPercentageAndName(sci.Product);
                cartItemModel.DiscountPercentage = discount.Item1;
                cartItemModel.DiscountOfferName = discount.Item2;
            }
            //picture
            if (_shoppingCartSettings.ShowProductImagesOnShoppingCart)
            {
                cartItemModel.Picture = PrepareCartItemPictureModel(sci,
                    _mediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName);
            }

            ////item warnings
            //var itemWarnings = _shoppingCartService.GetShoppingCartItemWarnings(
            //    _workContext.CurrentCustomer,
            //    sci.ShoppingCartType,
            //    sci.Product,
            //    sci.StoreId,
            //    sci.AttributesXml,
            //    sci.CustomerEnteredPrice,
            //    sci.RentalStartDateUtc,
            //    sci.RentalEndDateUtc,
            //    sci.Quantity,
            //    false);
            //foreach (var warning in itemWarnings.ErrorList)
            //    cartItemModel.Warnings.Add(warning);

            return cartItemModel;
        }

        /// <summary>
        /// Prepare the wishlist item model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="sci">Shopping cart item</param>
        /// <returns>Shopping cart item model</returns>
        protected virtual WishlistResponseModel.ShoppingCartItemModel PrepareWishlistItemModel(IList<ShoppingCartItem> cart, ShoppingCartItem sci)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (sci == null)
                throw new ArgumentNullException("sci");

            var cartItemModel = new WishlistResponseModel.ShoppingCartItemModel
            {
                Id = sci.Id,
                Sku = _productService.FormatSku(sci.Product, sci.AttributesXml),
                ProductId = sci.Product.Id,
                ProductName = _localizationService.GetLocalized(sci.Product, x => x.Name),
                ProductSeName = _urlRecordService.GetSeName(sci.Product),
                Quantity = sci.Quantity,
                AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml),
            };

            //allowed quantities
            var allowedQuantities = _productService.ParseAllowedQuantities(sci.Product);
            foreach (var qty in allowedQuantities)
            {
                cartItemModel.AllowedQuantities.Add(new SelectListItem
                {
                    Text = qty.ToString(),
                    Value = qty.ToString(),
                    Selected = sci.Quantity == qty
                });
            }


            //recurring info
            if (sci.Product.IsRecurring)
                cartItemModel.RecurringInfo = string.Format(_localizationService.GetResource("ShoppingCart.RecurringPeriod"),
                        sci.Product.RecurringCycleLength, _localizationService.GetLocalizedEnum(sci.Product.RecurringCyclePeriod));

            //rental info
            if (sci.Product.IsRental)
            {
                var rentalStartDate = sci.RentalStartDateUtc.HasValue
                    ? _productService.FormatRentalDate(sci.Product, sci.RentalStartDateUtc.Value)
                    : "";
                var rentalEndDate = sci.RentalEndDateUtc.HasValue
                    ? _productService.FormatRentalDate(sci.Product, sci.RentalEndDateUtc.Value)
                    : "";
                cartItemModel.RentalInfo =
                    string.Format(_localizationService.GetResource("ShoppingCart.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
            }

            //unit prices
            if (sci.Product.CallForPrice)
            {
                cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
            }
            else
            {
                decimal taxRate;
                decimal shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(sci.Product,
                    _priceCalculationService.GetUnitPrice(sci), out taxRate);
                decimal shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                cartItemModel.UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
            }
            //subtotal, discount
            if (sci.Product.CallForPrice)
            {
                cartItemModel.SubTotal = _localizationService.GetResource("Products.CallForPrice");
            }
            else
            {
                //sub total
                List<DiscountForCaching> scDiscounts;
                int? maximumDiscountQty;
                decimal shoppingCartItemDiscountBase;
                decimal taxRate;
                decimal shoppingCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetSubTotal(sci, true, out shoppingCartItemDiscountBase, out scDiscounts,
                        out maximumDiscountQty), out taxRate);
                decimal shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, _workContext.WorkingCurrency);
                cartItemModel.SubTotal = _priceFormatter.FormatPrice(shoppingCartItemSubTotalWithDiscount);
                cartItemModel.MaximumDiscountedQty = maximumDiscountQty;

                //display an applied discount amount
                if (shoppingCartItemDiscountBase > decimal.Zero)
                {
                    shoppingCartItemDiscountBase = _taxService.GetProductPrice(sci.Product, shoppingCartItemDiscountBase, out taxRate);
                    if (shoppingCartItemDiscountBase > decimal.Zero)
                    {
                        decimal shoppingCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemDiscountBase, _workContext.WorkingCurrency);
                        cartItemModel.Discount = _priceFormatter.FormatPrice(shoppingCartItemDiscount);
                    }
                }
            }

            //Added By Sunil Kumar at 28/1/19
            cartItemModel.Discount = cartItemModel.Discount == null ? string.Empty : cartItemModel.Discount;

            //Added By Sunil Kumar at 29/1/19
            cartItemModel.Price = sci.Product.Price == 0.0000M ? string.Empty : _priceFormatter.FormatPrice(sci.Product.Price);
            cartItemModel.OldPrice = sci.Product.OldPrice == 0.0000M ? string.Empty : _priceFormatter.FormatPrice(sci.Product.OldPrice);
            //Added by Sunil Kumar at 28-02-19
            cartItemModel.DiscountOfferName = string.Empty;
            if (sci.Product.HasDiscountsApplied)
            {
                var discount = _discountService.GetDiscountPercentageAndName(sci.Product);
                cartItemModel.DiscountPercentage = discount.Item1;
                cartItemModel.DiscountOfferName = discount.Item2;
            }
            else
            {
                cartItemModel.DiscountOfferName = string.Empty;
            }

            //picture
            if (_shoppingCartSettings.ShowProductImagesOnWishList)
            {
                cartItemModel.Picture = PrepareCartItemPictureModel(sci,
                    _mediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName);
            }

            //item warnings
            var itemWarnings = _shoppingCartService.GetShoppingCartItemWarnings(
                _workContext.CurrentCustomer,
                sci.ShoppingCartType,
                sci.Product,
                sci.StoreId,
                sci.AttributesXml,
                sci.CustomerEnteredPrice,
                sci.RentalStartDateUtc,
                sci.RentalEndDateUtc,
                sci.Quantity,
                false);
            foreach (var warning in itemWarnings.ErrorList)
                cartItemModel.Warnings.Add(warning);

            return cartItemModel;
        }

        /// <summary>
        /// Prepare the order review data model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <returns>Order review data model</returns>
        protected virtual ShoppingCartResponseModel.OrderReviewDataModel PrepareOrderReviewDataModel(IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            var model = new ShoppingCartResponseModel.OrderReviewDataModel();
            model.Display = true;

            //billing info
            var billingAddress = _workContext.CurrentCustomer.BillingAddress;
            if (billingAddress != null)
            {
                _addressModelFactoryApi.PrepareAddressModel(model.BillingAddress,
                        address: billingAddress,
                        excludeProperties: false,
                        addressSettings: _addressSettings);
            }


            //shipping info
            if (_shoppingCartService.ShoppingCartRequiresShipping(cart))
            {
                var shippingAddress = _addressService.GetAddressById(_genericAttributeService.GetAttribute<int>(_workContext.CurrentCustomer, NopCustomerDefaults.PickupPointAddressId, _storeContext.CurrentStore.Id));
                if (shippingAddress != null)
                {
                    shippingAddress.PhoneNumber = billingAddress.PhoneNumber;
                }
                model.IsShippable = true;
                var pickupPoint = _genericAttributeService.GetAttribute<PickupPoint>(_workContext.CurrentCustomer,
                    NopCustomerDefaults.SelectedPickupPointAttribute, _storeContext.CurrentStore.Id);
                model.SelectedPickUpInStore = _shippingSettings.AllowPickUpInStore && pickupPoint != null;
                if (!model.SelectedPickUpInStore)
                {
                    if (_workContext.CurrentCustomer.ShippingAddress != null || billingAddress != null)
                    {
                        _addressModelFactoryApi.PrepareAddressModel(model.ShippingAddress,
                                //address: _workContext.CurrentCustomer.ShippingAddress,
                                address: shippingAddress != null ? shippingAddress : billingAddress,
                                excludeProperties: false,
                                addressSettings: _addressSettings);
                    }
                }
                else
                {
                    var country = _countryService.GetCountryByTwoLetterIsoCode(pickupPoint.CountryCode);
                    model.PickupAddress = new AddressModel
                    {
                        Address1 = pickupPoint.Address,
                        CityId = pickupPoint.CityId,
                        CountryName = country != null ? country.Name : string.Empty,
                        ZipPostalCode = pickupPoint.ZipPostalCode
                    };
                }

                //selected shipping method
                var shippingOption = _genericAttributeService.GetAttribute<ShippingOption>(_workContext.CurrentCustomer,
                    NopCustomerDefaults.SelectedShippingOptionAttribute, _storeContext.CurrentStore.Id);
                if (shippingOption != null)
                    model.ShippingMethod = shippingOption.Name;
            }

            //payment info
            var selectedPaymentMethodSystemName = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer, NopCustomerDefaults.SelectedPaymentMethodAttribute, _storeContext.CurrentStore.Id);
            var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(selectedPaymentMethodSystemName);
            model.PaymentMethod = paymentMethod != null
                ? _localizationService.GetLocalizedFriendlyName(paymentMethod, _workContext.WorkingLanguage.Id)
                : "";

            return model;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the cart item picture model
        /// </summary>
        /// <param name="sci">Shopping cart item</param>
        /// <param name="pictureSize">Picture size</param>
        /// <param name="showDefaultPicture">Whether to show the default picture</param>
        /// <param name="productName">Product name</param>
        /// <returns>Picture model</returns>
        public virtual PictureModel PrepareCartItemPictureModel(ShoppingCartItem sci, int pictureSize, bool showDefaultPicture, string productName)
        {
            var pictureCacheKey = string.Format(ModelCacheEventConsumer.CART_PICTURE_MODEL_KEY, sci.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
            var model = _cacheManager.Get(pictureCacheKey,
                //as we cache per user (shopping cart item identifier)
                //let's cache just for 3 minutes
                () =>
                {
                    //shopping cart item picture
                    var sciPicture = _pictureService.GetProductPicture(sci.Product, sci.AttributesXml);
                    return new PictureModel
                    {
                        ImageUrl = _pictureService.GetPictureUrl(sciPicture, pictureSize, showDefaultPicture),
                        Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), productName),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), productName),
                    };
                });
            return model;
        }

        /// <summary>
        /// Prepare the shopping cart model
        /// </summary>
        /// <param name="model">Shopping cart model</param>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="isEditable">Whether model is editable</param>
        /// <param name="validateCheckoutAttributes">Whether to validate checkout attributes</param>
        /// <param name="prepareEstimateShippingIfEnabled">Whether to prepare estimate shipping model</param>
        /// <param name="setEstimateShippingDefaultAddress">Whether to use customer default shipping address for estimating</param>
        /// <param name="prepareAndDisplayOrderReviewData">Whether to prepare and display order review data</param>
        /// <returns>Shopping cart model</returns>
        public virtual ShoppingCartResponseModel PrepareShoppingCartModel(ShoppingCartResponseModel model,
            IList<ShoppingCartItem> cart, bool isEditable = true,
            bool validateCheckoutAttributes = false,
            bool prepareEstimateShippingIfEnabled = true, bool setEstimateShippingDefaultAddress = true,
            bool prepareAndDisplayOrderReviewData = false)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (model == null)
                throw new ArgumentNullException("model");

           // discount and gift card boxes
            model.DiscountBox.Display = _shoppingCartSettings.ShowDiscountBox;
            var discountCouponCodes = _customerService.ParseAppliedDiscountCouponCodes(_workContext.CurrentCustomer); //_workContext.CurrentCustomer.ParseAppliedDiscountCouponCodes();
            foreach (var couponCode in discountCouponCodes)
            {
                var discount = _discountService.GetAllDiscountsForCaching(couponCode: couponCode)
                    .FirstOrDefault(d => d.RequiresCouponCode && _discountService.ValidateDiscount(d, _workContext.CurrentCustomer).IsValid);

                if (discount != null)
                {
                    model.DiscountBox.AppliedDiscountsWithCodes.Add(new ShoppingCartResponseModel.DiscountBoxModel.DiscountInfoModel()
                    {
                        Id = discount.Id,
                        CouponCode = discount.CouponCode
                    });
                }
            }

            //model.GiftCardBox.Display = _shoppingCartSettings.ShowGiftCardBox;
            ////checkout attributes
            //var checkoutAttributesXml = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer,
            //    NopCustomerDefaults.CheckoutAttributes, _storeContext.CurrentStore.Id);
            //model.CheckoutAttributeInfo = _checkoutAttributeFormatter.FormatAttributes(checkoutAttributesXml, _workContext.CurrentCustomer);

            //model.CheckoutAttributes = PrepareCheckoutAttributeModels(cart);

            //cart items
            foreach (var sci in cart)
            {
                //Added By Sunil Kumar At 2/1/19 for Removing Unpublished items from Cart
                // Updated by Alexandar Rajavel on 05-Feb-2019
                var cartItemModel = PrepareShoppingCartItemModel(cart, sci);
                //Added By Sunil Kumar At 13/01/2020 for Added Weight And Shipping Charge Based On Weight
                var productWeight = (sci.Product.Weight) * (sci.Quantity);
                cartItemModel.Weight = Convert.ToString(productWeight) + " KG";
                var shippingChargeBasedonWeight = string.Empty;
                //Added By Sunil Kumar At 25/02/2020 for Added Code For Delivery Charge and KG Charge
                //var kgCharge = 0M;
                //var deliveryCharge = 0M;

                //var city = cart[0].Customer.BillingAddress == null ? null : cart[0].Customer.BillingAddress.City;
                //if (city != null)
                //{
                //    switch (decimal.Ceiling(sci.Product.Weight))
                //    {
                //        case 0:
                //        case 1:
                //            shippingChargeBasedonWeight += _shippingSettings.IsShippingChargeQtyWise ? (city.ShippingChargeUptoOneKg ?? 0M) * sci.Quantity : (city.ShippingChargeUptoOneKg ?? 0M);
                //            break;
                //        case 2:
                //            shippingChargeBasedonWeight += _shippingSettings.IsShippingChargeQtyWise ? (city.ShippingChargeUptoTwoKg ?? 0M) * sci.Quantity : (city.ShippingChargeUptoTwoKg ?? 0M);
                //            break;
                //        case 3:
                //            shippingChargeBasedonWeight += _shippingSettings.IsShippingChargeQtyWise ? (city.ShippingChargeUptoThreeKg ?? 0M) * sci.Quantity : (city.ShippingChargeUptoThreeKg ?? 0M);
                //            break;
                //        case 4:
                //            shippingChargeBasedonWeight += _shippingSettings.IsShippingChargeQtyWise ? (city.ShippingChargeUptoFourKg ?? 0M) * sci.Quantity : (city.ShippingChargeUptoFourKg ?? 0M);
                //            break;
                //        case 5:
                //            shippingChargeBasedonWeight += _shippingSettings.IsShippingChargeQtyWise ? (city.ShippingChargeUptoFiveKg ?? 0M) * sci.Quantity : (city.ShippingChargeUptoFiveKg ?? 0M);
                //            break;
                //        default:
                //            shippingChargeBasedonWeight += _shippingSettings.IsShippingChargeQtyWise ? ((city.ShippingChargeUptoFiveKg ?? 0M) + city.ShippingChargeAboveFiveKg ?? 0M) * sci.Quantity : ((city.ShippingChargeUptoFiveKg ?? 0M) + city.ShippingChargeAboveFiveKg ?? 0M);
                //            //kgCharge = (decimal)city.ShippingChargeAboveFiveKg;
                //            //deliveryCharge = (decimal)city.ShippingChargeUptoFiveKg;
                //            break;
                //    }


                //}
                shippingChargeBasedonWeight = shippingChargeBasedonWeight == string.Empty ? "0" : shippingChargeBasedonWeight;
                cartItemModel.ShippingCharge = _priceFormatter.FormatPrice(Convert.ToDecimal(shippingChargeBasedonWeight), true, false);
                //cartItemModel.KgCharge = _priceFormatter.FormatPrice(Convert.ToDecimal(kgCharge), true, false);
                //cartItemModel.DeliveryCharge = _priceFormatter.FormatPrice(Convert.ToDecimal(deliveryCharge), true, false);
                if (sci.Product.Published)
                {
                    var stockQuantity = 0;
                    if (sci.Product.ProductAttributeCombinations.Any())
                    {
                        var combinations = _productAttributeParser.FindProductAttributeCombination(sci.Product, sci.AttributesXml);
                        stockQuantity = combinations == null ? 0 : combinations.StockQuantity;
                    }
                    cartItemModel.AvailableQuantity = stockQuantity > 0 ? stockQuantity : sci.Product.StockQuantity;
                    //added by Sunil at 28-05-2020
                    cartItemModel.MaxItemQuantity = 99;
                    model.Items.Add(cartItemModel);
                }
            }

            //order review data
            if (prepareAndDisplayOrderReviewData)
            {
                model.OrderReviewData = PrepareOrderReviewDataModel(cart);
            }
            //model.Count = model.Items.Count;
            //Added by Alexandar Rajavel on 10-Jan-2019
            model.CanContinue = model.Items.Where(x => x.Warnings.Any()).Any() ? false : true;
            return model;
        }

        /// <summary>
        /// Prepare the wishlist model
        /// </summary>
        /// <param name="model">Wishlist model</param>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="isEditable">Whether model is editable</param>
        /// <returns>Wishlist model</returns>
        public virtual WishlistResponseModel PrepareWishlistModel(WishlistResponseModel model, IList<ShoppingCartItem> cart, bool isEditable = true)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (model == null)
                throw new ArgumentNullException("model");

            model.EmailWishlistEnabled = _shoppingCartSettings.EmailWishlistEnabled;
            model.IsEditable = isEditable;
            model.DisplayAddToCart = _permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart);
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoWishlist;

            if (!cart.Any())
                return model;

            //simple properties
            var customer = cart.FirstOrDefault(item => item.Customer != null)?.Customer;
            model.CustomerGuid = customer.CustomerGuid;
            model.CustomerFullname = _customerService.GetCustomerFullName(customer);
            model.ShowProductImages = _shoppingCartSettings.ShowProductImagesOnWishList;
            model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;

            //cart warnings
            var cartWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, "", false);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);

            //cart items
            foreach (var sci in cart)
            {
                var cartItemModel = PrepareWishlistItemModel(cart, sci);
                model.Items.Add(cartItemModel);
            }

            return model;
        }


        /// <summary>
        /// Prepare the order totals model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="isEditable">Whether model is editable</param>
        /// <returns>Order totals model</returns>
        public virtual OrderTotalsResponseModel PrepareOrderTotalsModel(IList<ShoppingCartItem> cart, bool isEditable)
        {
            var model = new OrderTotalsResponseModel();
            model.IsEditable = isEditable;

            if (cart.Any())
            {
                //subtotal
                decimal orderSubTotalDiscountAmountBase;
                List<DiscountForCaching> orderSubTotalAppliedDiscounts;
                decimal subTotalWithoutDiscountBase;
                decimal subTotalWithDiscountBase;
                var subTotalIncludingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
                _orderTotalCalculationService.GetShoppingCartSubTotal(cart, subTotalIncludingTax,
                    out orderSubTotalDiscountAmountBase, out orderSubTotalAppliedDiscounts,
                    out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
                decimal subtotalBase = subTotalWithoutDiscountBase;
                decimal subtotal = _currencyService.ConvertFromPrimaryStoreCurrency(subtotalBase, _workContext.WorkingCurrency);
                model.SubTotal = _priceFormatter.FormatPrice(subtotal, true, _workContext.WorkingCurrency, _workContext.WorkingLanguage, subTotalIncludingTax);

                if (orderSubTotalDiscountAmountBase > decimal.Zero)
                {
                    decimal orderSubTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderSubTotalDiscountAmountBase, _workContext.WorkingCurrency);
                    model.SubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountAmount, true, _workContext.WorkingCurrency, _workContext.WorkingLanguage, subTotalIncludingTax);
                }


                //shipping info
                //model.RequiresShipping = _shoppingCartService.ShoppingCartRequiresShipping(cart);
                model.RequiresShipping = true;
                if (model.RequiresShipping)
                {
                    decimal? shoppingCartShippingBase = decimal.Zero;
                    //decimal? shoppingCartShippingBase = _orderTotalCalculationService.GetShoppingCartShippingTotal(cart);
                    if (shoppingCartShippingBase.HasValue)
                    {
                        decimal shoppingCartShipping = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartShippingBase.Value, _workContext.WorkingCurrency);
                        model.Shipping = _priceFormatter.FormatShippingPrice(shoppingCartShipping, true);


                        //selected shipping method
                        var shippingOption = _genericAttributeService.GetAttribute<ShippingOption>(_workContext.CurrentCustomer, NopCustomerDefaults.SelectedShippingOptionAttribute, _storeContext.CurrentStore.Id);
                        if (shippingOption != null)
                            model.SelectedShippingMethod = shippingOption.Name;
                    }
                }
                else
                {
                    model.HideShippingTotal = _shippingSettings.HideShippingTotal;
                }

                //payment method fee
                //var paymentMethodSystemName = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer,
                //    NopCustomerDefaults.SelectedPaymentMethodAttribute, _storeContext.CurrentStore.Id);
                //decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, paymentMethodSystemName);
                //decimal paymentMethodAdditionalFeeWithTaxBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, _workContext.CurrentCustomer);
                //if (paymentMethodAdditionalFeeWithTaxBase > decimal.Zero)
                //{
                //    decimal paymentMethodAdditionalFeeWithTax = _currencyService.ConvertFromPrimaryStoreCurrency(paymentMethodAdditionalFeeWithTaxBase, _workContext.WorkingCurrency);
                //    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeWithTax, true);
                //}

        


                //model.PaymentMethodAdditionalFee =
                //tax
                bool displayTax = true;
                bool displayTaxRates = true;
                if (_taxSettings.HideTaxInOrderSummary && _workContext.TaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    SortedDictionary<decimal, decimal> taxRates;
                    decimal shoppingCartTaxBase = _orderTotalCalculationService.GetTaxTotal(cart, out taxRates);
                    //decimal shoppingCartTax = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartTaxBase, _workContext.WorkingCurrency);

                    //decimal shoppingCartTaxBase = 0;
                    //decimal shoppingCartTax = 0;

                    if (shoppingCartTaxBase == 0 && _taxSettings.HideZeroTax)
                    {
                        displayTax = false;
                        displayTaxRates = false;
                    }
                    else
                    {
                        //displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                        //displayTax = !displayTaxRates;


                        displayTaxRates = false;
                        displayTax = true;


                        model.Tax = "0 MMK";
                        //model.Tax = _priceFormatter.FormatPrice(shoppingCartTax, true, false);
                        foreach (var tr in taxRates)
                        {
                            model.TaxRates.Add(new OrderTotalsResponseModel.TaxRate
                            {
                                Rate = _priceFormatter.FormatTaxRate(tr.Key),
                                Value = _priceFormatter.FormatPrice(_currencyService.ConvertFromPrimaryStoreCurrency(tr.Value, _workContext.WorkingCurrency), true, false),
                            });
                        }
                    }
                }
                model.DisplayTaxRates = displayTaxRates;
                model.DisplayTax = displayTax;

                //total
                //commented on 04-05-2020 by Sunil Kumar
                model.MinOrderTotalAmount = _orderSettings.MinOrderTotalAmount;

                //var cities = _cityService.GetCities();
                //var singleCartDetails = cart.FirstOrDefault();
                //var cityId = singleCartDetails.Customer.ShippingAddress != null ? singleCartDetails.Customer.ShippingAddress.CityId : singleCartDetails.Customer.BillingAddress != null ? singleCartDetails.Customer.BillingAddress.CityId : 0;
                //var cityValue = cities.FirstOrDefault(o => o.Id == cityId);
                //var minOrderValue = cityValue != null ? cityValue.MinOrderValue : _orderSettings.MinOrderTotalAmount;


                //Added Code at 25-05-2020 for Shopping Cart Bug by Sunil Kumar
                //model.MinOrderTotalAmount = (decimal)minOrderValue;

                decimal orderTotalDiscountAmountBase;
                List<DiscountForCaching> orderTotalAppliedDiscounts;
                List<AppliedGiftCard> appliedGiftCards;
                int redeemedRewardPoints;
                decimal redeemedRewardPointsAmount;
                decimal? shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotal(cart,
                    out orderTotalDiscountAmountBase, out orderTotalAppliedDiscounts,
                    out appliedGiftCards, out redeemedRewardPoints, out redeemedRewardPointsAmount);
                if (shoppingCartTotalBase.HasValue)
                {
                    decimal shoppingCartTotal = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartTotalBase.Value, _workContext.WorkingCurrency);

                    var paymentMethodSystemName = string.Empty;
                    if (_workContext.CurrentCustomer != null)
                    {
                        paymentMethodSystemName = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer,
                            NopCustomerDefaults.SelectedPaymentMethodAttribute, _storeContext.CurrentStore.Id);
                    }

                    if (paymentMethodSystemName != string.Empty && paymentMethodSystemName == "Payments.2C2P")
                    {
                        var orderTotal = Convert.ToDecimal(shoppingCartTotal);
                        var mdrvalue = (orderTotal * (decimal)0.048) < 500 ? 500 : (orderTotal * (decimal)0.048);
                        mdrvalue = Math.Round(mdrvalue);
                        shoppingCartTotal = shoppingCartTotal + mdrvalue;
                        model.PaymentMethodAdditionalFee = _priceFormatter.FormatPrice(mdrvalue, true, false);

                        model.OrderTotal = _priceFormatter.FormatPrice(shoppingCartTotal, true, false);
                        model.OrderTotalInt = Convert.ToInt32(Math.Floor(shoppingCartTotal));//Added by Alexandar Rajavel on 02-Feb-2019


                    }
                    else
                    {
                        model.OrderTotal = _priceFormatter.FormatPrice(shoppingCartTotal, true, false);
                        model.OrderTotalInt = Convert.ToInt32(Math.Floor(shoppingCartTotal));//Added by Alexandar Rajavel on 02-Feb-2019
                    }


                    
                }

                //discount
                if (orderTotalDiscountAmountBase > decimal.Zero)
                {
                    decimal orderTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderTotalDiscountAmountBase, _workContext.WorkingCurrency);
                    model.OrderTotalDiscount = _priceFormatter.FormatPrice(-orderTotalDiscountAmount, true, false);
                }


               


                //gift cards
                //if (appliedGiftCards != null && appliedGiftCards.Any())
                //{
                //    foreach (var appliedGiftCard in appliedGiftCards)
                //    {
                //        var gcModel = new OrderTotalsResponseModel.GiftCard
                //        {
                //            Id = appliedGiftCard.GiftCard.Id,
                //            CouponCode = appliedGiftCard.GiftCard.GiftCardCouponCode,
                //        };
                //        decimal amountCanBeUsed = _currencyService.ConvertFromPrimaryStoreCurrency(appliedGiftCard.AmountCanBeUsed, _workContext.WorkingCurrency);
                //        gcModel.Amount = _priceFormatter.FormatPrice(-amountCanBeUsed, true, false);

                    //        decimal remainingAmountBase = _giftCardService.GetGiftCardRemainingAmount(appliedGiftCard.GiftCard) - appliedGiftCard.AmountCanBeUsed;
                    //        decimal remainingAmount = _currencyService.ConvertFromPrimaryStoreCurrency(remainingAmountBase, _workContext.WorkingCurrency);
                    //        gcModel.Remaining = _priceFormatter.FormatPrice(remainingAmount, true, false);

                    //        model.GiftCards.Add(gcModel);
                    //    }
                    //}

                    //reward points to be spent (redeemed)
                    //if (redeemedRewardPointsAmount > decimal.Zero)
                    //{
                    //    decimal redeemedRewardPointsAmountInCustomerCurrency = _currencyService.ConvertFromPrimaryStoreCurrency(redeemedRewardPointsAmount, _workContext.WorkingCurrency);
                    //    model.RedeemedRewardPoints = redeemedRewardPoints;
                    //    model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-redeemedRewardPointsAmountInCustomerCurrency, true, false);
                    //}

                    ////reward points to be earned
                    //if (_rewardPointsSettings.Enabled &&
                    //    _rewardPointsSettings.DisplayHowMuchWillBeEarned &&
                    //    shoppingCartTotalBase.HasValue)
                    //{
                    //    decimal? shippingBaseInclTax = model.RequiresShipping
                    //        ? _orderTotalCalculationService.GetShoppingCartShippingTotal(cart, true)
                    //        : 0;
                    //    if (shippingBaseInclTax.HasValue)
                    //    {
                    //        var totalForRewardPoints = _orderTotalCalculationService.CalculateApplicableOrderTotalForRewardPoints(shippingBaseInclTax.Value, shoppingCartTotalBase.Value);
                    //        model.WillEarnRewardPoints = _orderTotalCalculationService.CalculateRewardPoints(_workContext.CurrentCustomer, totalForRewardPoints);
                    //    }
                    //}

            }
            return model;
        }


        public virtual DiscountsResponseModel PrepareExistingDiscountsModel()
        {
            var model = new DiscountsResponseModel();

            //get applied coupon codes
            var existingCouponCodes = _customerService.ParseAppliedDiscountCouponCodes(_workContext.CurrentCustomer);

            foreach (var couponCode in existingCouponCodes)
            {
                var discount = _discountService.GetAllDiscountsForCaching(couponCode: couponCode, showHidden: true)
                    .Where(d => d.RequiresCouponCode)
                    .ToList().
                    FirstOrDefault();

                if (discount != null)
                {
                    var discountModel = new DiscountModel
                    {
                        Id = discount.Id,
                        CouponCode = discount.CouponCode
                    };
                    model.DiscountModels.Add(discountModel);
                }
            }

            return model;
        }

        #endregion
    }
}

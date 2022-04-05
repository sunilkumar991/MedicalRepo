using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Factories;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.ShoppingCart;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Core.Domain;
using Nop.Plugin.Payments.OKDollar;
using Nop.Services.Configuration;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class ShoppingCartController : BaseApiController
    {
        #region Field

        private readonly IShoppingCartModelFactoryApi _shoppingCartModelFactoryApi;
        private readonly IProductService _productService;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICurrencyService _currencyService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IDownloadService _downloadService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ITaxService _taxService;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly IPermissionService _permissionService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IPictureService _pictureService;
        private readonly IWebHelper _webHelper;
        private readonly ICacheManager _cacheManager;
        private readonly MediaSettings _mediaSettings;
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private readonly IGiftCardService _giftCardService;
        private readonly IDiscountService _discountService;
        //private readonly GenericAttributeServiceApi _genericAttributeServiceApi;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPaymentService _paymentService;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly AddressSettings _addressSettings;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly ShippingSettings _shippingSettings;
        private readonly ISettingService _settingService;
        #endregion

        #region Ctor
        public ShoppingCartController(IShoppingCartModelFactoryApi shoppingCartModelFactoryApi,
            IProductService productService,
            ShoppingCartSettings shoppingCartSettings,
            IWorkContext workContext,
            IStoreContext storeContext,
            ICurrencyService currencyService,
            IShoppingCartService shoppingCartService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IProductAttributeService productAttributeService,
            IProductAttributeParser productAttributeParser,
            IDownloadService downloadService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            ITaxService taxService,
            ICheckoutAttributeService checkoutAttributeService,
            IPermissionService permissionService,
            ICheckoutAttributeParser checkoutAttributeParser,
            IGenericAttributeService genericAttributeService,
            IProductAttributeFormatter productAttributeFormatter,
            IPictureService pictureService,
            IWebHelper webHelper,
             ICacheManager cacheManager,
            MediaSettings mediaSettings,
            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            IGiftCardService giftCardService,
            IDiscountService discountService,
            IOrderTotalCalculationService orderTotalCalculationService,
            //GenericAttributeServiceApi genericAttributeServiceApi,
            TaxSettings taxSettings,
            IPaymentService paymentService,
            RewardPointsSettings rewardPointsSettings,
            CatalogSettings catalogSettings,
            AddressSettings addressSettings,
            IAddressAttributeFormatter addressAttributeFormatter,
            ShippingSettings shippingSettings,
            ISettingService settingService
            )
        {
            this._shoppingCartModelFactoryApi = shoppingCartModelFactoryApi;
            this._productService = productService;
            this._shoppingCartSettings = shoppingCartSettings;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._currencyService = currencyService;
            this._shoppingCartService = shoppingCartService;
            this._customerService = customerService;
            this._localizationService = localizationService;
            this._productAttributeService = productAttributeService;
            this._downloadService = downloadService;
            this._productAttributeParser = productAttributeParser;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._taxService = taxService;
            this._checkoutAttributeService = checkoutAttributeService;
            this._permissionService = permissionService;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._genericAttributeService = genericAttributeService;
            this._productAttributeFormatter = productAttributeFormatter;
            this._pictureService = pictureService;
            this._webHelper = webHelper;
            this._cacheManager = cacheManager;
            this._mediaSettings = mediaSettings;
            this._checkoutAttributeFormatter = checkoutAttributeFormatter;
            this._giftCardService = giftCardService;
            this._discountService = discountService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._taxSettings = taxSettings;
            this._paymentService = paymentService;
            this._rewardPointsSettings = rewardPointsSettings;
            this._catalogSettings = catalogSettings;
            this._shippingSettings = shippingSettings;
            this._addressSettings = addressSettings;
            this._addressAttributeFormatter = addressAttributeFormatter;
            //this._genericAttributeServiceApi = genericAttributeServiceApi;
            _settingService = settingService;
        }
        #endregion

        #region Utility

        /// <summary>
        /// Gets the productAttributeValue by from suupplied keys.
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="form">Form Data</param>
        /// <returns>Product attribute value</returns>
        private ProductAttributeValue ParseValueAndGetProductAttributeValue(Product product, NameValueCollection form)
        {
            var productAttributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            foreach (var attribute in productAttributes)
            {
                string measurementId = string.Format("product_measurement_{0}_{1}_{2}", attribute.ProductId, attribute.ProductAttributeId, attribute.Id);
                var productAttributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                var measurementKey = form[measurementId];
                if (!string.IsNullOrWhiteSpace(measurementKey))
                {
                    //var productAttributeValue = productAttributeValues.SingleOrDefault(item => item.Name == measurementKey);
                    var productAttributeValue = productAttributeValues.SingleOrDefault(item => item.Id == Convert.ToInt32(measurementKey));
                    if (productAttributeValue != null)
                        return productAttributeValue;
                }
            }
            return null;
        }


        /// <summary>
        /// Parse product attributes on the product details page
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="form">Form</param>
        /// <returns>Parsed attributes</returns>
        [NonAction]
        protected virtual string ParseProductAttributes(Product product, NameValueCollection form)
        {
            string attributesXml = "";

            #region Product attributes
            var productAttributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            foreach (var attribute in productAttributes)
            {
                string controlId = string.Format("product_attribute_{0}_{1}_{2}", attribute.ProductId, attribute.ProductAttributeId, attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                foreach (var item in ctrlAttributes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                string enteredText = ctrlAttributes.Trim();
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var day = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));
                            }
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            Guid downloadGuid;
                            Guid.TryParse(form[controlId], out downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, download.DownloadGuid.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            #endregion

            #region Gift cards

            if (product.IsGiftCard)
            {
                string recipientName = "";
                string recipientEmail = "";
                string senderName = "";
                string senderEmail = "";
                string giftCardMessage = "";
                foreach (string formKey in form.AllKeys)
                {
                    if (formKey.Equals(string.Format("giftcard_{0}.RecipientName", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.RecipientEmail", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.SenderName", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.SenderEmail", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.Message", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        giftCardMessage = form[formKey];
                        continue;
                    }
                }

                attributesXml = _productAttributeParser.AddGiftCardAttribute(attributesXml,
                    recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);
            }

            #endregion

            return attributesXml;
        }

        [NonAction]
        protected virtual void ParseAndSaveCheckoutAttributes(List<ShoppingCartItem> cart, NameValueCollection form)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (form == null)
                throw new ArgumentNullException("form");

            string attributesXml = "";
            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id,
                !_shoppingCartService.ShoppingCartRequiresShipping(cart));
            foreach (var attribute in checkoutAttributes)
            {
                string controlId = string.Format("checkout_attribute_{0}", attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _checkoutAttributeService.GetCheckoutAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                string enteredText = ctrlAttributes.Trim();
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var date = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(date));
                            }
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            Guid downloadGuid;
                            Guid.TryParse(form[controlId], out downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml,
                                           attribute, download.DownloadGuid.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            //save checkout attributes
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, NopCustomerDefaults.CheckoutAttributes, attributesXml, _storeContext.CurrentStore.Id);
        }

        /// <summary>
        /// Parse product rental dates on the product details page
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="form">Form</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        [NonAction]
        protected virtual void ParseRentalDates(Product product, NameValueCollection form,
            out DateTime? startDate, out DateTime? endDate)
        {
            startDate = null;
            endDate = null;

            string startControlId = string.Format("rental_start_date_{0}", product.Id);
            string endControlId = string.Format("rental_end_date_{0}", product.Id);
            var ctrlStartDate = form[startControlId];
            var ctrlEndDate = form[endControlId];
            try
            {
                //currenly we support only this format (as in the \Views\Product\_RentalInfo.cshtml file)
                const string datePickerFormat = "MM/dd/yyyy";
                startDate = DateTime.ParseExact(ctrlStartDate, datePickerFormat, CultureInfo.InvariantCulture);
                endDate = DateTime.ParseExact(ctrlEndDate, datePickerFormat, CultureInfo.InvariantCulture);
            }
            catch
            {
            }
        }





        #endregion

        #region Action Method

        [Route("api/AddProductToCart/{productId}/{shoppingCartTypeId}")]
        [HttpPost]
        public IActionResult AddProductToCart_Details(int productId, int shoppingCartTypeId, [FromBody]List<KeyValueApi> formValues)
        {
            int count = 1;
            var form = new NameValueCollection();
            foreach (var values in formValues)
            {
                form.Add(values.Key, values.Value);
            }
            var errrorList = new List<string>();
            var result = new AddProductToCartResponseModel();
            var product = _productService.GetProductById(productId);
            if (product == null)
            {
                errrorList.Add("Product Not Found");
                result.ErrorList = errrorList;
                result.Success = false;
                result.StatusCode = (int)ErrorType.NotOk;
            }

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
            {
                errrorList.Add("Only simple products could be added to the cart");
                result.ErrorList = errrorList;
                result.Success = false;
                result.StatusCode = (int)ErrorType.NotOk;
            }

            #region Update existing shopping cart item?
            int updatecartitemid = 0;

            foreach (string formKey in form.AllKeys)
                if (formKey.Equals(string.Format("addtocart_{0}.UpdatedShoppingCartItemId", productId), StringComparison.InvariantCultureIgnoreCase))
                {
                    int.TryParse(form[formKey], out updatecartitemid);
                    break;
                }
            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found?
                if (updatecartitem == null)
                {
                    errrorList.Add("No shopping cart item found to update");
                    result.ErrorList = errrorList;
                    result.Success = false;
                    result.StatusCode = (int)ErrorType.NotOk;
                }
                //is it this product?
                if (product.Id != updatecartitem.ProductId)
                {
                    errrorList.Add("This product does not match a passed shopping cart item identifier");
                    result.ErrorList = errrorList;
                    result.Success = false;
                    result.StatusCode = (int)ErrorType.NotOk;
                }
            }
            #endregion

            #region Customer entered price
            decimal customerEnteredPriceConverted = decimal.Zero;
            if (product.CustomerEntersPrice)
            {
                foreach (string formKey in form.AllKeys)
                {
                    if (formKey.Equals(string.Format("addtocart_{0}.CustomerEnteredPrice", productId), StringComparison.InvariantCultureIgnoreCase))
                    {
                        decimal customerEnteredPrice;
                        if (decimal.TryParse(form[formKey], out customerEnteredPrice))
                            customerEnteredPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(customerEnteredPrice, _workContext.WorkingCurrency);
                        break;
                    }
                }
            }
            #endregion

            #region Quantity

            int quantity = 1;

            foreach (string formKey in form.AllKeys)
                if (formKey.Equals(string.Format("addtocart_{0}.EnteredQuantity", productId), StringComparison.InvariantCultureIgnoreCase))
                {
                    int.TryParse(form[formKey], out quantity);
                    break;
                }

            #endregion

            //product and gift card attributes
            string attributes = ParseProductAttributes(product, form);

            //rental attributes
            DateTime? rentalStartDate = null;
            DateTime? rentalEndDate = null;
            if (product.IsRental)
            {
                ParseRentalDates(product, form, out rentalStartDate, out rentalEndDate);
            }

            //save item
            var addToCartWarnings = new ErrorListModel();
            var cartType = (ShoppingCartType)shoppingCartTypeId;

            var Wishlist = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(x => x.ShoppingCartType == ShoppingCartType.Wishlist)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
            bool has = Wishlist.Any(cartItems => cartItems.ProductId == productId);

            var addtocart = _workContext.CurrentCustomer.ShoppingCartItems
                   .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
                   .LimitPerStore(_storeContext.CurrentStore.Id)
                   .ToList();
            bool AlreadyAddedInCart = addtocart.Any(cartItems => cartItems.ProductId == productId);
            if (updatecartitem == null)
            {
                //Added By Sunil Kumar at 13/02/19 some conditions for already added Item In addtocart when minimum quantity added as 2 
                if (shoppingCartTypeId == 1)
                {
                    if (AlreadyAddedInCart && quantity > 1)
                    {
                        //Commented on 22-05-2020 by Sunil Kumar for Removing Default Value(1) added same item more than once in cart 
                        //quantity = 1;
                    }
                    //add to the cart
                    addToCartWarnings = _shoppingCartService.AddToCart(_workContext.CurrentCustomer,
                        product, cartType, _storeContext.CurrentStore.Id,
                        attributes, customerEnteredPriceConverted,
                        rentalStartDate, rentalEndDate, quantity, true, count);
                }
                else if (shoppingCartTypeId == 2)
                {
                    if (has == false)
                    {
                        //add to the cart
                        addToCartWarnings = _shoppingCartService.AddToCart(_workContext.CurrentCustomer,
                            product, cartType, _storeContext.CurrentStore.Id,
                            attributes, customerEnteredPriceConverted,
                            rentalStartDate, rentalEndDate, quantity, true, count);

                    }
                    else
                    {
                        result.SuccessMessage = _localizationService.GetResource("Products.Wishlist.ProductAlreadyAdded");
                    }
                }

            }
            else
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                var otherCartItemWithSameParameters = _shoppingCartService.FindShoppingCartItemInTheCart(
                    cart, cartType, product, attributes, customerEnteredPriceConverted,
                    rentalStartDate, rentalEndDate);
                if (otherCartItemWithSameParameters != null &&
                    otherCartItemWithSameParameters.Id == updatecartitem.Id)
                {
                    //ensure it's other shopping cart cart item
                    otherCartItemWithSameParameters = null;
                }
                //Code Added By Sunil Kumar at 09/11/18 for already added Item In WishList
                if (shoppingCartTypeId == 1)
                {
                    //update existing item
                    addToCartWarnings.ErrorList = _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                        updatecartitem.Id, attributes, customerEnteredPriceConverted,
                        rentalStartDate, rentalEndDate, quantity, true);
                    if (otherCartItemWithSameParameters != null && addToCartWarnings.ErrorList.Count == 0)
                    {
                        //delete the same shopping cart item (the other one)
                        _shoppingCartService.DeleteShoppingCartItem(otherCartItemWithSameParameters);
                    }
                }
                if (shoppingCartTypeId == 2)
                {
                    if (!has)
                    {
                        //update existing item
                        addToCartWarnings.ErrorList = _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                            updatecartitem.Id, attributes, customerEnteredPriceConverted,
                            rentalStartDate, rentalEndDate, quantity, true);
                        if (otherCartItemWithSameParameters != null && addToCartWarnings.ErrorList.Count == 0)
                        {
                            //delete the same shopping cart item (the other one)
                            _shoppingCartService.DeleteShoppingCartItem(otherCartItemWithSameParameters);
                        }
                    }
                    else
                    {
                        result.SuccessMessage = _localizationService.GetResource("Products.Wishlist.ProductAlreadyAdded");
                    }
                }


            }

            #region Return result

            if (addToCartWarnings.ErrorList.Count > 0)
            {
                result.ErrorList = addToCartWarnings.ErrorList.ToList();
                result.IsAttributeError = addToCartWarnings.IsAttributeError;
                result.Success = false;
                result.StatusCode = (int)ErrorType.NotOk;
            }
            else
            {
                //added to the cart/wishlist
                switch (cartType)
                {
                    case ShoppingCartType.Wishlist:
                        {
                            //activity log
                            //_customerActivityService.InsertActivity("PublicStore.AddToWishlist", _localizationService.GetResource("ActivityLog.PublicStore.AddToWishlist"), product.Name);

                            if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct)
                            {
                                result.ForceRedirect = true;
                            }

                            result.Success = true;
                            break;
                        }
                    case ShoppingCartType.ShoppingCart:
                    default:
                        {
                            //activity log
                            // _customerActivityService.InsertActivity("PublicStore.AddToShoppingCart", _localizationService.GetResource("ActivityLog.PublicStore.AddToShoppingCart"), product.Name);
                            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                                .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
                                .LimitPerStore(_storeContext.CurrentStore.Id)
                                .ToList();
                            result.Count = _workContext.CurrentCustomer.ShoppingCartItems
                            .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                            .LimitPerStore(_storeContext.CurrentStore.Id)
                            .ToList()
                            .Sum(x => x.Quantity);
                            if (_shoppingCartSettings.DisplayCartAfterAddingProduct)
                            {
                                result.ForceRedirect = true;
                                result.Success = true;
                            }

                            result.Success = true;
                            break;
                        }
                }
            }
            return Ok(result);

            #endregion
        }

        [Route("api/AddProductToCart/{productId}/{shoppingCartTypeId}/{count}")]
        [HttpPost]
        public IActionResult AddProductToCart_Details(int productId, int shoppingCartTypeId, int count, [FromBody]List<KeyValueApi> formValues)
        {
            var form = new NameValueCollection();
            foreach (var values in formValues)
            {
                form.Add(values.Key, values.Value);
            }
            var errrorList = new List<string>();
            var result = new AddProductToCartResponseModel();
            var product = _productService.GetProductById(productId);
            if (product == null)
            {
                errrorList.Add("Product Not Found");
                result.ErrorList = errrorList;
                result.Success = false;
                result.StatusCode = (int)ErrorType.NotOk;
            }

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
            {
                errrorList.Add("Only simple products could be added to the cart");
                result.ErrorList = errrorList;
                result.Success = false;
                result.StatusCode = (int)ErrorType.NotOk;
            }

            #region Update existing shopping cart item?
            int updatecartitemid = 0;

            foreach (string formKey in form.AllKeys)
                if (formKey.Equals(string.Format("addtocart_{0}.UpdatedShoppingCartItemId", productId), StringComparison.InvariantCultureIgnoreCase))
                {
                    int.TryParse(form[formKey], out updatecartitemid);
                    break;
                }
            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found?
                if (updatecartitem == null)
                {
                    errrorList.Add("No shopping cart item found to update");
                    result.ErrorList = errrorList;
                    result.Success = false;
                    result.StatusCode = (int)ErrorType.NotOk;
                }
                //is it this product?
                if (product.Id != updatecartitem.ProductId)
                {
                    errrorList.Add("This product does not match a passed shopping cart item identifier");
                    result.ErrorList = errrorList;
                    result.Success = false;
                    result.StatusCode = (int)ErrorType.NotOk;
                }
            }
            #endregion

            #region Customer entered price
            decimal customerEnteredPriceConverted = decimal.Zero;
            if (product.CustomerEntersPrice)
            {
                foreach (string formKey in form.AllKeys)
                {
                    if (formKey.Equals(string.Format("addtocart_{0}.CustomerEnteredPrice", productId), StringComparison.InvariantCultureIgnoreCase))
                    {
                        decimal customerEnteredPrice;
                        if (decimal.TryParse(form[formKey], out customerEnteredPrice))
                            customerEnteredPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(customerEnteredPrice, _workContext.WorkingCurrency);
                        break;
                    }
                }
            }
            #endregion

            #region Quantity

            int quantity = 1;

            foreach (string formKey in form.AllKeys)
                if (formKey.Equals(string.Format("addtocart_{0}.EnteredQuantity", productId), StringComparison.InvariantCultureIgnoreCase))
                {
                    int.TryParse(form[formKey], out quantity);
                    break;
                }

            #endregion

            //product and gift card attributes
            string attributes = ParseProductAttributes(product, form);

            //rental attributes
            DateTime? rentalStartDate = null;
            DateTime? rentalEndDate = null;
            if (product.IsRental)
            {
                ParseRentalDates(product, form, out rentalStartDate, out rentalEndDate);
            }

            //save item
            var addToCartWarnings = new ErrorListModel();
            var cartType = (ShoppingCartType)shoppingCartTypeId;

            var Wishlist = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(x => x.ShoppingCartType == ShoppingCartType.Wishlist)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
            bool has = Wishlist.Any(cartItems => cartItems.ProductId == productId);

            var addtocart = _workContext.CurrentCustomer.ShoppingCartItems
                   .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
                   .LimitPerStore(_storeContext.CurrentStore.Id)
                   .ToList();
            bool AlreadyAddedInCart = addtocart.Any(cartItems => cartItems.ProductId == productId);
            if (updatecartitem == null)
            {
                //Added By Sunil Kumar at 13/02/19 some conditions for already added Item In addtocart when minimum quantity added as 2 
                if (shoppingCartTypeId == 1)
                {
                    if (AlreadyAddedInCart && quantity > 1)
                    {
                        //Commented on 22-05-2020 by Sunil Kumar for Removing Default Value(1) added same item more than once in cart 
                        //quantity = 1;
                    }
                    //add to the cart
                    addToCartWarnings = _shoppingCartService.AddToCart(_workContext.CurrentCustomer,
                        product, cartType, _storeContext.CurrentStore.Id,
                        attributes, customerEnteredPriceConverted,
                        rentalStartDate, rentalEndDate, quantity, true, count);
                }
                else if (shoppingCartTypeId == 2)
                {
                    if (has == false)
                    {
                        //add to the cart
                        addToCartWarnings = _shoppingCartService.AddToCart(_workContext.CurrentCustomer,
                            product, cartType, _storeContext.CurrentStore.Id,
                            attributes, customerEnteredPriceConverted,
                            rentalStartDate, rentalEndDate, quantity, true, count);

                    }
                    else
                    {
                        result.SuccessMessage = _localizationService.GetResource("Products.Wishlist.ProductAlreadyAdded");
                    }
                }

            }
            else
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                var otherCartItemWithSameParameters = _shoppingCartService.FindShoppingCartItemInTheCart(
                    cart, cartType, product, attributes, customerEnteredPriceConverted,
                    rentalStartDate, rentalEndDate);
                if (otherCartItemWithSameParameters != null &&
                    otherCartItemWithSameParameters.Id == updatecartitem.Id)
                {
                    //ensure it's other shopping cart cart item
                    otherCartItemWithSameParameters = null;
                }
                //Code Added By Sunil Kumar at 09/11/18 for already added Item In WishList
                if (shoppingCartTypeId == 1)
                {
                    //update existing item
                    addToCartWarnings.ErrorList = _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                        updatecartitem.Id, attributes, customerEnteredPriceConverted,
                        rentalStartDate, rentalEndDate, quantity, true);
                    if (otherCartItemWithSameParameters != null && addToCartWarnings.ErrorList.Count == 0)
                    {
                        //delete the same shopping cart item (the other one)
                        _shoppingCartService.DeleteShoppingCartItem(otherCartItemWithSameParameters);
                    }
                }
                if (shoppingCartTypeId == 2)
                {
                    if (!has)
                    {
                        //update existing item
                        addToCartWarnings.ErrorList = _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                            updatecartitem.Id, attributes, customerEnteredPriceConverted,
                            rentalStartDate, rentalEndDate, quantity, true);
                        if (otherCartItemWithSameParameters != null && addToCartWarnings.ErrorList.Count == 0)
                        {
                            //delete the same shopping cart item (the other one)
                            _shoppingCartService.DeleteShoppingCartItem(otherCartItemWithSameParameters);
                        }
                    }
                    else
                    {
                        result.SuccessMessage = _localizationService.GetResource("Products.Wishlist.ProductAlreadyAdded");
                    }
                }


            }

            #region Return result

            if (addToCartWarnings.ErrorList.Count > 0)
            {
                result.ErrorList = addToCartWarnings.ErrorList.ToList();
                result.IsAttributeError = addToCartWarnings.IsAttributeError;
                result.Success = false;
                result.StatusCode = (int)ErrorType.NotOk;
            }
            else
            {
                //added to the cart/wishlist
                switch (cartType)
                {
                    case ShoppingCartType.Wishlist:
                        {
                            //activity log
                            //_customerActivityService.InsertActivity("PublicStore.AddToWishlist", _localizationService.GetResource("ActivityLog.PublicStore.AddToWishlist"), product.Name);

                            if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct)
                            {
                                result.ForceRedirect = true;
                            }

                            result.Success = true;
                            break;
                        }
                    case ShoppingCartType.ShoppingCart:
                    default:
                        {
                            //activity log
                            // _customerActivityService.InsertActivity("PublicStore.AddToShoppingCart", _localizationService.GetResource("ActivityLog.PublicStore.AddToShoppingCart"), product.Name);
                            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                                .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
                                .LimitPerStore(_storeContext.CurrentStore.Id)
                                .ToList();
                            result.Count = _workContext.CurrentCustomer.ShoppingCartItems
                            .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                            .LimitPerStore(_storeContext.CurrentStore.Id)
                            .ToList()
                            .Sum(x => x.Quantity);
                            if (_shoppingCartSettings.DisplayCartAfterAddingProduct)
                            {
                                result.ForceRedirect = true;
                                result.Success = true;
                            }

                            result.Success = true;
                            break;
                        }
                }
            }
            return Ok(result);

            #endregion
        }

        //handle product attribute selection event. this way we return new price, overridden gtin/sku/mpn
        //currently we use this method on the product details pages
        [Route("api/ProductDetailsPagePrice/{productId}")]
        [HttpPost]
        public IActionResult ProductDetails_AttributeChange(int productId, [FromBody]List<KeyValueApi> formValues)
        {
            var form = new NameValueCollection();
            var attributeCount = 0;
            foreach (var values in formValues)
            {
                attributeCount += values.Key.Contains("product_attribute_") ? 1 : 0;
                form.Add(values.Key, values.Value);
            }
            var result = new ProductDetailPriceResponseModel();
            var errrorList = new List<string>();
            var product = _productService.GetProductById(productId);
            if (product == null)
            {
                //errrorList.Add("Product Not Found");
                //result.ErrorList = errrorList;
                //result.StatusCode = (int)ErrorType.NotFount;
                //return Ok(result);
                return NotFound(new { Message = "Product Not Found" });
            }

            string attributeXml = ParseProductAttributes(product, form);
            // Added by Alexandar Rajavel on 23-Feb-2018
            switch (product.ManageInventoryMethod)
            {
                case ManageInventoryMethod.ManageStock:
                    result.StockQuantity = product.StockQuantity;
                    break;
                case ManageInventoryMethod.ManageStockByAttributes:
                    var checking = _productAttributeService.GetAllProductAttributeCombinations(product.Id);
                    var productAttributeMappings = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
                    if (productAttributeMappings.Count() == attributeCount)
                    {
                        var combinations = _productAttributeParser.FindProductAttributeCombination(product, attributeXml);
                        result.StockQuantity = combinations == null ? 0 : combinations.StockQuantity;
                    }
                    else
                    {
                        result.StockQuantity = checking.Sum(s => s.StockQuantity);
                    }
                    break;
            }


            var productAttributeValue = ParseValueAndGetProductAttributeValue(product, form);

            //rental attributes
            DateTime? rentalStartDate = null;
            DateTime? rentalEndDate = null;
            if (product.IsRental)
            {
                ParseRentalDates(product, form, out rentalStartDate, out rentalEndDate);
            }

            result.Sku = _productService.FormatSku(product, attributeXml);
            result.Mpn = _productService.FormatMpn(product, attributeXml);
            result.Gtin = _productService.FormatGtin(product, attributeXml);
            //Added by jayadevan to display  Measurement Data
            if (productAttributeValue != null)
                result.MeasurementData = productAttributeValue.MeasurementData != null ? productAttributeValue.MeasurementData : string.Empty;


            string price = "";
            if (!product.CustomerEntersPrice)
            {
                //we do not calculate price of "customer enters price" option is enabled
                // Discount scDiscount;
                List<DiscountForCaching> scDiscounts; // change 3.8
                decimal discountAmount;
                decimal finalPrice = _priceCalculationService.GetUnitPrice(product,
                    _workContext.CurrentCustomer,
                    ShoppingCartType.ShoppingCart,
                    1, attributeXml, 0,
                    rentalStartDate, rentalEndDate,
                    true, out discountAmount, out scDiscounts);
                decimal taxRate;
                decimal finalPriceWithDiscountBase = _taxService.GetProductPrice(product, finalPrice, out taxRate);
                decimal finalPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithDiscountBase, _workContext.WorkingCurrency);
                price = _priceFormatter.FormatPrice(finalPriceWithDiscount);
            }
            result.Price = price;
            return Ok(result);

        }

        [Route("api/ShoppingCartCount")]
        [HttpGet]
        public IActionResult ShoppingCartCount()
        {
            var cart = (from _pushlst in _workContext.CurrentCustomer.ShoppingCartItems
                       where _pushlst.ShoppingCartType== ShoppingCartType.ShoppingCart && _pushlst.StoreId==_storeContext.CurrentStore.Id
                              select _pushlst).ToList();
            var model = new ShoppingCartCount();
            model.Count = cart.Sum(x => x.Quantity);
            //added By Sunil Kumar on 23-04-2021
            //if (cart.Any())
            //{
            //    var customer = cart.FirstOrDefault(item => item.Customer != null)?.Customer;
            //    _genericAttributeService.SaveAttribute<string>(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, null, _storeContext.CurrentStore.Id);
            //    _genericAttributeService.SaveAttribute<string>(customer, "SubPaymetnType", null, _storeContext.CurrentStore.Id);
            //}
            return Ok(model);
        }

        [Route("api/ShoppingCart")]
        [HttpGet]
        public IActionResult Cart()
        {
            var cart = (from _pushlst in _workContext.CurrentCustomer.ShoppingCartItems
                        where _pushlst.ShoppingCartType == ShoppingCartType.ShoppingCart && _pushlst.StoreId == _storeContext.CurrentStore.Id
                        select _pushlst).ToList();
            //var cart = cartnew.ToList();

            //var cart = _workContext.CurrentCustomer.ShoppingCartItems
            //    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
            //    .LimitPerStore(_storeContext.CurrentStore.Id)
            //    .ToList();
            var model = new ShoppingCartResponseModel();

            _shoppingCartModelFactoryApi.PrepareShoppingCartModel(model, cart);
            //commented on 2/1/19 by Sunil Kumar - uncommented on 7/1/19 by Sunil Kumar
            model.Count = cart.Sum(x => x.Quantity);

            //added By Sunil Kumar at 13-04-2020
            if (cart.Any())
            {
                var customer = cart.FirstOrDefault(item => item.Customer != null)?.Customer;
                _genericAttributeService.SaveAttribute<string>(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, null, _storeContext.CurrentStore.Id);
                _genericAttributeService.SaveAttribute<string>(customer, "SubPaymetnType", null, _storeContext.CurrentStore.Id);
            }
            model.OrderTotalResponseModel = _shoppingCartModelFactoryApi.PrepareOrderTotalsModel(cart, true);
            return Ok(model);
        }

        [Route("api/ShoppingCart/applycheckoutattribute")]
        [HttpPost]
        public IActionResult ApplyCheckoutAttribute([FromBody]List<KeyValueApi> formValues)
        {
            var form = new NameValueCollection();
            foreach (var values in formValues)
            {
                form.Add(values.Key, values.Value);
            }

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            ParseAndSaveCheckoutAttributes(cart, form);

            var model = new GeneralResponseModel<bool>()
            {
                Data = true
            };
            return Ok(model);

        }

        [Route("api/ShoppingCart/UpdateCart")]
        [HttpPost]
        public IActionResult UpdateCart( [FromBody]List<KeyValueApi> formValues)
        {

            int TicalCount = 1;
            var form = new NameValueCollection();
            foreach (var values in formValues)
            {
                form.Add(values.Key, values.Value);
            }

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var allIdsToRemove = form["removefromcart"] != null ? form["removefromcart"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() : new List<int>();

            //current warnings <cart item identifier, warnings>
            var innerWarnings = new Dictionary<int, IList<string>>();
            foreach (var sci in cart)
            {
                bool remove = allIdsToRemove.Contains(sci.Id);
                if (remove)
                {
                    // Added by Alexandar Rajavel on 27-Feb-2019 to release the product from reserved
                    if (!string.IsNullOrEmpty(sci.Product.ReservedCustomerIds) && sci.Product.ReservedCustomerIds.Contains(_workContext.CurrentCustomer.Id.ToString()))
                    {
                        var product = sci.Product;
                        product.ReservedQty -= sci.Quantity;
                        product.IsReserved = product.ReservedQty <= 0 ? false : true;
                        product.ReservedCustomerIds = product.ReservedCustomerIds.Replace(_workContext.CurrentCustomer.Id.ToString(), "");
                        _productService.UpdateProduct(product);
                    }
                    _shoppingCartService.DeleteShoppingCartItem(sci, ensureOnlyActiveCheckoutAttributes: true);
                }
                else
                {
                    foreach (string formKey in form.AllKeys)
                        if (formKey.Equals(string.Format("itemquantity{0}", sci.Id), StringComparison.InvariantCultureIgnoreCase))
                        {
                            int newQuantity;
                            if (int.TryParse(form[formKey], out newQuantity))
                            {
                                var currSciWarnings = _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                                    sci.Id, sci.AttributesXml, sci.CustomerEnteredPrice,
                                    sci.RentalStartDateUtc, sci.RentalEndDateUtc,
                                    newQuantity, true, TicalCount );
                                innerWarnings.Add(sci.Id, currSciWarnings);
                            }
                            break;
                        }
                }
            }

            //updated cart
            cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new ShoppingCartResponseModel();
            _shoppingCartModelFactoryApi.PrepareShoppingCartModel(model, cart);
            //update current warnings
            model.ErrorList = new List<string>();
            foreach (var kvp in innerWarnings)
            {
                //kvp = <cart item identifier, warnings>
                var sciId = kvp.Key;
                var warnings = kvp.Value;
                //find model
                var sciModel = model.Items.FirstOrDefault(x => x.Id == sciId);
                if (sciModel != null)
                    foreach (var w in warnings)
                        if (!model.ErrorList.Contains(w))
                            model.ErrorList.Add(w);


            }
            if (model.ErrorList.Count > 0)
            {
                model.StatusCode = (int)ErrorType.NotOk;
            }
            model.Count = cart.Sum(x => x.Quantity);
            model.OrderTotalResponseModel = _shoppingCartModelFactoryApi.PrepareOrderTotalsModel(cart, true);
            return Ok(model);
        }

        [Route("api/ShoppingCart/UpdateCart/{TicalCount}")]
        [HttpPost]
        public IActionResult UpdateCart(int TicalCount, [FromBody]List<KeyValueApi> formValues)
        {
            var form = new NameValueCollection();
            foreach (var values in formValues)
            {
                form.Add(values.Key, values.Value);
            }

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var allIdsToRemove = form["removefromcart"] != null ? form["removefromcart"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() : new List<int>();

            //current warnings <cart item identifier, warnings>
            var innerWarnings = new Dictionary<int, IList<string>>();
            foreach (var sci in cart)
            {
                bool remove = allIdsToRemove.Contains(sci.Id);
                if (remove)
                {
                    // Added by Alexandar Rajavel on 27-Feb-2019 to release the product from reserved
                    if (!string.IsNullOrEmpty(sci.Product.ReservedCustomerIds) && sci.Product.ReservedCustomerIds.Contains(_workContext.CurrentCustomer.Id.ToString()))
                    {
                        var product = sci.Product;
                        product.ReservedQty -= sci.Quantity;
                        product.IsReserved = product.ReservedQty <= 0 ? false : true;
                        product.ReservedCustomerIds = product.ReservedCustomerIds.Replace(_workContext.CurrentCustomer.Id.ToString(), "");
                        _productService.UpdateProduct(product);
                    }
                    _shoppingCartService.DeleteShoppingCartItem(sci, ensureOnlyActiveCheckoutAttributes: true);
                }
                else
                {
                    foreach (string formKey in form.AllKeys)
                        if (formKey.Equals(string.Format("itemquantity{0}", sci.Id), StringComparison.InvariantCultureIgnoreCase))
                        {
                            int newQuantity;
                            if (int.TryParse(form[formKey], out newQuantity))
                            {
                                var currSciWarnings = _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                                    sci.Id, sci.AttributesXml, sci.CustomerEnteredPrice,
                                    sci.RentalStartDateUtc, sci.RentalEndDateUtc,
                                    newQuantity, true, TicalCount);
                                innerWarnings.Add(sci.Id, currSciWarnings);
                            }
                            break;
                        }
                }
            }

            //updated cart
            cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new ShoppingCartResponseModel();
            _shoppingCartModelFactoryApi.PrepareShoppingCartModel(model, cart);
            //update current warnings
            model.ErrorList = new List<string>();
            foreach (var kvp in innerWarnings)
            {
                //kvp = <cart item identifier, warnings>
                var sciId = kvp.Key;
                var warnings = kvp.Value;
                //find model
                var sciModel = model.Items.FirstOrDefault(x => x.Id == sciId);
                if (sciModel != null)
                    foreach (var w in warnings)
                        if (!model.ErrorList.Contains(w))
                            model.ErrorList.Add(w);


            }
            if (model.ErrorList.Count > 0)
            {
                model.StatusCode = (int)ErrorType.NotOk;
            }
            model.Count = cart.Sum(x => x.Quantity);
            model.OrderTotalResponseModel = _shoppingCartModelFactoryApi.PrepareOrderTotalsModel(cart, true);
            return Ok(model);
        }

        [Route("api/ShoppingCart/ApplyGiftCard")]
        [HttpPost]
        public IActionResult ApplyGiftCard([FromBody]SingleValue value)
        {
            string giftcardcouponcode = value.Value;
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var model = new CoupontypeResponse();
            model.OrderTotalResponseModel = _shoppingCartModelFactoryApi.PrepareOrderTotalsModel(cart, true);
            var errorList = new List<string>();
            string error = string.Empty;

            if (!_shoppingCartService.ShoppingCartIsRecurring(cart))
            {
                if (!String.IsNullOrWhiteSpace(giftcardcouponcode))
                {
                    var giftCard = _giftCardService.GetAllGiftCards(giftCardCouponCode: giftcardcouponcode).FirstOrDefault();
                    bool isGiftCardValid = giftCard != null && _giftCardService.IsGiftCardValid(giftCard);
                    if (isGiftCardValid)
                    {
                        _customerService.ApplyGiftCardCouponCode(_workContext.CurrentCustomer, giftcardcouponcode);
                        _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                        //model.GiftCardBox.Message = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.Applied");
                        model.Data = true;
                    }
                    else
                    {
                        error = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.WrongGiftCard");

                    }
                }
                else
                {
                    error = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.WrongGiftCard");

                }
            }
            else
            {
                error = _localizationService.GetResource("ShoppingCart.GiftCardCouponCode.DontWorkWithAutoshipProducts");

            }
            if (!string.IsNullOrEmpty(error))
            {
                errorList.Add(error);
                model.ErrorList = errorList;
                model.Data = false;
                model.StatusCode = (int)ErrorType.NotOk;
            }
            // PrepareShoppingCartModel(model, cart);
            return Ok(model);
        }

        [Route("api/ShoppingCart/RemoveGiftCard")]
        [HttpPost]
        public IActionResult RemoveGiftCardCode([FromBody]SingleValue value)
        {
            var model = new CoupontypeResponse();
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            model.OrderTotalResponseModel = _shoppingCartModelFactoryApi.PrepareOrderTotalsModel(cart, true);
            //get gift card identifier
            int giftCardId = Convert.ToInt32(value.Value);
            var gc = _giftCardService.GetGiftCardById(giftCardId);
            if (gc != null)
            {
                _customerService.RemoveGiftCardCouponCode(_workContext.CurrentCustomer, gc.GiftCardCouponCode);
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                model.Data = true;
            }
            else
            {
                model.StatusCode = (int)ErrorType.NotOk;
                model.Data = false;
            }

            return Ok(model);
        }

        [Route("api/ShoppingCart/ApplyDiscountCoupon")]
        [HttpPost]
        public IActionResult ApplyDiscountCoupon([FromBody]SingleValue value)
        {
            string discountcouponcode = value.Value;
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var model = new CoupontypeResponse();


            List<string> errorList = new List<string>();

            if (!String.IsNullOrWhiteSpace(discountcouponcode))
            {
                var discounts = _discountService.GetAllDiscountsForCaching(couponCode: discountcouponcode, showHidden: true)
                    .Where(d => d.RequiresCouponCode)
                    .ToList();
                if (discounts.Any())
                {
                    var userErrors = new List<string>();
                    var anyValidDiscount = discounts.Any(discount =>
                    {
                        var validationResult = _discountService.ValidateDiscount(discount, _workContext.CurrentCustomer, new[] { discountcouponcode });
                        userErrors.AddRange(validationResult.Errors);

                        return validationResult.IsValid;
                    });
                    if (anyValidDiscount)
                    {
                        //valid
                        _customerService.ApplyDiscountCouponCode(_workContext.CurrentCustomer, discountcouponcode);
                        model.Data = true;
                    }
                    else
                    {
                        if (userErrors.Any())
                        {
                            //some user error
                            errorList = userErrors;
                            model.Data = false;
                        }
                        else
                        {
                            //general error text
                            errorList.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount"));
                            model.Data = false;
                        }
                    }
                }
                else
                {
                    //discount cannot be found
                    errorList.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount"));
                    model.Data = false;
                }
            }
            else
            {
                errorList.Add(_localizationService.GetResource("ShoppingCart.DiscountCouponCode.WrongDiscount"));
                model.Data = false;
            }

            if (errorList.Any())
            {
                model.ErrorList = errorList;
                model.Data = false;
                model.StatusCode = (int)ErrorType.NotOk;
            }
            model.OrderTotalResponseModel = _shoppingCartModelFactoryApi.PrepareOrderTotalsModel(cart, true);
            return Ok(model);
        }

        [Route("api/ShoppingCart/RemoveDiscountCoupon")]
        [HttpGet]
        public IActionResult ExistingDiscounts()
        {
            var model = _shoppingCartModelFactoryApi.PrepareExistingDiscountsModel();

            return Ok(model);
        }


        [Route("api/ShoppingCart/RemoveDiscountCoupon")]
        [HttpPost]
        public IActionResult RemoveDiscountCoupon([FromBody]SingleValue value)
        {
            //get discount identifier            
            int discountId = Convert.ToInt32(value.Value);
            var discount = _discountService.GetDiscountById(discountId);
            if (discount != null)
                _customerService.RemoveDiscountCouponCode(_workContext.CurrentCustomer, discount.CouponCode);

            var model = new CoupontypeResponse();
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            model.OrderTotalResponseModel = _shoppingCartModelFactoryApi.PrepareOrderTotalsModel(cart, true);
            model.Data = true;
            return Ok(model);
        }

        [Route("api/ShoppingCart/OrderTotal")]
        [HttpGet]
        public IActionResult OrderTotals()
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = _shoppingCartModelFactoryApi.PrepareOrderTotalsModel(cart, true);
            return Ok(model);
        }

        [Route("api/shoppingCart/wishlist")]
        [HttpGet]
        public IActionResult Wishlist(Guid? customerGuid = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                return Challenge();

            Customer customer = customerGuid.HasValue ?
                _customerService.GetCustomerByGuid(customerGuid.Value)
                : _workContext.CurrentCustomer;
            if (customer == null)
                return Challenge();
            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist && sci.Product.Published == true)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var model = new WishlistResponseModel();
            _shoppingCartModelFactoryApi.PrepareWishlistModel(model, cart, !customerGuid.HasValue);
            return Ok(model);
        }

        [Route("api/ShoppingCart/UpdateWishlist")]
        [HttpPost]
        public IActionResult UpdateWishlist([FromBody]List<KeyValueApi> formValues)
        {
            var form = new NameValueCollection();
            foreach (var values in formValues)
            {
                form.Add(values.Key, values.Value);
            }
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                return Challenge();

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var allIdsToRemove = form["removefromcart"] != null
                ? form["removefromcart"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList()
                : new List<int>();

            //current warnings <cart item identifier, warnings>
            var innerWarnings = new Dictionary<int, IList<string>>();
            foreach (var sci in cart)
            {
                bool remove = allIdsToRemove.Contains(sci.Id);
                if (remove)
                    _shoppingCartService.DeleteShoppingCartItem(sci);
                else
                {
                    foreach (string formKey in form.AllKeys)
                        if (formKey.Equals(string.Format("itemquantity{0}", sci.Id), StringComparison.InvariantCultureIgnoreCase))
                        {
                            int newQuantity;
                            if (int.TryParse(form[formKey], out newQuantity))
                            {
                                var currSciWarnings = _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                                    sci.Id, sci.AttributesXml, sci.CustomerEnteredPrice,
                                    sci.RentalStartDateUtc, sci.RentalEndDateUtc,
                                    newQuantity, true);
                                innerWarnings.Add(sci.Id, currSciWarnings);
                            }
                            break;
                        }
                }
            }

            //updated wishlist
            cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new WishlistResponseModel();
            _shoppingCartModelFactoryApi.PrepareWishlistModel(model, cart);
            //update current warnings
            foreach (var kvp in innerWarnings)
            {
                //kvp = <cart item identifier, warnings>
                var sciId = kvp.Key;
                var warnings = kvp.Value;
                //find model
                var sciModel = model.Items.FirstOrDefault(x => x.Id == sciId);
                if (sciModel != null)
                    foreach (var w in warnings)
                        if (!model.ErrorList.Contains(w))
                            model.ErrorList.Add(w);


            }
            if (model.ErrorList.Count > 0)
            {
                model.StatusCode = (int)ErrorType.NotOk;
            }
            return Ok(model);
        }

        [Route("api/ShoppingCart/AddItemsToCartFromWishlist")]
        [HttpPost]
        public IActionResult AddItemsToCartFromWishlist([FromBody]List<KeyValueApi> formValues, Guid? customerGuid = null)
        {
            Customer customer = customerGuid.HasValue ?
               _customerService.GetCustomerByGuid(customerGuid.Value)
               : _workContext.CurrentCustomer;
            var form = new NameValueCollection();
            foreach (var values in formValues)
            {
                form.Add(values.Key, values.Value);
            }
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return Challenge();

            var response = new WishlistResponseModel();

            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                return Challenge();

            var pageCustomer = customerGuid.HasValue
                ? _customerService.GetCustomerByGuid(customerGuid.Value)
                : _workContext.CurrentCustomer;
            if (pageCustomer == null)
                return Challenge();

            var pageCart = pageCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            // var numberOfAddedItems = 0;
            var allIdsToAdd = form["addtocart"] != null
                ? form["addtocart"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList()
                : new List<int>();
            foreach (var sci in pageCart)
            {
                if (allIdsToAdd.Contains(sci.Id))
                {
                    var warnings = _shoppingCartService.AddToCart(_workContext.CurrentCustomer,
                        sci.Product, ShoppingCartType.ShoppingCart,
                        _storeContext.CurrentStore.Id,
                        sci.AttributesXml, sci.CustomerEnteredPrice,
                        sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, true, 1);

                    if (_shoppingCartSettings.MoveItemsFromWishlistToCart && //settings enabled
                        !customerGuid.HasValue && //own wishlist
                        warnings.ErrorList.Count == 0) //no warnings ( already in the cart)
                    {
                        //let's remove the item from wishlist
                        _shoppingCartService.DeleteShoppingCartItem(sci);
                    }
                    response.ErrorList.AddRange(warnings.ErrorList);
                }
            }
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                 .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                 .ToList();
            response.Count = cart.Sum(item => item.Quantity);
            var wishlistCart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            _shoppingCartModelFactoryApi.PrepareWishlistModel(response, wishlistCart, !customerGuid.HasValue);

            if (response.ErrorList.Count > 0)
            {
                response.StatusCode = (int)ErrorType.NotOk;
            }
            return Ok(response);
        }

        [Route("api/shoppingcart/checkoutorderinformation")]
        [HttpGet]
        public IActionResult CheckoutOrderInformation()
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
               .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
              .LimitPerStore(_storeContext.CurrentStore.Id)
               .ToList();

            var model = new CheckoutInformationResponseModel();
            _shoppingCartModelFactoryApi.PrepareShoppingCartModel(model.ShoppingCartModel, cart, prepareAndDisplayOrderReviewData: true);
            model.OrderTotalModel = _shoppingCartModelFactoryApi.PrepareOrderTotalsModel(cart, true);



          


            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            // Added by Alexandar Rajavel on 06-Feb-2019
            var oKDollarPaymentSettings = _settingService.LoadSetting<OKDollarPaymentSettings>(storeScope);
            model.MerchantNumber = oKDollarPaymentSettings.Destination;

            return Ok(model);
        }

        [Route("api/v1/shoppingcart/checkoutorderinformation")]
        [HttpGet]
        public IActionResult V1CheckoutOrderInformation()
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
               .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
              .LimitPerStore(_storeContext.CurrentStore.Id)
               .ToList();

            var model = new CheckoutInformationResponseModel();
            _shoppingCartModelFactoryApi.PrepareShoppingCartModel(model.ShoppingCartModel, cart, prepareAndDisplayOrderReviewData: true);
            model.OrderTotalModel = _shoppingCartModelFactoryApi.PrepareOrderTotalsModel(cart, true);






            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            // Added by Alexandar Rajavel on 06-Feb-2019
            var oKDollarPaymentSettings = _settingService.LoadSetting<OKDollarPaymentSettings>(storeScope);
            model.MerchantNumber = oKDollarPaymentSettings.Destination;

            return Ok(model);
        }

        /// <summary>
        /// Created By : Sunil Kumar S
        /// Created Date : 13/12/2018
        [Route("api/ShoppingCart/RemoveProduct/{productId}/{shoppingCartTypeId}")]
        [HttpDelete]
        public IActionResult RemoveProduct(int productId, int shoppingCartTypeId)
        {
            var itesmDeleted = _shoppingCartService.DeleteShoppingCartItems(productId, _workContext.CurrentCustomer.Id, shoppingCartTypeId);
            var response = new GeneralResponseModel<string>();
            if (itesmDeleted > 0)
            {
                response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                response.Data = "Success";
            }
            else
            {
                response.StatusCode = (int)ErrorType.NotFound;
                response.Data = "Not Found";
                return NotFound(response);
            }

            return Ok(response);
        }
        #endregion
    }
}

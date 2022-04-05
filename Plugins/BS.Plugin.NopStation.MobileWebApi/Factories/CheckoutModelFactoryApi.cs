using System;
using System.Collections.Generic;
using System.Linq;
using BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Checkout;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using Nop.Core.Domain.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Payment;
using Newtonsoft.Json;
using System.IO;
using static BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Checkout.CheckoutPaymentMethodResponseModel;
using Microsoft.Extensions.Configuration;

namespace BS.Plugin.NopStation.MobileWebApi.Factories
{
    public partial class CheckoutModelFactoryApi : ICheckoutModelFactoryApi
    {
        #region Fields
        private readonly IAddressModelFactoryApi _addressModelFactoryApi;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ILocalizationService _localizationService;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IShippingService _shippingService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IWebHelper _webHelper;

        private readonly OrderSettings _orderSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly AddressSettings _addressSettings;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IConfiguration configuration;
        private const string THE_PRODUCT_IS_RESERVED = "The product is reserved try after {0} minutes.";
        #endregion

        #region Ctor
        public CheckoutModelFactoryApi(IAddressModelFactoryApi addressModelFactoryApi,
            IWorkContext workContext,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            ILocalizationService localizationService,
            ITaxService taxService,
            ICurrencyService currencyService,
            IPriceFormatter priceFormatter,
            IOrderProcessingService orderProcessingService,
            IGenericAttributeService genericAttributeService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IShippingService shippingService,
            IPaymentService paymentService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IRewardPointService rewardPointService,
            IWebHelper webHelper,
            OrderSettings orderSettings,
            RewardPointsSettings rewardPointsSettings,
            PaymentSettings paymentSettings,
            ShippingSettings shippingSettings,
            AddressSettings addressSettings,
            IShoppingCartService shoppingCartService,
            IProductService productService,
            IProductAttributeParser productAttributeParser,
            IConfiguration iconfig)
        {
            this._addressModelFactoryApi = addressModelFactoryApi;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeMappingService = storeMappingService;
            this._localizationService = localizationService;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._orderProcessingService = orderProcessingService;
            this._genericAttributeService = genericAttributeService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._shippingService = shippingService;
            this._paymentService = paymentService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._rewardPointService = rewardPointService;
            this._webHelper = webHelper;

            this._orderSettings = orderSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._paymentSettings = paymentSettings;
            this._shippingSettings = shippingSettings;
            this._addressSettings = addressSettings;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
            _productAttributeParser = productAttributeParser;
            configuration = iconfig;
        }
        #endregion

        #region Utilities

        public CheckoutShippingMethodResponseModel.ShippingOptionModel PrepareShippingOptionModel(
            ShippingOption shippingOption)
        {
            var shippingOptionModel = new CheckoutShippingMethodResponseModel.ShippingOptionModel
            {
                ShippingRateComputationMethodSystemName = shippingOption.ShippingRateComputationMethodSystemName,
                Rate = shippingOption.Rate,
                Name = shippingOption.Name,
                Description = shippingOption.Description
            };

            return shippingOptionModel;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Prepare billing address model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="selectedCountryId">Selected country identifier</param>
        /// <param name="prePopulateNewAddressWithCustomerFields">Pre populate new address with customer fields</param>
        /// <param name="overrideAttributesXml">Override attributes xml</param>
        /// <returns>Billing address model</returns>
        public virtual CheckoutBillingAddressResponseModel PrepareBillingAddressModel(int? selectedCountryId = null,
            bool prePopulateNewAddressWithCustomerFields = false,
            string overrideAttributesXml = "")
        {
            var model = new CheckoutBillingAddressResponseModel();

            //existing addresses
            var addresses = _workContext.CurrentCustomer.Addresses
                .Where(a => a.Country == null ||
                    (//published
                    a.Country.Published &&
                    //allow billing
                    a.Country.AllowsBilling &&
                    //enabled for the current store
                    _storeMappingService.Authorize(a.Country) &&
                    !string.IsNullOrEmpty(a.Address1)))
                .ToList();

            //Add already existing billing address at first place changed By Ankur 27 oct. 2018
            if (_workContext.CurrentCustomer.BillingAddress != null && addresses.Count > 0)
            {
                Address address = addresses.Where(o => o.Id == _workContext.CurrentCustomer.BillingAddress.Id).FirstOrDefault();
                var addressModel = new AddressModel();
                _addressModelFactoryApi.PrepareAddressModel(addressModel,
                    address: address,
                    excludeProperties: false,
                    addressSettings: _addressSettings);
                model.ExistingAddresses.Add(addressModel);
                addresses.Remove(address);
            }

            foreach (var address in addresses)
            {

                var addressModel = new AddressModel();
                _addressModelFactoryApi.PrepareAddressModel(addressModel,
                    address: address,
                    excludeProperties: false,
                    addressSettings: _addressSettings);
                model.ExistingAddresses.Add(addressModel);
            }

            //new address
            model.NewAddress.CountryId = selectedCountryId;
            _addressModelFactoryApi.PrepareAddressModel(model.NewAddress,
                address: null,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountriesForBilling(_workContext.WorkingLanguage.Id),
                prePopulateWithCustomerFields: prePopulateNewAddressWithCustomerFields,
                customer: _workContext.CurrentCustomer,
                overrideAttributesXml: overrideAttributesXml);
            if (String.IsNullOrEmpty(model.NewAddress.Email) && String.IsNullOrEmpty(model.NewAddress.FirstName) && String.IsNullOrEmpty(model.NewAddress.LastName) && String.IsNullOrEmpty(model.NewAddress.Company) && String.IsNullOrEmpty(model.NewAddress.Address1) && String.IsNullOrEmpty(model.NewAddress.Address2) && String.IsNullOrEmpty(model.NewAddress.ZipPostalCode) && String.IsNullOrEmpty(model.NewAddress.City) && String.IsNullOrEmpty(model.NewAddress.FaxNumber))
            {
                var addressModel = new AddressModel();
                model.NewAddress = null;
            }

            return model;
        }


        /// <summary>
        /// Prepare shipping method model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="shippingAddress">Shipping address</param>
        /// <returns>Shipping method model</returns>
        public virtual CheckoutShippingMethodResponseModel PrepareShippingMethodModel(IList<ShoppingCartItem> cart, Address shippingAddress)
        {
            var model = new CheckoutShippingMethodResponseModel();

            var getShippingOptionResponse = _shippingService.GetShippingOptions(cart, shippingAddress, _workContext.CurrentCustomer, storeId: _storeContext.CurrentStore.Id);

            if (getShippingOptionResponse.Success)
            {
                //performance optimization. cache returned shipping options.
                //we'll use them later (after a customer has selected an option).
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                                                       NopCustomerDefaults.OfferedShippingOptionsAttribute,
                                                       getShippingOptionResponse.ShippingOptions,
                                                       _storeContext.CurrentStore.Id);

                foreach (var shippingOption in getShippingOptionResponse.ShippingOptions)
                {
                    var soModel = new CheckoutShippingMethodResponseModel.ShippingMethodModel
                    {
                        Name = shippingOption.Name,
                        Description = shippingOption.Description,
                        ShippingRateComputationMethodSystemName = shippingOption.ShippingRateComputationMethodSystemName,
                        ShippingOption = PrepareShippingOptionModel(shippingOption),
                    };

                    //adjust rate
                    List<DiscountForCaching> appliedDiscounts;
                    var shippingTotal = _orderTotalCalculationService.AdjustShippingRate(
                        shippingOption.Rate, cart, out appliedDiscounts);

                    decimal rateBase = _taxService.GetShippingPrice(shippingTotal, _workContext.CurrentCustomer);
                    decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                    soModel.Fee = _priceFormatter.FormatShippingPrice(rate, true);

                    model.ShippingMethods.Add(soModel);
                }

                //find a selected (previously) shipping method
                var selectedShippingOption = _genericAttributeService.GetAttribute<ShippingOption>(_workContext.CurrentCustomer,
                        NopCustomerDefaults.SelectedShippingOptionAttribute, _storeContext.CurrentStore.Id);
                if (selectedShippingOption != null)
                {
                    var shippingOptionToSelect = model.ShippingMethods.ToList()
                        .Find(so =>
                           !String.IsNullOrEmpty(so.Name) &&
                           so.Name.Equals(selectedShippingOption.Name, StringComparison.InvariantCultureIgnoreCase) &&
                           !String.IsNullOrEmpty(so.ShippingRateComputationMethodSystemName) &&
                           so.ShippingRateComputationMethodSystemName.Equals(selectedShippingOption.ShippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase));
                    if (shippingOptionToSelect != null)
                    {
                        shippingOptionToSelect.Selected = true;
                    }
                }
                //if no option has been selected, let's do it for the first one
                if (model.ShippingMethods.FirstOrDefault(so => so.Selected) == null)
                {
                    var shippingOptionToSelect = model.ShippingMethods.FirstOrDefault();
                    if (shippingOptionToSelect != null)
                    {
                        shippingOptionToSelect.Selected = true;
                    }
                }

                //notify about shipping from multiple locations
                if (_shippingSettings.NotifyCustomerAboutShippingFromMultipleLocations)
                {
                    model.NotifyCustomerAboutShippingFromMultipleLocations = getShippingOptionResponse.ShippingFromMultipleLocations;
                }
            }
            else
            {
                foreach (var error in getShippingOptionResponse.Errors)
                    model.Warnings.Add(error);
            }

            return model;
        }




        /// <summary>
        /// Prepare payment method model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="filterByCountryId">Filter by country identifier</param>
        /// <returns>Payment method model</returns>
        public virtual CheckoutPaymentMethodResponseModel PreparePaymentMethodModel(IList<ShoppingCartItem> cart, int filterByCountryId)
        {
            var model = new CheckoutPaymentMethodResponseModel();

            //reward points
            if (_rewardPointsSettings.Enabled && !_shoppingCartService.ShoppingCartIsRecurring(cart))
            {
                int rewardPointsBalance = _rewardPointService.GetRewardPointsBalance(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
                decimal rewardPointsAmountBase = _orderTotalCalculationService.ConvertRewardPointsToAmount(rewardPointsBalance);
                decimal rewardPointsAmount = _currencyService.ConvertFromPrimaryStoreCurrency(rewardPointsAmountBase, _workContext.WorkingCurrency);
                if (rewardPointsAmount > decimal.Zero &&
                    _orderTotalCalculationService.CheckMinimumRewardPointsToUseRequirement(rewardPointsBalance))
                {
                    model.DisplayRewardPoints = true;
                    model.RewardPointsAmount = _priceFormatter.FormatPrice(rewardPointsAmount, true, false);
                    model.RewardPointsBalance = rewardPointsBalance;

                    //are points enough to pay for entire order? like if this option (to use them) was selected
                    //model.RewardPointsEnoughToPayForOrder = !_orderProcessingService.IsPaymentWorkflowRequired(cart, true);
                }
            }

            //filter by country
            var paymentMethods = _paymentService
                .LoadActivePaymentMethods(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id, filterByCountryId)
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Standard || pm.PaymentMethodType == PaymentMethodType.Redirection)
                .Where(pm => !pm.HidePaymentMethod(cart))
                .ToList();

            // Added by Alexandar Rajavel on 27-Feb-2019 for Online inventory management
            //var canLoadPaymentMethod = false;
            var timeLimit = string.Empty;
            //foreach (var item in cart)
            //{
            //    var stockQuantity = 0;
            //    switch (item.Product.ManageInventoryMethod)
            //    {
            //        case ManageInventoryMethod.ManageStock:
            //            stockQuantity = item.Product.StockQuantity;
            //            break;
            //        case ManageInventoryMethod.ManageStockByAttributes:
            //            var combinations = _productAttributeParser.FindProductAttributeCombination(item.Product, item.AttributesXml);
            //            stockQuantity = combinations == null ? 0 : combinations.StockQuantity;
            //            break;
            //    }
            //    if (stockQuantity == 1 && !item.Product.IsReserved)
            //    {
            //        var product = item.Product;
            //        product.IsReserved = true;
            //        product.UpdatedOnUtc = DateTime.UtcNow;
            //        product.ReservedCustomerId = _workContext.CurrentCustomer.Id;
            //        _productService.UpdateProduct(product);
            //    }
            //    else if (stockQuantity == 1 && item.Product.IsReserved)
            //    {
            //        DateTime dateTimeNow = DateTime.UtcNow;
            //        DateTime lastUpdateTime = item.Product.UpdatedOnUtc;
            //        TimeSpan span = dateTimeNow.Subtract(lastUpdateTime);
            //        var minutes = span.TotalMinutes;
            //        timeLimit = _localizationService.GetResource("OIM_TimeLimit");
            //        canLoadPaymentMethod = minutes > Convert.ToDouble(timeLimit) ? true : false;
            //        if (canLoadPaymentMethod)
            //        {
            //            var product = item.Product;
            //            product.IsReserved = true;
            //            product.UpdatedOnUtc = DateTime.UtcNow;
            //            product.ReservedCustomerId = _workContext.CurrentCustomer.Id;
            //            _productService.UpdateProduct(product);
            //        }
            //        if (minutes < Convert.ToDouble(timeLimit) && item.Product.ReservedCustomerId == _workContext.CurrentCustomer.Id)
            //        {
            //            canLoadPaymentMethod = true;
            //        }
            //    }
            //    if (!canLoadPaymentMethod)
            //        break;
            //}

            //foreach (var item in cart)
            //{
            //    var stockQuantity = 0;
            //    switch (item.Product.ManageInventoryMethod)
            //    {
            //        case ManageInventoryMethod.ManageStock:
            //            stockQuantity = item.Product.StockQuantity;
            //            break;
            //        case ManageInventoryMethod.ManageStockByAttributes:
            //            var combinations = _productAttributeParser.FindProductAttributeCombination(item.Product, item.AttributesXml);
            //            stockQuantity = combinations == null ? 0 : combinations.StockQuantity;
            //            break;
            //    }

            //    DateTime dateTimeNow = DateTime.UtcNow;
            //    DateTime lastUpdateTime = item.Product.UpdatedOnUtc;
            //    TimeSpan span = dateTimeNow.Subtract(lastUpdateTime);
            //    var minutes = span.TotalMinutes;
            //    timeLimit = _localizationService.GetResource("OIM_TimeLimit");
            //    //canLoadPaymentMethod = minutes > Convert.ToDouble(timeLimit) ? true : false;

            //    if (!item.Product.IsReserved)
            //    {
            //        canLoadPaymentMethod = true;
            //        var product = item.Product;
            //        product.IsReserved = true;
            //        product.UpdatedOnUtc = DateTime.UtcNow;
            //        product.ReservedQty = item.Quantity;
            //        product.ReservedCustomerIds = _workContext.CurrentCustomer.Id.ToString();
            //        _productService.UpdateProduct(product);
            //    }
            //    else if (item.Product.IsReserved && minutes < Convert.ToDouble(timeLimit))
            //    {
            //        if (item.Product.ReservedCustomerIds.Contains(_workContext.CurrentCustomer.Id.ToString()) && stockQuantity >= item.Product.ReservedQty)
            //        {
            //            canLoadPaymentMethod = true;
            //        }
            //        else if (stockQuantity >= (item.Product.ReservedQty + item.Quantity))
            //        {
            //            canLoadPaymentMethod = true;
            //            var product = item.Product;
            //            product.IsReserved = true;
            //            product.UpdatedOnUtc = DateTime.UtcNow;
            //            product.ReservedQty += item.Quantity;
            //            product.ReservedCustomerIds += "," + _workContext.CurrentCustomer.Id;
            //            _productService.UpdateProduct(product);
            //        }
            //    }
            //    else if (item.Product.IsReserved && minutes > Convert.ToDouble(timeLimit) && stockQuantity >= item.Quantity)
            //    {
            //        canLoadPaymentMethod = true;
            //        var product = item.Product;
            //        product.IsReserved = true;
            //        product.UpdatedOnUtc = DateTime.UtcNow;
            //        product.ReservedQty = item.Quantity;
            //        product.ReservedCustomerIds = _workContext.CurrentCustomer.Id.ToString();
            //        _productService.UpdateProduct(product);
            //    }
            //    if (!canLoadPaymentMethod)
            //        break;
            //}

            //if (canLoadPaymentMethod)
            //{
                foreach (var pm in paymentMethods)
                {
                    if (_shoppingCartService.ShoppingCartIsRecurring(cart) && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                        continue;

                    var pmModel = new CheckoutPaymentMethodResponseModel.PaymentMethodModel
                    {
                        Name = _localizationService.GetLocalizedFriendlyName(pm, _workContext.WorkingLanguage.Id),
                        PaymentMethodSystemName = pm.PluginDescriptor.SystemName,
                        LogoUrl = PluginManager.GetLogoUrl(pm.PluginDescriptor),
                        PAYMENT_URL= configuration.GetValue<string>("OKDollarPAYMENT_URL:Key"),
                        API_KEY = configuration.GetValue<string>("OKDollarAPI_KEY:Key"),
                        SECRET_KEY = configuration.GetValue<string>("OKDollarSECRET_KEY:Key"),
                        IV_VALUE = configuration.GetValue<string>("OKDollarIV_VALUE:Key")
            };
                //payment method additional fee
                //decimal paymentMethodAdditionalFee = 0;
                //decimal rateBase =0;
                decimal rate =0;

                //decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, pm.PluginDescriptor.SystemName);
                //decimal rateBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, _workContext.CurrentCustomer);
                //decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                if (rate > decimal.Zero)
                        pmModel.Fee = _priceFormatter.FormatPaymentMethodAdditionalFee(rate, true);

                    #region CashOnDelivery
                    var paymentMethodList = new List<string>();
                    //Added by Sunil Kumar At 12-03-2020 changed string from CashOnDelivery to COD
                    paymentMethodList.Add("Payments.COD");
                    paymentMethodList.Add("Payments.CheckMoneyOrder");
                    paymentMethodList.Add("Payments.Manual");
                    //Added by Sunil Kumar At 25-03-2020 for Other Payment Custom Properties
                    paymentMethodList.Add("Payments.OtherPayment");

                    if (paymentMethodList.Any(x => x.Equals(pmModel.PaymentMethodSystemName)) || pm.PaymentMethodType == PaymentMethodType.Redirection)
                    {
                    List<SubPaymentMethodName> objLisT = new List<SubPaymentMethodName>();
                    if (pmModel.PaymentMethodSystemName == "Payments.2C2P")
                        {
                            var folderDetails = Path.Combine(Directory.GetCurrentDirectory(), $"{"otherpaymentdata.json"}");
                            var json = File.ReadAllText(folderDetails);
                            dynamic jsonObj = JsonConvert.DeserializeObject(json);




                        

                        //Dictionary<string, object> objDict = new Dictionary<string, object>();
                            var i = 0;
                            foreach (var item in jsonObj["OtherPayment"])
                            {

                            objLisT.Add(
                           new SubPaymentMethodName()
                           {

                               Name= item["Name"],
                               PaymentNumber = item["PaymentNumber"],
                               LogoUrl = item["LogoUrl"],
                               MDRPercentage= item["MDRPercentage"]
                           });

                            //objLisT = item["Name"];
                            ////objDict.Add("Name", );
                            //    objDict.Add("PaymentNumber" + i, item["PaymentNumber"]);
                            //    objDict.Add("LogoUrl" + i, item["LogoUrl"]);
                                i++;
                            }
                            
                        }
                    pmModel.subPaymentMethodNames = objLisT;
                        model.PaymentMethods.Add(pmModel);
                    }
                    #endregion
                }

                //find a selected (previously) payment method
                var selectedPaymentMethodSystemName = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, _storeContext.CurrentStore.Id);
                if (!String.IsNullOrEmpty(selectedPaymentMethodSystemName))
                {
                    var paymentMethodToSelect = model.PaymentMethods.ToList()
                        .Find(pm => pm.PaymentMethodSystemName.Equals(selectedPaymentMethodSystemName, StringComparison.InvariantCultureIgnoreCase));
                    if (paymentMethodToSelect != null)
                        paymentMethodToSelect.Selected = true;
                }
                //if no option has been selected, let's do it for the first one
                if (model.PaymentMethods.FirstOrDefault(so => so.Selected) == null)
                {
                    var paymentMethodToSelect = model.PaymentMethods.FirstOrDefault();
                    if (paymentMethodToSelect != null)
                        paymentMethodToSelect.Selected = true;
                }
            //}
            //else
            //{
            //    model.StatusCode = (int)ErrorType.NotOk;
            //    model.ErrorList.Add(string.Format(THE_PRODUCT_IS_RESERVED, timeLimit));
            //}

            return model;
        }


        /// <summary>
        /// Prepare checkout completed model
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Checkout completed model</returns>
        public virtual CompleteResponseModel PrepareCheckoutCompletedModel(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            var model = new CompleteResponseModel
            {
                OrderId = order.Id,
                //OnePageCheckoutEnabled = _orderSettings.OnePageCheckoutEnabled,
                CustomOrderNumber = order.CustomOrderNumber
            };

            return model;
        }


        #endregion
    }
}

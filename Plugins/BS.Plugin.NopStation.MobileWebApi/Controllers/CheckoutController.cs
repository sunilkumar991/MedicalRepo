using BS.Plugin.NopStation.MobileApp.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Extensions.Authorize.Net;
using BS.Plugin.NopStation.MobileWebApi.Extensions.Paypal;
using BS.Plugin.NopStation.MobileWebApi.Factories;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Checkout;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Payment;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Checkout;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.ERPUpdate;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Order;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.PayPal;
using BS.Plugin.NopStation.MobileWebApi.PluginSettings;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Models.Checkout;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class CheckoutController : BaseApiController
    {
        #region field
        private IConfiguration configuration;
        private readonly IOrderModelFactoryApi _orderModelFactoryApi;
        private readonly ICheckoutModelFactoryApi _checkoutModelFactoryApi;
        private readonly IWorkContext _workContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICityService _cityService;
        private readonly ILocalizationService _localizationService;
        private readonly AddressSettings _addressSettings;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerService _customerService;
        private readonly ILogger _logger;
        private readonly IShippingService _shippingService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ShippingSettings _shippingSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly IPaymentService _paymentService;
        private readonly IWebHelper _webHelper;
        private readonly OrderSettings _orderSettings;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPluginFinder _pluginFinder;
        private readonly AuthorizeNetPaymentSettings _authorizeNetPaymentSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly IAddressService _addressService;
        private readonly IPaymentTransactionHistoryService _PaymentTransactionHistoryService;
        private readonly IDeviceService _deviceService;
        private readonly INotificationService _notificationService;
        private readonly IAddressModelFactoryApi _addressModelFactoryApi;
        private readonly ICustomerActivityService _customerActivityService;

        #endregion

        #region Ctor
        public CheckoutController(IOrderModelFactoryApi orderModelFactoryApi,
            ICheckoutModelFactoryApi checkoutModelFactoryApi,
            IWorkContext workContext,
            IStoreMappingService storeMappingService, IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService, IAddressAttributeFormatter addressAttributeFormatter,
            ICountryService countryService, IStateProvinceService stateProvinceService,
            ILocalizationService localizationService, AddressSettings addressSettings,
            IStoreContext storeContext, ICustomerService customerService,
            ILogger logger, IShippingService shippingService,
            IGenericAttributeService genericAttributeService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ITaxService taxService, ICurrencyService currencyService,
            IPriceFormatter priceFormatter, ShippingSettings shippingSettings,
            PaymentSettings paymentSettings, IPaymentService paymentService,
            IWebHelper webHelper, RewardPointsSettings rewardPointsSettings,
            OrderSettings orderSettings, IOrderService orderService,
            IOrderProcessingService orderProcessingService, IPluginFinder pluginFinder,
            AuthorizeNetPaymentSettings authorizeNetPaymentSettings,
            CurrencySettings currencySettings,
            IRewardPointService rewardPointService,
            IAddressService addressService,
            ICityService cityService,
            IPaymentTransactionHistoryService paymentTransactionHistoryService,
            IDeviceService deviceService,
            INotificationService notificationService,
            IAddressModelFactoryApi addressModelFactoryApi,
            ICustomerActivityService customerActivityService,
            IConfiguration iconfig
            )
        {
            _orderModelFactoryApi = orderModelFactoryApi;
            _checkoutModelFactoryApi = checkoutModelFactoryApi;
            _workContext = workContext;
            _storeMappingService = storeMappingService;
            _addressAttributeParser = addressAttributeParser;
            _addressAttributeService = addressAttributeService;
            _addressAttributeFormatter = addressAttributeFormatter;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _localizationService = localizationService;
            _addressSettings = addressSettings;
            _storeContext = storeContext;
            _customerService = customerService;
            _logger = logger;
            _shippingService = shippingService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _taxService = taxService;
            _currencyService = currencyService;
            _priceFormatter = priceFormatter;
            _shippingSettings = shippingSettings;
            _paymentSettings = paymentSettings;
            _paymentService = paymentService;
            _webHelper = webHelper;
            _rewardPointsSettings = rewardPointsSettings;
            _orderSettings = orderSettings;
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _pluginFinder = pluginFinder;
            _authorizeNetPaymentSettings = authorizeNetPaymentSettings;
            _currencySettings = currencySettings;
            _rewardPointService = rewardPointService;
            _genericAttributeService = genericAttributeService;
            _addressService = addressService;
            _cityService = cityService;
            _PaymentTransactionHistoryService = paymentTransactionHistoryService;
            _deviceService = deviceService;
            _notificationService = notificationService;
            _addressModelFactoryApi = addressModelFactoryApi;
            _customerActivityService = customerActivityService;
            configuration = iconfig;
        }
        #endregion

        #region Utility

        [NonAction]
        protected virtual bool IsMinimumOrderPlacementIntervalValid(Customer customer)
        {
            //prevent 2 orders being placed within an X seconds time frame
            if (_orderSettings.MinimumOrderPlacementInterval == 0)
            {
                return true;
            }

            Order lastOrder = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1)
                .FirstOrDefault();
            if (lastOrder == null)
            {
                return true;
            }

            TimeSpan interval = DateTime.UtcNow - lastOrder.CreatedOnUtc;
            return interval.TotalSeconds > _orderSettings.MinimumOrderPlacementInterval;
        }

        #endregion

        #region Action Method

        [Route("api/checkout/opccheckoutforguest")]
        [HttpGet]
        public IActionResult OpcCheckoutForGuest()
        {
            GeneralResponseModel<bool> model = new GeneralResponseModel<bool>();
            if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
            {
                model.Data = false;
            }
            else
            {
                model.Data = true;
            }
            return Ok(model);
        }

        [Route("api/checkout/billingform")]
        [HttpGet]
        public IActionResult OpcBillingForm()
        {
            CheckoutBillingAddressResponseModel billingAddressModel = _checkoutModelFactoryApi.PrepareBillingAddressModel(prePopulateNewAddressWithCustomerFields: true);
            return Ok(billingAddressModel);
        }

        [Route("api/checkout/checkoutsaveadressid/{addressType}")]
        [HttpPost]
        public IActionResult CheckoutSaveAddressId(int addressType, [FromBody]SingleValue value)
        {
            int.TryParse(value.Value, out int addressId);
            GeneralResponseModel<bool> result = new GeneralResponseModel<bool>();
            if (addressId > 0)
            {
                //existing address
                Address address = _workContext.CurrentCustomer.Addresses.FirstOrDefault(a => a.Id == addressId);
                if (address == null)
                {
                    throw new Exception("Address can't be loaded");
                }

                AddressType aT = (AddressType)addressType;

                if (aT == AddressType.Billing)
                {
                    _workContext.CurrentCustomer.BillingAddress = address;
                }
                else if (aT == AddressType.Shipping)
                {
                    _workContext.CurrentCustomer.ShippingAddress = address;
                    _genericAttributeService.SaveAttribute<PickupPoint>(_workContext.CurrentCustomer, NopCustomerDefaults.SelectedPickupPointAttribute, null, _storeContext.CurrentStore.Id);
                }
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                result.Data = true;

                //Added By Ankur on 19-Oct-2018 for EC-175
                City city = _cityService.GetCityById(address.CityId.Value);
                result.IsDeliveryAllowed = city.IsDeliveryAllowed;

            }
            else
            {
                result.StatusCode = (int)ErrorType.NotOk;
                result.Data = false;
                result.ErrorList = new List<string>
             {
                 "Address can't be loaded"
             };
            }
            return Ok(result);
        }

        [Route("api/checkout/checkoutsaveadress/{addressType}")]
        [HttpPost]
        public IActionResult CheckoutSaveAddress(int addressType, [FromBody]List<KeyValueApi> formValues)
        {
            GeneralResponseModel<bool> result = new GeneralResponseModel<bool>();
            NameValueCollection form = formValues.ToNameValueCollection();
            //////custom address attributes
            string customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
            IList<string> customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            AddressType aT = (AddressType)addressType;
            string prefix = HelperExtension.GetEnumDescription((AddressType)addressType);
            Address address = form.AddressFromToModel(prefix);

            foreach (string error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }
            ValidationExtension.AddressValidator(ModelState, address, _localizationService, _addressSettings, _stateProvinceService);
            if (ModelState.IsValid)
            {
                //var mainAddress = _addressService.FindAddress(_workContext.CurrentCustomer.Addresses.ToList(),
                //    address.FirstName, address.LastName, address.PhoneNumber,
                //    address.Email, address.FaxNumber, address.Company,
                //    address.Address1, address.Address2, // address.CityId,
                //    address.County, address.StateProvinceId, address.CityId, address.ZipPostalCode,
                //    address.CountryId, customAttributes, address.HouseNo, address.FloorNo, address.RoomNo, address.RopeColor, address.IsLiftOption);
                //if (mainAddress == null)

                if (address != null)  //address is not found. let's create a new one
                {
                    address.CustomAttributes = customAttributes;
                    address.CreatedOnUtc = DateTime.UtcNow;
                    // Added by Alexandar Rajavel on 07-Mar-2019
                    //Added code By Sunil Kumar for Noreplay_Email by Getting from DB on 08-04-2020
                    address.Email = string.IsNullOrEmpty(address.Email) ? _workContext.CurrentCustomer.Email ?? _localizationService.GetResource("Noreplay_Email") : address.Email;
                    //some validation
                    if (address.CountryId == 0)
                    {
                        address.CountryId = null;
                    }

                    if (address.StateProvinceId == 0)
                    {
                        address.StateProvinceId = null;
                    }

                    if (address.CountryId.HasValue && address.CountryId.Value > 0)
                    {
                        address.Country = _countryService.GetCountryById(address.CountryId.Value);
                    }
                    if (address.Id == 0)
                    {
                        _workContext.CurrentCustomer.Addresses.Add(address); //Add address
                    }
                    else
                    {
                        //Address add = _addressService.GetAddressById(address.Id);
                        //add = address;
                        _addressService.UpdateAddress(address); //Update address
                    }
                }

                if (aT == AddressType.Billing)
                {
                    //_workContext.CurrentCustomer.BillingAddress = mainAddress ?? address;
                    _workContext.CurrentCustomer.BillingAddress = address;
                }

                else if (aT == AddressType.Shipping)
                {
                    //_workContext.CurrentCustomer.ShippingAddress = mainAddress ?? address;
                    _workContext.CurrentCustomer.ShippingAddress = address;
                    _genericAttributeService.SaveAttribute<PickupPoint>(_workContext.CurrentCustomer, NopCustomerDefaults.SelectedPickupPointAttribute, null, _storeContext.CurrentStore.Id);
                }

                //Added By Ankur for Address Mapping on 21/9/2018
                if (address.Id == 0) //Add mapping for only new address
                {
                    _workContext.CurrentCustomer.CustomerAddressMappings.Add(new CustomerAddressMapping { Address = address });
                }
                else
                {
                    Address obj = _workContext.CurrentCustomer.Addresses.FirstOrDefault(x => x.Id == address.Id);
                    int index = _workContext.CurrentCustomer.Addresses.IndexOf(obj);
                    _workContext.CurrentCustomer.Addresses.Insert(index, address);
                }

                _customerService.UpdateCustomer(_workContext.CurrentCustomer);

                result.Data = true;

                //Added By Ankur on 19-Oct-2018 for EC-175
                City city = _cityService.GetCityById(address.CityId.Value);
                result.IsDeliveryAllowed = city.IsDeliveryAllowed;

            }
            //try to find an address with the same values (don't duplicate records)
            else
            {
                foreach (KeyValuePair<string, Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateEntry> state in ModelState)
                {
                    foreach (Microsoft.AspNetCore.Mvc.ModelBinding.ModelError error in state.Value.Errors)
                    {
                        result.ErrorList.Add(error.ErrorMessage);
                    }
                }
                result.Data = false;
                result.StatusCode = (int)ErrorType.NotOk;
            }
            return Ok(result);
        }

        [Route("api/checkout/checkoutgetshippingmethods")]
        [HttpGet]
        public IActionResult CheckoutGetShippingMethods()
        {
            List<ShoppingCartItem> cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();

            CheckoutShippingMethodResponseModel shippingMethodModel = _checkoutModelFactoryApi.PrepareShippingMethodModel(cart, _workContext.CurrentCustomer.ShippingAddress);
            return Ok(shippingMethodModel);
        }

        [Route("api/checkout/checkoutsetshippingmethod")]
        [HttpPost]
        public IActionResult CheckoutSetShippingMethods([FromBody]SingleValue value)
        {
            GeneralResponseModel<bool> result = new GeneralResponseModel<bool>();
            List<ShoppingCartItem> cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();

            string shippingoption = value.Value;
            if (string.IsNullOrEmpty(shippingoption))
            {
                throw new Exception("Selected shipping method can't be parsed");
            }

            string[] splittedOption = shippingoption.Split(new[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
            if (splittedOption.Length != 2)
            {
                throw new Exception("Selected shipping method can't be parsed");
            }

            string selectedName = splittedOption[0];
            string shippingRateComputationMethodSystemName = splittedOption[1];

            //find it
            //performance optimization. try cache first
            List<ShippingOption> shippingOptions = _genericAttributeService.GetAttribute<List<ShippingOption>>(_workContext.CurrentCustomer,
                NopCustomerDefaults.OfferedShippingOptionsAttribute, _storeContext.CurrentStore.Id);
            if (shippingOptions == null || !shippingOptions.Any())
            {
                //not found? let's load them using shipping service
                shippingOptions = _shippingService
                    .GetShippingOptions(cart, _workContext.CurrentCustomer.ShippingAddress, _workContext.CurrentCustomer, shippingRateComputationMethodSystemName, _storeContext.CurrentStore.Id)
                    .ShippingOptions
                    .ToList();
            }
            else
            {
                //loaded cached results. let's filter result by a chosen shipping rate computation method
                shippingOptions = shippingOptions.Where(so => so.ShippingRateComputationMethodSystemName.Equals(shippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
            }

            ShippingOption shippingOption = shippingOptions
                .Find(so => !string.IsNullOrEmpty(so.Name) && so.Name.Equals(selectedName, StringComparison.InvariantCultureIgnoreCase));
            if (shippingOption == null)
            {
                throw new Exception("Selected shipping method can't be loaded");
            }

            //save
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, NopCustomerDefaults.SelectedShippingOptionAttribute, shippingOption, _storeContext.CurrentStore.Id);
            result.Data = true;
            return Ok(result);
        }

        [Route("api/checkout/checkoutgetpaymentmethod")]
        [HttpGet]
        public IActionResult CheckoutGetPaymentMethods()
        {
            //var cart = _workContext.CurrentCustomer.ShoppingCartItems
            //         .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
            //         .LimitPerStore(_storeContext.CurrentStore.Id)
            //         .ToList();

            List<ShoppingCartItem> cart = (from _pushlst in _workContext.CurrentCustomer.ShoppingCartItems
                                           where _pushlst.ShoppingCartType == ShoppingCartType.ShoppingCart && _pushlst.StoreId == _storeContext.CurrentStore.Id
                                           select _pushlst).ToList();

            int filterByCountryId = 60;
            //if (_addressSettings.CountryEnabled &&
            //    _workContext.CurrentCustomer.BillingAddress != null &&
            //    _workContext.CurrentCustomer.BillingAddress.Country != null)
            //{
            //    filterByCountryId = _workContext.CurrentCustomer.BillingAddress.Country.Id;
            //}

            //payment is required
            CheckoutPaymentMethodResponseModel paymentMethodModel = _checkoutModelFactoryApi.PreparePaymentMethodModel(cart, filterByCountryId);
            //paymentMethodModel.PaymentMethods

            return Ok(paymentMethodModel);
        }

        [Route("api/checkout/checkoutsavepaymentmethod")]
        [HttpPost]
        public IActionResult OpcSavePaymentMethod([FromBody]CheckoutSavePaymentMethodQueryModel checkoutmodel)
        {
            try
            {
                //validation
                //var cart = _workContext.CurrentCustomer.ShoppingCartItems
                //    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                //    .LimitPerStore(_storeContext.CurrentStore.Id)
                //    .ToList();

                List<ShoppingCartItem> cart = (from _pushlst in _workContext.CurrentCustomer.ShoppingCartItems
                                               where _pushlst.ShoppingCartType == ShoppingCartType.ShoppingCart && _pushlst.StoreId == _storeContext.CurrentStore.Id
                                               select _pushlst).ToList();

                if (cart.Count == 0)
                {
                    throw new Exception("Your cart is empty");
                }

                string paymentmethod = checkoutmodel.PaymentMethod;

                string SubPaymetnType =string.Empty;
                //payment method 
                if (string.IsNullOrEmpty(paymentmethod))
                {
                    throw new Exception("Selected payment method can't be parsed");
                }

                //if (paymentmethod == "Payments.2C2P")
                //{
                    SubPaymetnType = checkoutmodel.SubPaymetnType;
                //}
                

                CheckoutPaymentMethodModel model = new CheckoutPaymentMethodModel();
                //TryUpdateModel(model);

                //reward points
                //if (_rewardPointsSettings.Enabled)
                //{
                //    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                //        NopCustomerDefaults.UseRewardPointsDuringCheckoutAttribute, model.UseRewardPoints,
                //        _storeContext.CurrentStore.Id);
                //}

                //var paymentMethodInst = _paymentService.LoadPaymentMethodBySystemName(paymentmethod);
                //if (paymentMethodInst == null ||
                //    !_paymentService.IsPaymentMethodActive(paymentMethodInst) ||
                //    !_pluginFinder.AuthenticateStore(paymentMethodInst.PluginDescriptor, _storeContext.CurrentStore.Id))
                //    throw new Exception("Selected payment method can't be parsed");

                //save
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, paymentmethod, _storeContext.CurrentStore.Id);


                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
               "SubPaymetnType", SubPaymetnType, _storeContext.CurrentStore.Id);


                //Added by Sunil at 29-04-19
                //Updating Version code 
                _workContext.CurrentCustomer.VersionCode = checkoutmodel.VersionCode != 0 ? checkoutmodel.VersionCode : _workContext.CurrentCustomer.VersionCode;
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);

                GeneralResponseModel<bool> response = new GeneralResponseModel<bool>()
                {
                    Data = true,
                    VersionCode = _workContext.CurrentCustomer.VersionCode,
                };

                return Ok(response);

            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return Json(new { error = 1, message = exc.Message });
            }
        }

        [Route("api/v1/checkout/checkoutsavepaymentmethod")]
        [HttpPost]
        public IActionResult V1OpcSavePaymentMethod([FromBody]CheckoutSavePaymentMethodQueryModel checkoutmodel)
        {
            try
            {
                //validation
                //var cart = _workContext.CurrentCustomer.ShoppingCartItems
                //    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                //    .LimitPerStore(_storeContext.CurrentStore.Id)
                //    .ToList();

                List<ShoppingCartItem> cart = (from _pushlst in _workContext.CurrentCustomer.ShoppingCartItems
                                               where _pushlst.ShoppingCartType == ShoppingCartType.ShoppingCart && _pushlst.StoreId == _storeContext.CurrentStore.Id
                                               select _pushlst).ToList();

                if (cart.Count == 0)
                {
                    throw new Exception("Your cart is empty");
                }

                string paymentmethod = checkoutmodel.PaymentMethod;

                string SubPaymetnType = string.Empty;
                //payment method 
                if (string.IsNullOrEmpty(paymentmethod))
                {
                    throw new Exception("Selected payment method can't be parsed");
                }

                //if (paymentmethod == "Payments.2C2P")
                //{
                SubPaymetnType = checkoutmodel.SubPaymetnType;
                //}


                CheckoutPaymentMethodModel model = new CheckoutPaymentMethodModel();
                //TryUpdateModel(model);

                //reward points
                //if (_rewardPointsSettings.Enabled)
                //{
                //    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                //        NopCustomerDefaults.UseRewardPointsDuringCheckoutAttribute, model.UseRewardPoints,
                //        _storeContext.CurrentStore.Id);
                //}

                //var paymentMethodInst = _paymentService.LoadPaymentMethodBySystemName(paymentmethod);
                //if (paymentMethodInst == null ||
                //    !_paymentService.IsPaymentMethodActive(paymentMethodInst) ||
                //    !_pluginFinder.AuthenticateStore(paymentMethodInst.PluginDescriptor, _storeContext.CurrentStore.Id))
                //    throw new Exception("Selected payment method can't be parsed");

                //save
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, paymentmethod, _storeContext.CurrentStore.Id);


                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
               "SubPaymetnType", SubPaymetnType, _storeContext.CurrentStore.Id);


                //Added by Sunil at 29-04-19
                //Updating Version code 
                _workContext.CurrentCustomer.VersionCode = checkoutmodel.VersionCode != 0 ? checkoutmodel.VersionCode : _workContext.CurrentCustomer.VersionCode;
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);

                GeneralResponseModel<bool> response = new GeneralResponseModel<bool>()
                {
                    Data = true,
                    VersionCode = _workContext.CurrentCustomer.VersionCode,
                };

                return Ok(response);

            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return Json(new { error = 1, message = exc.Message });
            }
        }



        /// <summary>
        /// Added PickupAddressId as parameter for Complete API APP will pass selected Id of address else 0
        /// Changed By : Ankur Shrivastava on 26-10-2018 for EC-181
        /// </summary>
        /// <param name="PickupAddressId"></param>
        /// <returns></returns>
        [Route("api/checkout/checkoutcomplete")]
        [HttpGet]
        public IActionResult Complete()
        {

            CompleteResponseModel result = new CompleteResponseModel
            {
                ErrorList = new List<string>()
            };
            try
            {
                //string pickupPointId
                //_genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, NopCustomerDefaults.PickupPointAddressId, pickupPointId, _storeContext.CurrentStore.Id);

                List<ShoppingCartItem> cart = _workContext.CurrentCustomer.ShoppingCartItems
                   .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                   .LimitPerStore(_storeContext.CurrentStore.Id)
                   .ToList();
                if (cart.Count == 0)
                {
                    throw new Exception("Your cart is empty");
                }

                //prevent 2 orders being placed within an X seconds time frame
                if (!IsMinimumOrderPlacementIntervalValid(_workContext.CurrentCustomer))
                {
                    throw new Exception(_localizationService.GetResource("Checkout.MinOrderPlacementInterval"));
                }

                //place order
                //var paymentMethodSystemName = "Payments.CheckMoneyOrder"; // Just for testing

                string paymentMethodSystemName = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer, NopCustomerDefaults.SelectedPaymentMethodAttribute, _storeContext.CurrentStore.Id);


                IPaymentMethod paymentMethodget = _paymentService.LoadPaymentMethodBySystemName(paymentMethodSystemName);
                if (paymentMethodget == null)
                {
                    throw new Exception("Payment method is not selected");
                }

                ProcessPaymentRequest processPaymentRequest = new ProcessPaymentRequest
                {
                    StoreId = _storeContext.CurrentStore.Id,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    PaymentMethodSystemName = paymentMethodSystemName
                };

                PlaceOrderResult placeOrderResult = _orderProcessingService.PlaceOrder(processPaymentRequest);
                if (placeOrderResult.Success)
                {
                    PostProcessPaymentRequest postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = placeOrderResult.PlacedOrder,
                        Orders = placeOrderResult.PlacedOrders // Added By ankur for multiple order EC-151 jira Ticket
                    };

                    //Create list of Ids and Custom Order Numbers Added By ankur for multiple order EC-151
                    List<int> OrderIds = new List<int>();
                    List<string> CustomOrderNumbers = new List<string>();
                    foreach (Order o in placeOrderResult.PlacedOrders)
                    {
                        result.OrderGroupNumber = o.OrderGroupNumber; // Added By ankur for multiple order EC-151 jira Ticket
                        OrderIds.Add(o.Id);
                        CustomOrderNumbers.Add(o.CustomOrderNumber);
                    }

                    IPaymentMethod paymentMethod = _paymentService.LoadPaymentMethodBySystemName(placeOrderResult.PlacedOrder.PaymentMethodSystemName);
                    if (paymentMethod == null)
                    {
                        //payment method could be null if order total is 0
                        //success
                        throw new Exception("OrderTotal 0");
                    }

                    //success
                    //result.Data = postProcessPaymentRequest.Order.Id;
                    if (paymentMethodSystemName == "Payments.PayPalStandard")
                    {
                        result.CompleteOrder = false;
                        result.OrderId = postProcessPaymentRequest.Order.Id;
                        result.OrderIds = OrderIds; // Added By ankur for multiple order EC-151
                        result.CustomOrderNumbers = CustomOrderNumbers; // Added By ankur for multiple order EC-151
                        result.PayPal = new CompleteResponseModel.PaypalModel
                        {
                            ClientId = PayPalExtension.ClientId
                        };
                        result.PaymentType = (int)PaymentType.PayPal;
                        //result.Orders = placeOrderResult.PlacedOrders;

                    }
                    else if (paymentMethodSystemName == "Payments.AuthorizeNet")
                    {
                        result.CompleteOrder = false;
                        result.OrderId = postProcessPaymentRequest.Order.Id;
                        result.OrderIds = OrderIds; // Added By ankur for multiple order EC-151
                        result.CustomOrderNumbers = CustomOrderNumbers; // Added By ankur for multiple order EC-151
                        result.PaymentType = (int)PaymentType.AuthorizeDotNet;
                        //result.Orders = placeOrderResult.PlacedOrders;
                    }
                    else if (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection)
                    {
                        result.CompleteOrder = false;
                        result.OrderId = postProcessPaymentRequest.Order.Id;
                        result.OrderIds = OrderIds; // Added By ankur for multiple order EC-151
                        result.CustomOrderNumbers = CustomOrderNumbers; // Added By ankur for multiple order EC-151
                        result.PaymentType = (int)PaymentType.ReDirectType;
                        //result.Orders = placeOrderResult.PlacedOrders;
                    }
                    else
                    {
                        result.CompleteOrder = true;
                        result.OrderId = postProcessPaymentRequest.Order.Id;
                        result.OrderIds = OrderIds; // Added By ankur for multiple order EC-151
                        result.CustomOrderNumbers = CustomOrderNumbers; // Added By ankur for multiple order EC-151
                        result.PaymentType = (int)PaymentType.CashOnDelivery;
                        //result.Orders = placeOrderResult.PlacedOrders;
                    }
                    return Ok(result);
                }

                //error
                result.StatusCode = (int)ErrorType.NotOk;

                foreach (string error in placeOrderResult.Errors)
                {
                    result.ErrorList.Add(error);
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                result.StatusCode = (int)ErrorType.NotOk;
                result.ErrorList.Add(e.Message);
                return Ok(result);
            }
        }

        /// <summary>
        /// For update payment status
        /// Created By : Alexandar Rajavel
        /// Created On : 23-Sep-2018
        /// Updated By : Ankur Shrivastava on 11-Oct-2018 for multiple order payment status update... For EC-151
        /// </summary>
        /// <param name="paymentStatus">Model</param>
        /// <returns>Result</returns>
        [Route("api/checkout/UpdatePaymentStatus")]
        [HttpPost]
        public IActionResult UpdatePaymentStatus([FromBody]UpdatePaymentStatus paymentStatus)
        {
            OrderStatusResponseModel result = new OrderStatusResponseModel
            {
                ErrorList = new List<string>()
            };
            try
            {
                if (paymentStatus.OrderId == 0)
                {
                    throw new Exception("Your order not completed");
                }
                //mark order as paid
                if (string.IsNullOrWhiteSpace(paymentStatus.TransactionStatus))
                {
                    throw new Exception("Please provide the TransactionStatus");
                }

                //var ord = _orderService.GetOrderItemById(paymentStatus.OrderId);
                Order order = _orderService.GetOrderById(paymentStatus.OrderId);
                //var address = _addressService.GetAddressById(order.PickUpInStore ? order.PickupAddressId ?? 0 : order.BillingAddressId);
                Address address = order.PickUpInStore ? order.PickupAddress : order.BillingAddress;

                if (address == null)
                {
                    throw new Exception("Your address is empty");
                }

                result.OrderGroupNumber = order.OrderGroupNumber.ToString();
                result.ReferenceNumber = paymentStatus.TransactionId;
                result.PhoneNumber = order.BillingAddress.PhoneNumber;
                result.ShippingAddress = _addressModelFactoryApi.AddressConcatenate(address);
                //Get all order of that order group number
                List<Order> orders = _orderService.GetOrdersByOrderGroupNumber(order.CustomerId, order.OrderGroupNumber);

                //Calculate to variables
                decimal totalproductAmount = 0;
                decimal totalTaxAmount = 0;
                decimal totalPaidAmount = 0;
                decimal totalshippingCharges = 0;


                //List<CheckoutOrderDetailJsonModel> checkoutOrderDetailJsons = new List<CheckoutOrderDetailJsonModel>();
                List<OrderDetailsResponseModel> orderDetailsResponseModels = new List<OrderDetailsResponseModel>();

                bool isOrderSuccess = false;
                foreach (Order o in orders)
                {
                    /*
                    //Set order detail for response object
                    CheckoutOrderDetailJsonModel detailJsonModel = new CheckoutOrderDetailJsonModel();
                    detailJsonModel.OrderId = o.Id;
                    detailJsonModel.CustomOrderNumber = o.CustomOrderNumber;
                    detailJsonModel.ExpectedDeliveryDate = o.ExpectedDeliveryDate != null ? o.ExpectedDeliveryDate.Value.ToString("dddd, dd MMMM yyyy") : "";
                    checkoutOrderDetailJsons.Add(detailJsonModel);
                    */

                    totalproductAmount += o.OrderSubtotalExclTax;
                    totalTaxAmount += o.OrderTax;// Added by Alexandar Rajavel on 28-Feb-2019
                    totalPaidAmount += o.OrderTotal;
                    totalshippingCharges += o.OrderShippingInclTax;

                    //Getting order details for PDF generation add by Ankur On 18 Oct. 2018
                    OrderDetailsResponseModel orderDetails = new OrderDetailsResponseModel();
                    orderDetails = _orderModelFactoryApi.PrepareOrderDetailsModel(o);
                    orderDetailsResponseModels.Add(orderDetails);

                    //Update payment status of each order
                    if (paymentStatus.TransactionStatus.ToLower() == "success")
                    {
                        if (_orderProcessingService.CanMarkOrderAsPaid(o))
                        {
                            o.AuthorizationTransactionId = paymentStatus.TransactionId;
                            _orderService.UpdateOrder(o);
                            _orderProcessingService.MarkOrderAsPaid(o);
                            isOrderSuccess = true;
                        }
                    }
                }
                if (isOrderSuccess)
                {
                    IList<Domain.Device> deviceDetails = _deviceService.GetDevicesByCustomerId(order.CustomerId);
                    if (deviceDetails.Any())
                    {
                        QueuedNotification notification = new QueuedNotification()
                        {
                            DeviceType = (DeviceType)deviceDetails[0].DeviceType,
                            SubscriptionId = deviceDetails[0].SubscriptionId,
                            Message = _localizationService.GetResource("Notification.Success"),
                        };
                        //_queuedNotificationApiService.SendNotication(notification);
                        _notificationService.SendNotication(notification);
                    }
                }

                result.ProductAmount = _priceFormatter.FormatPrice(totalproductAmount);
                result.TotalTax = _priceFormatter.FormatPrice(totalTaxAmount);
                result.TotalPaidAmount = _priceFormatter.FormatPrice(totalPaidAmount);
                result.ShippingCharges = _priceFormatter.FormatPrice(totalshippingCharges);
                //result.Orders = checkoutOrderDetailJsons;
                result.Orders = orderDetailsResponseModels;
                return Ok(result);
            }
            catch (Exception e)
            {
                result.StatusCode = (int)ErrorType.NotOk;
                result.ErrorList.Add(e.Message);
                return Ok(result);
            }
        }

        [Route("api/checkout/checkpaypalaccount")]
        [HttpPost]
        public async Task<IActionResult> CheckPayPalAccountAsync([FromBody]PayPalResponseModel payPal)
        {
            string paymentId = payPal.PaymentId;
            int id = payPal.OrderId;
            Order order = _orderService.GetOrderById(payPal.OrderId);

            PayPalExtension paypalExtension = new PayPalExtension();
            PaypalDetailModel payDetail = await paypalExtension.GetAmountAsync(paymentId);

            CompleteResponseModel result = new CompleteResponseModel
            {
                StatusCode = (int)ErrorType.NotOk,
                OrderId = id,
                CompleteOrder = false
            };

            if (payDetail != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Paypal PDT: From Mobile");
                sb.AppendLine("mc_gross: " + payDetail.Total);
                sb.AppendLine("Payment status: " + payDetail.PaymentStatus);
                sb.AppendLine("mc_currency: " + payDetail.Currency);
                sb.AppendLine("payer_id: " + payDetail.PayeeId);
                sb.AppendLine("Done From Mobile");
                decimal total = Convert.ToDecimal(payDetail.Total);

                //order note
                order.OrderNotes.Add(new OrderNote
                {
                    Note = sb.ToString(),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow,

                });
                _orderService.UpdateOrder(order);

                if (Math.Round(total).Equals(Math.Round(order.OrderTotal, 2)))
                {
                    if (_orderProcessingService.CanMarkOrderAsPaid(order))
                    {
                        order.AuthorizationTransactionId = paymentId;
                        _orderService.UpdateOrder(order);

                        _orderProcessingService.MarkOrderAsPaid(order);
                        result.StatusCode = (int)ErrorType.Ok;
                        result.CompleteOrder = true;
                    }
                    else
                    {
                        result.ErrorList.Add("Orrder Already Paid");
                    }

                }
                result.ErrorList.Add("Total not match");
            }

            return Ok(result);
        }

        [Route("api/checkout/checkauthorizepayment")]
        [HttpPost]
        public IActionResult CheckAuthorizePayment([FromBody]AuthorizeQueryModel authorizeNet)
        {
            CompleteResponseModel result = new CompleteResponseModel
            {
                StatusCode = (int)ErrorType.NotOk,
                OrderId = authorizeNet.OrderId
            };
            Order order = _orderService.GetOrderById(authorizeNet.OrderId);
            result.CompleteOrder = false;

            ProcessPaymentResult excuteResult = AuthorizeNetExtention.ExcuteTransaction(authorizeNet, _authorizeNetPaymentSettings, _currencyService,
                _currencySettings, _workContext.CurrentCustomer, _orderService);

            if (excuteResult.Errors.Count == 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Authorize.net PDT: From Mobile");
                sb.AppendLine("mc_gross: " + order.OrderTotal);
                sb.AppendLine("Payment status: " + excuteResult.NewPaymentStatus);
                sb.AppendLine("payer_id: " + excuteResult.AuthorizationTransactionId);
                sb.AppendLine("Done From Mobile");

                //order note
                order.OrderNotes.Add(new OrderNote
                {
                    Note = sb.ToString(),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow,

                });
                _orderService.UpdateOrder(order);

                if (_orderProcessingService.CanMarkOrderAsPaid(order))
                {

                    order.AuthorizationTransactionId = excuteResult.AuthorizationTransactionId;
                    _orderService.UpdateOrder(order);
                    if (excuteResult.NewPaymentStatus == PaymentStatus.Authorized)
                    {
                        if (_orderProcessingService.CanMarkOrderAsAuthorized(order))
                        {
                            _orderProcessingService.MarkAsAuthorized(order);
                            result.StatusCode = (int)ErrorType.Ok;
                            result.CompleteOrder = true;
                        }
                    }
                    else if (excuteResult.NewPaymentStatus == PaymentStatus.Paid)
                    {
                        _orderProcessingService.MarkOrderAsPaid(order);
                        result.StatusCode = (int)ErrorType.Ok;
                        result.CompleteOrder = true;
                    }

                }
                else
                {
                    result.ErrorList.Add("Order Already Paid");
                }
            }
            else
            {
                result.ErrorList.AddRange(excuteResult.Errors);
            }
            return Ok(result);
        }

        [Route("api/checkout/getcheckoutpickuppoints")]
        [HttpGet]
        public IActionResult GetCheckoutPickupPoints()
        {
            CheckoutPickupPointResponceModel model = new CheckoutPickupPointResponceModel
            {
                AllowPickUpInStore = _shippingSettings.AllowPickUpInStore
            };
            if (model.AllowPickUpInStore)
            {
                IList<Nop.Services.Shipping.Pickup.IPickupPointProvider> pickupPointProviders = _shippingService.LoadActivePickupPointProviders(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id);
                if (pickupPointProviders.Any())
                {
                    Nop.Services.Shipping.Pickup.GetPickupPointsResponse pickupPointsResponse = _shippingService.GetPickupPoints(_workContext.CurrentCustomer.BillingAddress, _workContext.CurrentCustomer, storeId: _storeContext.CurrentStore.Id);
                    if (pickupPointsResponse.Success)
                    {
                        model.PickupPoints = pickupPointsResponse.PickupPoints.Select(x =>
                        {
                            Country country = _countryService.GetCountryByTwoLetterIsoCode(x.CountryCode);
                            CheckoutPickupPointModelApi pickupPointModel = new CheckoutPickupPointModelApi
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Description = x.Description,
                                ProviderSystemName = x.ProviderSystemName,
                                Address = x.Address,
                                City = x.City.Name, //Changed by ankur on 31/8/2018
                                CountryName = country != null ? country.Name : string.Empty,
                                ZipPostalCode = x.ZipPostalCode,
                                Latitude = x.Latitude,//Added by Sunil Kumar at 9/1/19
                                Longitude = x.Longitude,//Added by Sunil Kumar at 9/1/19
                                OpeningHours = x.OpeningHours
                            };
                            if (x.PickupFee > 0)
                            {
                                decimal amount = _taxService.GetShippingPrice(x.PickupFee, _workContext.CurrentCustomer);
                                amount = _currencyService.ConvertFromPrimaryStoreCurrency(amount, _workContext.WorkingCurrency);
                                pickupPointModel.PickupFee = _priceFormatter.FormatShippingPrice(amount, true);
                            }
                            //Added by Sunil Kumar at 9/1/19
                            double distance = DistanceExtension.distance(Convert.ToDouble(x.Latitude), Convert.ToDouble(x.Longitude), Convert.ToDouble(_workContext.CurrentCustomer.BillingAddress.Latitude), Convert.ToDouble(_workContext.CurrentCustomer.BillingAddress.Longitude));
                            pickupPointModel.Distance = Convert.ToString(Math.Round(distance, 2)) + " KM";
                            return pickupPointModel;
                        }).ToList();
                    }
                    else
                    {
                        foreach (string error in pickupPointsResponse.Errors)
                        {
                            model.Warnings.Add(error);
                        }
                    }
                }


            }
            if (model.Warnings.Count > 0)
            {
                model.StatusCode = (int)ErrorType.NotOk;
            }
            else
            {
                model.StatusCode = (int)ErrorType.Ok;
            }
            return Ok(model);
        }

        [Route("api/checkout/getcheckoutpickuppoints/{Latitude}/{Longitude}")]
        [HttpGet]
        public IActionResult GetCheckoutPickupPoints(string Latitude, string Longitude)
        {
            CheckoutPickupPointResponceModel model = new CheckoutPickupPointResponceModel();

            bool latitudecheck = false, longitudecheck = false;
            if (decimal.TryParse(Latitude, out decimal value))
            {
                latitudecheck = true;
            }
            if (decimal.TryParse(Longitude, out value))
            {
                longitudecheck = true;
            }
            model.AllowPickUpInStore = _shippingSettings.AllowPickUpInStore;
            if (model.AllowPickUpInStore && latitudecheck && longitudecheck)
            {
                IList<Nop.Services.Shipping.Pickup.IPickupPointProvider> pickupPointProviders = _shippingService.LoadActivePickupPointProviders(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id);
                if (pickupPointProviders.Any())
                {
                    Nop.Services.Shipping.Pickup.GetPickupPointsResponse pickupPointsResponse = _shippingService.GetPickupPoints(_workContext.CurrentCustomer.BillingAddress, _workContext.CurrentCustomer, storeId: _storeContext.CurrentStore.Id);
                    if (pickupPointsResponse.Success)
                    {
                        model.PickupPoints = pickupPointsResponse.PickupPoints.Select(x =>
                        {
                            Country country = _countryService.GetCountryByTwoLetterIsoCode(x.CountryCode);
                            CheckoutPickupPointModelApi pickupPointModel = new CheckoutPickupPointModelApi
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Description = x.Description,
                                ProviderSystemName = x.ProviderSystemName,
                                Address = x.Address,
                                City = x.City.Name, //Changed by ankur on 31/8/2018
                                CountryName = country != null ? country.Name : string.Empty,
                                ZipPostalCode = x.ZipPostalCode,
                                Latitude = x.Latitude,
                                Longitude = x.Longitude,
                                OpeningHours = x.OpeningHours
                            };
                            if (x.PickupFee > 0)
                            {
                                decimal amount = _taxService.GetShippingPrice(x.PickupFee, _workContext.CurrentCustomer);
                                amount = _currencyService.ConvertFromPrimaryStoreCurrency(amount, _workContext.WorkingCurrency);
                                pickupPointModel.PickupFee = _priceFormatter.FormatShippingPrice(amount, true);
                            }
                            //Added by Sunil Kumar at 3/1/19
                            double distance = DistanceExtension.distance(Convert.ToDouble(x.Latitude), Convert.ToDouble(x.Longitude), Convert.ToDouble(Latitude.Trim()), Convert.ToDouble(Longitude.Trim()));
                            pickupPointModel.Distance = Convert.ToString(Math.Round(distance, 2)) + " KM";
                            return pickupPointModel;
                        }).ToList();
                    }
                    else
                    {
                        foreach (string error in pickupPointsResponse.Errors)
                        {
                            model.Warnings.Add(error);
                        }
                    }
                }
            }
            else if (latitudecheck == false)
            {
                model.Warnings.Add("Invalid Latitude Value");

            }
            else if (longitudecheck == false)
            {
                model.Warnings.Add("Invalid longitude Value");

            }
            if (model.Warnings.Count > 0)
            {
                model.StatusCode = (int)ErrorType.NotOk;
            }
            else
            {
                model.StatusCode = (int)ErrorType.Ok;
            }
            return Ok(model);
        }

        [Route("api/checkout/savecheckoutpickuppoint")]
        [HttpGet]
        public IActionResult SaveCheckoutPickupPoints(string pickupPointId)
        {
            GeneralResponseModel<bool> result = new GeneralResponseModel<bool>();
            try
            {
                if (_shippingSettings.AllowPickUpInStore)
                {

                    //no shipping address selected
                    //_workContext.CurrentCustomer.ShippingAddress = null;
                    //_customerService.UpdateCustomer(_workContext.CurrentCustomer);

                    //var pickupPoints = _shippingService
                    //    .GetPickupPoints(_workContext.CurrentCustomer.BillingAddress, null, storeId: _storeContext.CurrentStore.Id).PickupPoints.ToList();
                    //var selectedPoint = pickupPoints.FirstOrDefault(x => x.Id.Equals(pickupPointId));
                    //// var selectedPoint = _shippingService.GetPickupPoints(_workContext.CurrentCustomer.BillingAddress, pickupPoint[1], _storeContext.CurrentStore.Id).PickupPoints.Where(x=>x.Id == )
                    //if (selectedPoint == null)
                    //    throw new Exception("Pickup point is not allowed");

                    //var pickUpInStoreShippingOption = new ShippingOption
                    //{
                    //    Name = string.Format(_localizationService.GetResource("Checkout.PickupPoints.Name"), selectedPoint.Name),
                    //    Rate = selectedPoint.PickupFee,
                    //    Description = selectedPoint.Description,
                    //    ShippingRateComputationMethodSystemName = selectedPoint.ProviderSystemName
                    //};
                    //_genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, NopCustomerDefaults.SelectedShippingOptionAttribute, pickUpInStoreShippingOption, _storeContext.CurrentStore.Id);
                    //_genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, NopCustomerDefaults.SelectedPickupPointAttribute, selectedPoint, _storeContext.CurrentStore.Id);

                    //Save PickupPoint AddressId as Attribute against customer to be while placing order later  EC-181 | Ankur | 26-10-2018
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, NopCustomerDefaults.PickupPointAddressId, pickupPointId, _storeContext.CurrentStore.Id);

                }
                result.Data = true;
                result.StatusCode = (int)ErrorType.Ok;
            }
            catch (Exception exc)
            {

                result.Data = true;
                result.StatusCode = (int)ErrorType.NotOk;
                result.ErrorList.Add(exc.Message);
            }
            return Ok(result);

        }

        /// <summary>
        /// For Inserting Payment Transaction History
        /// Created By : Sunil Kumar S
        /// Created On : 09-Nov-2018
        /// </summary>
        /// <param name="SavePaymentTransactionHistory">Model</param>
        /// <returns>Result</returns>
        [Route("api/checkout/SavePaymentTransactionHistory")]
        [HttpPost]
        public IActionResult SavePaymentTransactionHistory([FromBody]SavePaymentTransactionHistory model)
        {
            GeneralResponseModel<bool> result = new GeneralResponseModel<bool>();
            try
            {
                PaymentTransactionHistory paymentStatus = new PaymentTransactionHistory() { CustomerId = _workContext.CurrentCustomer.Id, PaymentMethod = model.PaymentMethod, TransactionId = model.TransactionId, TransactionDescription = model.TransactionDescription, TransactionAmount = model.TransactionAmount };
                _PaymentTransactionHistoryService.InsertTransactionHistory(paymentStatus);
                result.Data = true;
                result.StatusCode = (int)ErrorType.Ok;
            }
            catch (Exception exc)
            {
                result.Data = false;
                result.StatusCode = (int)ErrorType.NotOk;
                result.ErrorList.Add(exc.Message);
            }
            return Ok(result);
        }

        /// <summary>
        /// Added  Method for Save Payment Transaction History and Checkout Complete With Update Payment Status
        /// Added By : Sunil Kumar S on 18-03-2019 modified at 22-03-19
        /// Updated By : Alexandar Rajavel on 17-May-2019 update payment status for multiple orders
        /// </summary>
        /// <param name="totalAmount">TotalAmount</param>
        /// <param name="paymentGatewayName">PaymentGatewayName</param>
        /// <param name="status">Status</param>
        /// <param name="transactionId">TransactionId</param>
        /// <returns>OrderStatusResponseModel</returns>
        [Route("api/checkout/checkoutcomplete")]
        [HttpPost]
        public IActionResult Checkoutcomplete([FromBody]SavePaymentTransactionHistory model)
        {
            ValidationExtension.SavePaymentTransactionHistoryValidator(ModelState, model, _localizationService);
            OrderStatusResponseModel result = new OrderStatusResponseModel
            {
                ErrorList = new List<string>()
            };

            try
            {
                if (ModelState.IsValid)
                {

                    #region SavePaymentTransactionHistory Function
                    //var isSuccess = model.TransactionStatusCode.ToLower().Contains("success");
                    int transactionStatus = 0;
                    bool isNewFlag = true;
                    switch (model.TransactionStatusCode)
                    {
                        case PAYMENT_STATUS_SUCCESS:
                            transactionStatus = (int)TransactionStatusType.Success;
                            isNewFlag = false;
                            break;
                        case PAYMENT_STATUS_PENDING:
                            transactionStatus = (int)TransactionStatusType.Pending;
                            break;
                        default:
                            transactionStatus = (int)TransactionStatusType.Failed;
                            break;
                    }

                    PaymentTransactionHistory paymentStatus = new PaymentTransactionHistory()
                    {
                        CustomerId = _workContext.CurrentCustomer.Id,
                        PaymentMethod = model.PaymentMethod,
                        TransactionId = model.TransactionId,
                        TransactionDescription = model.TransactionDescription,
                        TransactionAmount = model.TransactionAmount,
                        TransactionStatus = transactionStatus,
                        IsNew = isNewFlag,
                        IssueStatus = isNewFlag ? (int)IssueStatusType.Open : 0
                    };
                    //added code by Sunil Kumar at 25-04-2020 insert only success transaction History
                    if (transactionStatus == (int)TransactionStatusType.Success)
                    {
                        _PaymentTransactionHistoryService.InsertTransactionHistory(paymentStatus);
                    }
                    #endregion
                    if (model.TransactionStatusCode == PAYMENT_STATUS_SUCCESS || model.TransactionStatusCode == PAYMENT_STATUS_PENDING)// Added by Alexandar Rajavel on 28-Marc-2019
                    {
                        #region Checkout Complete Function

                        //var cart = _workContext.CurrentCustomer.ShoppingCartItems
                        //   .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                        //   .LimitPerStore(_storeContext.CurrentStore.Id)
                        //   .ToList();


                        List<ShoppingCartItem> cart = (from _pushlst in _workContext.CurrentCustomer.ShoppingCartItems
                                                       where _pushlst.ShoppingCartType == ShoppingCartType.ShoppingCart && _pushlst.StoreId == _storeContext.CurrentStore.Id
                                                       select _pushlst).ToList();




                        if (cart.Count == 0)
                        {
                            throw new Exception("Your cart is empty");
                        }

                        //prevent 2 orders being placed within an X seconds time frame
                        if (!IsMinimumOrderPlacementIntervalValid(_workContext.CurrentCustomer))
                        {
                            throw new Exception(_localizationService.GetResource("Checkout.MinOrderPlacementInterval"));
                        }

                        string paymentMethodSystemName = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer, NopCustomerDefaults.SelectedPaymentMethodAttribute, _storeContext.CurrentStore.Id);
                        string paymentmethodsubstype = string.Empty;
                       
                            paymentmethodsubstype = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer, "SubPaymetnType", _storeContext.CurrentStore.Id);
                        
                        IPaymentMethod paymentMethodget = _paymentService.LoadPaymentMethodBySystemName(paymentMethodSystemName);
                        if (paymentMethodget == null)
                        {
                            throw new Exception("Payment method is not selected");
                        }

                        ProcessPaymentRequest processPaymentRequest = new ProcessPaymentRequest
                        {
                            StoreId = _storeContext.CurrentStore.Id,
                            CustomerId = _workContext.CurrentCustomer.Id,
                            PaymentMethodSystemName = paymentMethodSystemName,
                            PaymentMethodSubType = paymentmethodsubstype
                        };

                        PlaceOrderResult placeOrderResult = _orderProcessingService.PlaceOrder(processPaymentRequest);
                        if (placeOrderResult.Success)
                        {
                            ////Added Code By Sunil Kumar at 30-04-2020 for Insert Transaction History Only After Transaction Success
                            //_PaymentTransactionHistoryService.InsertTransactionHistory(paymentStatus);
                            PostProcessPaymentRequest postProcessPaymentRequest = new PostProcessPaymentRequest
                            {
                                Order = placeOrderResult.PlacedOrder,
                                Orders = placeOrderResult.PlacedOrders // Added By ankur for multiple order EC-151 jira Ticket
                            };




                            #region Update Payment Status Function

                            if (postProcessPaymentRequest.Order.Id == 0)
                            {
                                throw new Exception("Your order not completed");
                            }
                            //mark order as paid
                            if (string.IsNullOrWhiteSpace(paymentStatus.TransactionDescription))
                            {
                                throw new Exception("Please provide the TransactionStatus");
                            }

                            //var order = _orderService.GetOrderById(postProcessPaymentRequest.Order.Id);
                            Order order = postProcessPaymentRequest.Order;
                            Address address = order.PickUpInStore ? order.PickupAddress : order.BillingAddress;

                            //decimal totaldiscount = 0;


                            if (address == null)
                            {
                                throw new Exception("Your address is empty");
                            }

                            result.OrderGroupNumber = order.OrderGroupNumber.ToString();
                            result.ReferenceNumber = paymentStatus.TransactionId;
                            result.PhoneNumber = order.BillingAddress.PhoneNumber;
                            result.ShippingAddress = _addressModelFactoryApi.ShippingAddressConcatenate(address);
                            //Added By Sunil Kumar for DiscountAmount in Order Details Page on 10-04-2020
                            result.DiscountAmount = _priceFormatter.FormatPrice(order.OrderDiscount);
                            //totaldiscount= order.OrderDiscount;

                            //Get all order of that order group number
                            //List<Order> orders = _orderService.GetOrdersByOrderGroupNumber(order.CustomerId, order.OrderGroupNumber);
                            List<Order> orders = postProcessPaymentRequest.Orders;

                            //Calculate to variables
                            decimal totalproductAmount = 0;
                            decimal totalTaxAmount = 0;
                            decimal totalPaidAmount = 0;
                            decimal totalshippingCharges = 0;

                            //List<CheckoutOrderDetailJsonModel> checkoutOrderDetailJsons = new List<CheckoutOrderDetailJsonModel>();
                            List<OrderDetailsResponseModel> orderDetailsResponseModels = new List<OrderDetailsResponseModel>();
                            string orderNumbers = string.Empty;
                            foreach (Order ord in orders)
                            {
                                totalproductAmount += ord.OrderSubtotalExclTax;
                                totalTaxAmount += ord.OrderTax;// Added by Alexandar Rajavel on 28-Feb-2019
                                totalPaidAmount += ord.OrderTotal;
                                totalshippingCharges += ord.OrderShippingInclTax;

                                //Getting order details for PDF generation add by Ankur On 18 Oct. 2018
                                OrderDetailsResponseModel orderDetails = new OrderDetailsResponseModel();
                                orderDetails = _orderModelFactoryApi.PrepareOrderDetailsModel(ord);
                                orderDetailsResponseModels.Add(orderDetails);

                                if (!ord.PickUpInStore)// Added by Alexandar Rajavel on 02-Nov-2019
                                {
                                    ord.ShippingStatusId = (int)ShippingStatus.NotYetShipped;
                                }

                                //Update payment status of each order
                                if (model.TransactionStatusCode == PAYMENT_STATUS_SUCCESS)
                                {
                                    ord.AuthorizationTransactionId = paymentStatus.TransactionId;
                                    ord.AuthorizationTransactionCode = model.TransactionStatusCode;
                                    ord.AuthorizationTransactionResult = paymentStatus.TransactionDescription;
                                    //_orderService.UpdateOrder(o);
                                    _orderProcessingService.MarkOrderAsPaid(ord);
                                    orderNumbers += ord.CustomOrderNumber + COMMA;
                                    //send order id to warehouse management
                                    //_notificationService.SendNoticationToWHM(ord);
                                }
                                else
                                {
                                    ord.AuthorizationTransactionId = paymentStatus.TransactionId;
                                    ord.AuthorizationTransactionCode = model.TransactionStatusCode;
                                    ord.AuthorizationTransactionResult = paymentStatus.TransactionDescription;
                                    ord.PaymentStatusId = (int)PaymentStatus.Pending;
                                    orderNumbers += ord.CustomOrderNumber + COMMA;
                                    _orderService.UpdateOrder(ord);
                                }
                                //Added code By Sunil Kumar at 01-04-2020 for Log
                                //var successorFailed = isOrderSuccess ? "Success" : "Failed";
                                //_customerActivityService.InsertActivity("PublicStore.OrderTransactionLog",
                                // string.Format(_localizationService.GetResource("ActivityLog.PublicStore.OrderTransactionLog"), order.Id, successorFailed, ord.AuthorizationTransactionId, ord.AuthorizationTransactionCode, ord.AuthorizationTransactionResult, ord.PaymentStatusId));


                            }
                            //commented on 13-03-2020 by Sunil Kumar
                            //if (isOrderSuccess)
                            //{
                            //IList<Domain.Device> deviceDetails = _deviceService.GetDevicesByCustomerId(order.CustomerId);
                            //if (deviceDetails.Any())
                            //{
                            //    QueuedNotification notification = new QueuedNotification()
                            //    {
                            //        DeviceType = (DeviceType)deviceDetails[0].DeviceType,
                            //        SubscriptionId = deviceDetails[0].SubscriptionId,
                            //        Message = _localizationService.GetResource("Notification.Success"),
                            //    };
                            //    //_queuedNotificationApiService.SendNotication(notification);
                            //    _notificationService.SendNotication(notification);
                            //}
                            //SMSRequest smsObj = new SMSRequest()
                            //{
                            //    DestinationNumber = order.BillingAddress.PhoneNumber.Remove(1, 3),
                            //    Message = string.Format(_localizationService.GetResource("Send_OrderNumber"), orderNumbers),
                            //    Application = _localizationService.GetResource("Application_Name")
                            //};
                            //Added Code By Sunil Kumar At 31-03-2020 for Sending Notification to Customer in between 9 PM to 6 Am Order
                            //var smsOrderNotesObj = new SMSRequest()
                            //{
                            //    DestinationNumber = order.BillingAddress.PhoneNumber.Remove(1, 3),
                            //    Message = string.Format(_localizationService.GetResource("Send_BePatient"), orderNumbers),
                            //    Application = _localizationService.GetResource("Application_Name")
                            //};
                            ////Send order number to customer via sms
                            ////_queuedNotificationApiService.SendSMS(smsObj

                            //TimeSpan start = TimeSpan.Parse("21:00"); // 9 PM
                            //TimeSpan end = TimeSpan.Parse("06:00");   // 6 AM
                            //TimeSpan orderTime = order.CreatedOnUtc.TimeOfDay;
                            //// start and stop times are in different days
                            //if (orderTime >= start || orderTime <= end)
                            //{
                            //    // orderTime time is between start and stop
                            //    _notificationService.SendSMS(smsOrderNotesObj);
                            //}
                            //else
                            //{
                            //_notificationService.SendSMS(smsObj);
                            //}
                            //}

                            result.ProductAmount = _priceFormatter.FormatPrice(totalproductAmount);
                            result.TotalTax = _priceFormatter.FormatPrice(totalTaxAmount);
                            result.TotalPaidAmount = _priceFormatter.FormatPrice(totalPaidAmount);
                            result.ShippingCharges = _priceFormatter.FormatPrice(totalshippingCharges);
                            result.Orders = orderDetailsResponseModels;
                            result.OrderStatus = model.TransactionStatusCode == PAYMENT_STATUS_SUCCESS ? "Success" : "Pending";

                            string enabledisableLogNew = configuration.GetValue<string>("EnableLog:Status");

                            if (enabledisableLogNew == "1")
                            {
                                string ERPCompanyID = configuration.GetValue<string>("ERPCompanyID:CompanyID");
                                string ERPBusinessID = configuration.GetValue<string>("ERPBusinessID:BusinessID");

                                //for Posting detials to erp server Added By Priti
                                List<POSSalesInvoiceDetailModel> orderItems = new List<POSSalesInvoiceDetailModel>();
                                foreach (Order placedOrders in placeOrderResult.PlacedOrders)
                                {
                                    foreach (OrderItem orderitem in placedOrders.OrderItems)
                                    {
                                        if (orderitem.Product.CGMItemID != null)
                                        {
                                            int mainquantity = 0;
                                            string mynumber = Regex.Replace(orderitem.AttributeDescription, @"\D", "");
                                            if (mynumber != null && mynumber != "")
                                            {
                                                if (Convert.ToInt32(mynumber) > 0)
                                                {
                                                    //mainquantity = orderitem.Quantity;
                                                    mainquantity = (Convert.ToInt32(mynumber) * orderitem.Quantity);
                                                }
                                                else
                                                {
                                                    mainquantity = orderitem.Quantity;
                                                }
                                            }
                                            else
                                            {
                                                mainquantity = orderitem.Quantity;
                                            }

                                            POSSalesInvoiceDetailModel orderItem = new POSSalesInvoiceDetailModel
                                            {
                                                ProductId = Convert.ToString(orderitem.Product.CGMItemID),
                                                Quantity = mainquantity,
                                                SellingRate = orderitem.UnitPriceInclTax,
                                                ecommid = orderitem.Id,

                                            };
                                            orderItems.Add(orderItem);
                                        }
                                    }

                                }

                                if (orderItems.Count > 0)
                                {
                                    POSSalesInvoiceHeaderModel processPaymentOrderDetail = new POSSalesInvoiceHeaderModel
                                    {
                                        CompanyId = ERPCompanyID,
                                        BusinessUnitId = ERPBusinessID,
                                        InvoiceDate = Convert.ToString(postProcessPaymentRequest.Order.CreatedOnUtc),

                                        TotalAmount = postProcessPaymentRequest.Order.OrderTotal,
                                        SalesInvoiceCode = Convert.ToString(postProcessPaymentRequest.Order.Id),
                                        Remarks = postProcessPaymentRequest.Order.Customer.BillingAddress.FirstName + postProcessPaymentRequest.Order.Customer.BillingAddress.LastName + "|" + postProcessPaymentRequest.Order.Customer.Username
                                    };

                                    RootObjectModel possalesorder = new RootObjectModel
                                    {
                                        POSSalesInvoiceHeaderModel = processPaymentOrderDetail,
                                        POSSalesInvoiceDetail = orderItems
                                    };

                                    string GETERPApi = configuration.GetValue<string>("ERPApi:Api");
                                    string json = JsonConvert.SerializeObject(possalesorder, Formatting.Indented);


                          //          string enabledisableLog = configuration.GetValue<string>("EnableLog:Status");

                          //          if (enabledisableLog == "1")
                          //          {
                          //              FileStream objFilestream = new FileStream(string.Format("{0}\\{1}", Path.GetTempPath(),
                          //"ConsoleCheckoutPage"), FileMode.Append, FileAccess.Write);
                          //              StreamWriter objStreamWriter = new StreamWriter(objFilestream);
                          //              objStreamWriter.WriteLine(json);
                          //              objStreamWriter.Close();
                          //              objFilestream.Close();
                          //          }



                                    string strProcessPaymentOrderDetail = Newtonsoft.Json.JsonConvert.SerializeObject(processPaymentOrderDetail);
                                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(GETERPApi);
                                    httpWebRequest.ContentType = "application/json";
                                    httpWebRequest.Method = "POST";
                                    using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                                    {
                                        streamWriter.Write(json);
                                    }
                                    System.Net.ServicePointManager.Expect100Continue = false;

                                    try
                                    {
                                        HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                    }

                                    catch (Exception)
                                    {

                              //          if (enabledisableLog == "1")
                              //          {
                              //              FileStream objFilestream = new FileStream(string.Format("{0}\\{1}", Path.GetTempPath(),
                              //"ConsoleCheckoutPage"), FileMode.Append, FileAccess.Write);
                              //              StreamWriter objStreamWriter = new StreamWriter(objFilestream);
                              //              objStreamWriter.WriteLine(ex.Message);
                              //              objStreamWriter.Close();
                              //              objFilestream.Close();
                              //          }

                                    }




                                }
                            }

                            IList<Domain.Device> deviceDetails = _deviceService.GetDevicesByCustomerId(order.CustomerId);
                            if (deviceDetails.Any())
                            {
                                QueuedNotification notification = new QueuedNotification()
                                {
                                    DeviceType = (DeviceType)deviceDetails[0].DeviceType,
                                    SubscriptionId = deviceDetails[0].SubscriptionId,
                                    Message = _localizationService.GetResource("Notification.Success"),
                                };
                                //_queuedNotificationApiService.SendNotication(notification);
                                _notificationService.SendNotication(notification);
                            }
                            SMSRequest smsObj = new SMSRequest()
                            {
                                DestinationNumber = order.BillingAddress.PhoneNumber.Remove(1, 3),
                                Message = string.Format(_localizationService.GetResource("Send_OrderNumber"), orderNumbers),
                                Application = _localizationService.GetResource("Application_Name")
                            };

                            _notificationService.SendSMS(smsObj);

                            // End

                            #endregion
                        }
                        else
                        {
                            //error
                            result.StatusCode = (int)ErrorType.NotOk;
                            foreach (string error in placeOrderResult.Errors)
                            {
                                result.ErrorList.Add(error);
                            }

                            return Ok(result);
                        }

                        #endregion
                    }
                    else
                    {
                        result.ErrorList.Add(model.TransactionDescription);
                        result.StatusCode = (int)ErrorType.NotOk;
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateEntry> state in ModelState)
                    {
                        foreach (Microsoft.AspNetCore.Mvc.ModelBinding.ModelError error in state.Value.Errors)
                        {
                            result.ErrorList.Add(error.ErrorMessage);
                        }
                    }
                    result.StatusCode = (int)ErrorType.NotOk;
                }
                return Ok(result);
            }

            catch (Exception exc)
            {
                result.StatusCode = (int)ErrorType.NotOk;
                result.ErrorList.Add(exc.Message);
                return Ok(result);
            }
        }
        #endregion


        /// <summary>
        /// For Inserting CGMOrderDetails
        /// Created By : Sunil S
        /// Created On : 06-04-2020
        /// </summary>
        /// <param name="CGMOrderDetails">CGMOrderDetails</param>
        /// <returns>Result</returns>
        [Route("api/checkout/CGMOrderDetails")]
        [HttpPost]
        public IActionResult CGMOrderDetails([FromBody]OrderDetailsModel model)
        {

            GeneralResponseModel<bool> result = new GeneralResponseModel<bool>();
            try
            {
                //var paymentStatus = new PaymentTransactionHistory() { CustomerId = _workContext.CurrentCustomer.Id, PaymentMethod = model.PaymentMethod, TransactionId = model.TransactionId, TransactionDescription = model.TransactionDescription, TransactionAmount = model.TransactionAmount };
                //_PaymentTransactionHistoryService.InsertTransactionHistory(paymentStatus);
                Order order = _orderService.GetOrderById(Convert.ToInt32(model.id));
                if (order != null)
                {
                    order.OrderGuid = new Guid(model.orderGuid);
                    order.CustomerId = Convert.ToInt32(model.customerId);
                    order.OrderTotal = Convert.ToDecimal(model.orderTotal);
                    order.OrderStatusId = 30;
                    order.PaymentStatusId = 30;
                    order.ShippingStatusId = 40;

                    // var orderItems = new OrderItem();
                    if (model.orderItems.Count() == order.OrderItems.Count())
                    {

                        foreach (OrderItem dbitem in order.OrderItems)
                        {
                            OrderItem orderItem = new OrderItem();
                            orderItem = dbitem;
                            OrderItemsModel modelOrderItemsdata = model.orderItems.FirstOrDefault(o => o.id == Convert.ToString(dbitem.Id));
                            orderItem.Quantity = Convert.ToInt32(modelOrderItemsdata.Quantity);
                            // order.OrderItems.Remove(orderItem);
                            order.OrderItems.Add(orderItem);
                        }

                        _orderService.UpdateOrder(order);
                        result.Data = true;
                        result.StatusCode = (int)ErrorType.Ok;
                    }
                    else
                    {
                        result.Data = false;
                        result.StatusCode = (int)ErrorType.NotOk;
                        result.ErrorList.Add("Order Items Count Mismatch");
                    }
                }
                else
                {
                    result.Data = false;
                    result.StatusCode = (int)ErrorType.NotOk;
                    result.ErrorList.Add("Order Not Found");
                }
            }
            catch (Exception exc)
            {
                result.Data = false;
                result.StatusCode = (int)ErrorType.NotOk;
                result.ErrorList.Add(exc.Message);
            }
            return Ok(result);
        }

    }
}

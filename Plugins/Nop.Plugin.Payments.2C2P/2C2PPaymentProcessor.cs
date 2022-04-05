using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Tax;

namespace Nop.Plugin.Payments._2C2P
{
    // Created by Alexandar Rajavel on 14-Mar-2019
    public class _2C2PPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICurrencyService _currencyService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ISettingService _settingService;
        private readonly ITaxService _taxService;
        private readonly IWebHelper _webHelper;
        private readonly _2C2PPaymentSettings _2C2PPaymentSettings;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerActivityService _customerActivityService;

        #endregion

        #region Constructor
        public _2C2PPaymentProcessor(CurrencySettings currencySettings,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICurrencyService currencyService,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ISettingService settingService,
            ITaxService taxService,
            IWebHelper webHelper,
            _2C2PPaymentSettings twoC2PPaymentSettings,
            IPaymentService paymentService,
            IOrderService orderService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ICustomerActivityService customerActivityService
            )
        {
            this._currencySettings = currencySettings;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._currencyService = currencyService;
            this._genericAttributeService = genericAttributeService;
            this._httpContextAccessor = httpContextAccessor;
            this._localizationService = localizationService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._settingService = settingService;
            this._taxService = taxService;
            this._webHelper = webHelper;
            this._2C2PPaymentSettings = twoC2PPaymentSettings;
            this._paymentService = paymentService;
            this._orderService = orderService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._customerActivityService = customerActivityService;
        }
        #endregion


        #region Utilities

        private string GetUniqueRefNumber()
        {
            return DateTime.Now.ToString("yyMMddHHmmss");
        }
        /// <summary>
        /// Gets 2C2P URL
        /// </summary>
        /// <returns></returns>
        private string Get2C2PUrl()
        {
            return _2C2PPaymentSettings.UseSandbox ? "https://demo2.2c2p.com/2C2PFrontEnd/RedirectV3/payment" : "https://t.2c2p.com/RedirectV3/payment";
        }

        /// <summary>
        /// Gets IPN 2C2P URL
        /// </summary>
        /// <returns></returns>
        private string GetIpn2C2PUrl()
        {
            return _2C2PPaymentSettings.UseSandbox ?
                "https://demo2.2c2p.com/2C2PFrontEnd/RedirectV3/payment" :
                "https://t.2c2p.com/RedirectV3/payment";
        }

        /// <summary>
        /// Verifies IPN
        /// </summary>
        /// <param name="formString">Form string</param>
        /// <param name="values">Values</param>
        /// <returns>Result</returns>
        public bool VerifyIpn(string formString, out Dictionary<string, string> values)
        {
            var req = (HttpWebRequest)WebRequest.Create(GetIpn2C2PUrl());
            req.Method = WebRequestMethods.Http.Post;
            req.ContentType = MimeTypes.ApplicationXWwwFormUrlencoded;
            //now PayPal requires user-agent. otherwise, we can get 403 error
            req.UserAgent = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.UserAgent];

            var formContent = $"cmd=_notify-validate&{formString}";
            req.ContentLength = formContent.Length;

            using (var sw = new StreamWriter(req.GetRequestStream(), Encoding.ASCII))
            {
                sw.Write(formContent);
            }

            string response;
            using (var sr = new StreamReader(req.GetResponse().GetResponseStream()))
            {
                response = WebUtility.UrlDecode(sr.ReadToEnd());
            }
            var success = response.Trim().Equals("VERIFIED", StringComparison.OrdinalIgnoreCase);

            values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var l in formString.Split('&'))
            {
                var line = l.Trim();
                var equalPox = line.IndexOf('=');
                if (equalPox >= 0)
                    values.Add(line.Substring(0, equalPox), line.Substring(equalPox + 1));
            }

            return success;
        }
        #endregion

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Payment2C2P/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        /// <returns>View component name</returns>
        public string GetPublicViewComponentName()
        {
            return "Payment2C2P";
        }

        /// <summary>
        /// Gets PDT details
        /// </summary>
        /// <param name="tx">TX</param>
        /// <param name="values">Values</param>
        /// <param name="response">Response</param>
        /// <returns>Result</returns>
        public bool GetPdtDetails(string tx, out Dictionary<string, string> values, out string response)
        {
            var req = (HttpWebRequest)WebRequest.Create(Get2C2PUrl());
            req.Method = WebRequestMethods.Http.Post;
            req.ContentType = MimeTypes.ApplicationXWwwFormUrlencoded;
            //now PayPal requires user-agent. otherwise, we can get 403 error
            req.UserAgent = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.UserAgent];

            var formContent = $"cmd=_notify-synch&at={_2C2PPaymentSettings.SecretKey}&tx={tx}";
            req.ContentLength = formContent.Length;

            using (var sw = new StreamWriter(req.GetRequestStream(), Encoding.ASCII))
                sw.Write(formContent);

            using (var sr = new StreamReader(req.GetResponse().GetResponseStream()))
                response = WebUtility.UrlDecode(sr.ReadToEnd());

            values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            bool firstLine = true, success = false;
            foreach (var l in response.Split('\n'))
            {
                var line = l.Trim();
                if (firstLine)
                {
                    success = line.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase);
                    firstLine = false;
                }
                else
                {
                    var equalPox = line.IndexOf('=');
                    if (equalPox >= 0)
                        values.Add(line.Substring(0, equalPox), line.Substring(equalPox + 1));
                }
            }

            return success;
        }


        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new _2C2PPaymentSettings
            {
                UseSandbox = true
            });

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.UseSandbox", "Use Sandbox");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.UseSandbox.Hint", "Check to enable Sandbox (testing environment).");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.Version", "Version");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.Version.Hint", "Specify 2C2P Version");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.MerchantId", "Merchant ID");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.MerchantId.Hint", "Specify merchant ID");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.SecretKey", "Secret Key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.SecretKey.Hint", "Specify your secret key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.ResultUrl", "Result Url");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.ResultUrl.Hint", "Specify your return url");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.Currency", "Currency");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.Currency.Hint", "Specify your currency code. i.e., Myanmar currency code: 104");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.PrivatePfxFilePath", "Private Pfx File Path");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.PrivatePfxFilePath.Hint", "Specify your private.pfx file path. i.e., D:/2C2P/private.pfx");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.PrivatePfxFilePassword", "Private Pfx File Password");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.PrivatePfxFilePassword.Hint", "Specify your private.pfx file's password");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.AdditionalFee", "Additional fee");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.AdditionalFee.Hint", "Enter additional fee to charge your customers.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.AdditionalFeePercentage", "Additional fee. Use percentage");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.PassProductNamesAndTotals", "Pass product names and order totals to PayPal");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.PassProductNamesAndTotals.Hint", "Check if product names and order totals should be passed to PayPal.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.RedirectionTip", "You will be redirected to 2C2P site to complete the order.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.PaymentMethodDescription", "You will be redirected to 2C2P site to complete the payment");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.RedirectAndPostDataURL", "Enter Redirect and post data URL");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payments.2C2P.Fields.RedirectAndPostDataURL.Hint", "Enter URL for Redirect and post data to 2C2P payment gateway");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<_2C2PPaymentSettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.UseSandbox");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.UseSandbox.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.Version");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.Version.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.MerchantId");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.MerchantId.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.SecretKey");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.SecretKey.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.ResultUrl");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.ResultUrl.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.Currency");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.Currency.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.PrivatePfxFilePath");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.PrivatePfxFilePath.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.PrivatePfxFilePassword");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.PrivatePfxFilePassword.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.AdditionalFee");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.AdditionalFee.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.AdditionalFeePercentage");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.AdditionalFeePercentage.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.PassProductNamesAndTotals");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.PassProductNamesAndTotals.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.PaymentMethodDescription");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.RedirectAndPostDataURL");
            _localizationService.DeletePluginLocaleResource("Plugins.Payments.2C2P.Fields.RedirectAndPostDataURL.Hint");

            base.Uninstall();
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return new CancelRecurringPaymentResult { Errors = new[] { "Recurring payment not supported" } };
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //let's ensure that at least 5 seconds passed after order is placed
            //P.S. there's no any particular reason for that. we just do it
            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds < 5)
                return false;

            return true;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            return new CapturePaymentResult { Errors = new[] { "Capture method not supported" } };
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return _paymentService.CalculateAdditionalFee(cart, _2C2PPaymentSettings.AdditionalFee, _2C2PPaymentSettings.AdditionalFeePercentage);
        }

        /// <summary>
        /// Get payment information
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>Payment info holder</returns>
        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            return new ProcessPaymentRequest();
        }

        //public void GetPublicViewComponent(out string viewComponentName)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return false;
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {

            //create common query parameters for the request
            var queryParameters = new Dictionary<string, string>();

            //whether to include order items in a transaction
            if (_2C2PPaymentSettings.PassProductNamesAndTotals)
            {
                //add order items query parameters to the request
                var parameters = new Dictionary<string, string>(queryParameters);
                AddItemsParameters(parameters, postProcessPaymentRequest);
            }

            //or add only an order total query parameters to the request
            AddOrderTotalParameters(queryParameters, postProcessPaymentRequest);

            //---------2C2P payment request---------------------
            var customerId = Convert.ToString(postProcessPaymentRequest.Order.CustomerId);
            var ordersTotalAmount = postProcessPaymentRequest.Order.OrderTotal;
            var secretKey = _2C2PPaymentSettings.SecretKey;
            var currency = _2C2PPaymentSettings.Currency;       // "104";
            var merchant_id = _2C2PPaymentSettings.MerchantId;  // "104104000000270";
            var version = _2C2PPaymentSettings.Version;         // "8.2";
            var result_url_1 = _2C2PPaymentSettings.ResultUrl;  // "http://localhost:30393/Home/About";
            //Added code at 10-12-19 by Sunil Kumar
            var amount = String.Format("{0:000000000000}", (Math.Round(ordersTotalAmount, 2)) * 100);// "000000000100";
            var order_id = GetUniqueRefNumber().Remove(0, customerId.Length + 1).Insert(0, customerId + "-");
            var strSignature = version + merchant_id + postProcessPaymentRequest.ProductListString + order_id + currency + amount + result_url_1;
            var hash_value = _2C2PHelper.getHMAC(strSignature, secretKey);

            var formPostText = @"<html><body><div>
                <form method=""POST"" action=""" + Get2C2PUrl() + @""" name=""formToPost2C2P"">
                <input type=""hidden"" name=""currency"" value=""" + currency + @""" /> 
                <input type=""hidden"" name=""merchant_id"" value=""" + merchant_id + @""" /> 
                <input type=""hidden"" name=""payment_description"" value=""" + postProcessPaymentRequest.ProductListString + @""" /> 
                <input type=""hidden"" name=""version"" value=""" + version + @""" /> 
                <input type=""hidden"" name=""result_url_1"" value=""" + result_url_1 + @""" /> 
                <input type=""hidden"" name=""hash_value"" value=""" + hash_value + @""" /> 
                <input type=""hidden"" name=""amount"" value=""" + amount + @""" /> 
                <input type=""hidden"" name=""order_id"" value=""" + order_id + @""" /> 
                </form></div><script type=""text/javascript"">document.formToPost2C2P.submit();</script></body></html>";
            _httpContextAccessor.HttpContext.Response.WriteAsync(formPostText);

            //Added Code By Sunil Kumar at 01-04-2020 for Log
            _customerActivityService.InsertActivity("PublicStore.2C2PPaymentRequest",
                                string.Format(_localizationService.GetResource("ActivityLog.PublicStore.2C2PPaymentRequest"), customerId, postProcessPaymentRequest.Order.Id, formPostText));

        }

        /// <summary>
        /// Create common query parameters for the request
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Created query parameters</returns>
        private IDictionary<string, string> CreateQueryParameters(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //get store location
            var storeLocation = _webHelper.GetStoreLocation();

            //create query parameters
            return new Dictionary<string, string>
            {
                //PayPal ID or an email address associated with your PayPal account
                //["business"] = _2C2PPaymentSettings.BusinessEmail,

                //the character set and character encoding
                ["charset"] = "utf-8",

                //set return method to "2" (the customer redirected to the return URL by using the POST method, and all payment variables are included)
                ["rm"] = "2",

                ["bn"] = _2C2PHelper.NopCommercePartnerCode,
                ["currency_code"] = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId)?.CurrencyCode,

                //order identifier
                ["invoice"] = postProcessPaymentRequest.Order.CustomOrderNumber,
                ["custom"] = postProcessPaymentRequest.Order.OrderGuid.ToString(),

                //PDT, IPN and cancel URL
                ["return"] = $"{storeLocation}Plugins/Payment2C2P/PDTHandler",
                ["notify_url"] = $"{storeLocation}Plugins/Payment2C2P/IPNHandler",
                ["cancel_return"] = $"{storeLocation}Plugins/Payment2C2P/CancelOrder",

                //shipping address, if exists
                ["no_shipping"] = postProcessPaymentRequest.Order.ShippingStatus == ShippingStatus.ShippingNotRequired ? "1" : "2",
                ["address_override"] = postProcessPaymentRequest.Order.ShippingStatus == ShippingStatus.ShippingNotRequired ? "0" : "1",
                ["first_name"] = postProcessPaymentRequest.Order.ShippingAddress?.FirstName,
                ["last_name"] = postProcessPaymentRequest.Order.ShippingAddress?.LastName,
                ["address1"] = postProcessPaymentRequest.Order.ShippingAddress?.Address1,
                ["address2"] = postProcessPaymentRequest.Order.ShippingAddress?.Address2,
                ["city"] = postProcessPaymentRequest.Order.ShippingAddress?.City?.Name,
                ["state"] = postProcessPaymentRequest.Order.ShippingAddress?.StateProvince?.Abbreviation,
                ["country"] = postProcessPaymentRequest.Order.ShippingAddress?.Country?.TwoLetterIsoCode,
                ["zip"] = postProcessPaymentRequest.Order.ShippingAddress?.ZipPostalCode,
                ["email"] = postProcessPaymentRequest.Order.ShippingAddress?.Email
            };

        }

        /// <summary>
        /// Add order items to the request query parameters
        /// </summary>
        /// <param name="parameters">Query parameters</param>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        private void AddItemsParameters(IDictionary<string, string> parameters, PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //upload order items
            parameters.Add("cmd", "_cart");
            parameters.Add("upload", "1");

            var cartTotal = decimal.Zero;
            var roundedCartTotal = decimal.Zero;
            var itemCount = 1;

            //add shopping cart items
            foreach (var item in postProcessPaymentRequest.Order.OrderItems)
            {
                var roundedItemPrice = Math.Round(item.UnitPriceExclTax, 2);

                //add query parameters
                parameters.Add($"item_name_{itemCount}", item.Product.Name);
                parameters.Add($"amount_{itemCount}", roundedItemPrice.ToString("0.00", CultureInfo.InvariantCulture));
                parameters.Add($"quantity_{itemCount}", item.Quantity.ToString());

                cartTotal += item.PriceExclTax;
                roundedCartTotal += roundedItemPrice * item.Quantity;
                itemCount++;
            }

            //add checkout attributes as order items
            var checkoutAttributeValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(postProcessPaymentRequest.Order.CheckoutAttributesXml);
            foreach (var attributeValue in checkoutAttributeValues)
            {
                var attributePrice = _taxService.GetCheckoutAttributePrice(attributeValue, false, postProcessPaymentRequest.Order.Customer);
                var roundedAttributePrice = Math.Round(attributePrice, 2);

                //add query parameters
                if (attributeValue.CheckoutAttribute != null)
                {
                    parameters.Add($"item_name_{itemCount}", attributeValue.CheckoutAttribute.Name);
                    parameters.Add($"amount_{itemCount}", roundedAttributePrice.ToString("0.00", CultureInfo.InvariantCulture));
                    parameters.Add($"quantity_{itemCount}", "1");

                    cartTotal += attributePrice;
                    roundedCartTotal += roundedAttributePrice;
                    itemCount++;
                }
            }

            //add shipping fee as a separate order item, if it has price
            var roundedShippingPrice = Math.Round(postProcessPaymentRequest.Order.OrderShippingExclTax, 2);
            if (roundedShippingPrice > decimal.Zero)
            {
                parameters.Add($"item_name_{itemCount}", "Shipping fee");
                parameters.Add($"amount_{itemCount}", roundedShippingPrice.ToString("0.00", CultureInfo.InvariantCulture));
                parameters.Add($"quantity_{itemCount}", "1");

                cartTotal += postProcessPaymentRequest.Order.OrderShippingExclTax;
                roundedCartTotal += roundedShippingPrice;
                itemCount++;
            }

            //add payment method additional fee as a separate order item, if it has price
            var roundedPaymentMethodPrice = Math.Round(postProcessPaymentRequest.Order.PaymentMethodAdditionalFeeExclTax, 2);
            if (roundedPaymentMethodPrice > decimal.Zero)
            {
                parameters.Add($"item_name_{itemCount}", "Payment method fee");
                parameters.Add($"amount_{itemCount}", roundedPaymentMethodPrice.ToString("0.00", CultureInfo.InvariantCulture));
                parameters.Add($"quantity_{itemCount}", "1");

                cartTotal += postProcessPaymentRequest.Order.PaymentMethodAdditionalFeeExclTax;
                roundedCartTotal += roundedPaymentMethodPrice;
                itemCount++;
            }

            //add tax as a separate order item, if it has positive amount
            var roundedTaxAmount = Math.Round(postProcessPaymentRequest.Order.OrderTax, 2);
            if (roundedTaxAmount > decimal.Zero)
            {
                parameters.Add($"item_name_{itemCount}", "Tax amount");
                parameters.Add($"amount_{itemCount}", roundedTaxAmount.ToString("0.00", CultureInfo.InvariantCulture));
                parameters.Add($"quantity_{itemCount}", "1");

                cartTotal += postProcessPaymentRequest.Order.OrderTax;
                roundedCartTotal += roundedTaxAmount;
                itemCount++;
            }

            if (cartTotal > postProcessPaymentRequest.Order.OrderTotal)
            {
                //get the difference between what the order total is and what it should be and use that as the "discount"
                var discountTotal = Math.Round(cartTotal - postProcessPaymentRequest.Order.OrderTotal, 2);
                roundedCartTotal -= discountTotal;

                //gift card or rewarded point amount applied to cart in nopCommerce - shows in PayPal as "discount"
                parameters.Add("discount_amount_cart", discountTotal.ToString("0.00", CultureInfo.InvariantCulture));
            }

            //save order total that actually sent to PayPal (used for PDT order total validation)
            _genericAttributeService.SaveAttribute(postProcessPaymentRequest.Order, _2C2PHelper.OrderTotalSentTo2C2P, roundedCartTotal);
        }


        /// <summary>
        /// Add order total to the request query parameters
        /// </summary>
        /// <param name="parameters">Query parameters</param>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        private void AddOrderTotalParameters(IDictionary<string, string> parameters, PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //round order total
            var roundedOrderTotal = Math.Round(postProcessPaymentRequest.Order.OrderTotal, 2);

            //save order total that actually sent to PayPal (used for PDT order total validation)
            _genericAttributeService.SaveAttribute(postProcessPaymentRequest.Order, _2C2PHelper.OrderTotalSentTo2C2P, roundedOrderTotal);
        }

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult();
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult { Errors = new[] { "Recurring payment not supported" } };
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            return new RefundPaymentResult { Errors = new[] { "Refund method not supported" } };
        }

        /// <summary>
        /// Validate payment form
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>List of validating errors</returns>
        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            return new List<string>();
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            return new VoidPaymentResult { Errors = new[] { "Void method not supported" } };
        }

        #region Properties

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get { return RecurringPaymentType.NotSupported; }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get { return PaymentMethodType.Redirection; }
        }

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        public string PaymentMethodDescription
        {
            //return description of this payment method to be display on "payment method" checkout step. good practice is to make it localizable
            //for example, for a redirection payment method, description may be like this: "You will be redirected to PayPal site to complete the payment"
            get { return _localizationService.GetResource("Plugins.Payments.2C2P.PaymentMethodDescription"); }
        }

        #endregion

    }
}

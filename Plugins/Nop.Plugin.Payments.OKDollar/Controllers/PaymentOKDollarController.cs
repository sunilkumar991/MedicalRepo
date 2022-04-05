using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Payments.OKDollar.Models;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.OKDollar.Controllers
{
    public class PaymentOKDollarController : BasePaymentController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPermissionService _permissionService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly PaymentSettings _paymentSettings;
        private readonly OKDollarPaymentSettings _oKDollarPaymentSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICustomerService _customerService;
        private readonly IPaymentTransactionHistoryService _paymentTransactionHistoryService;
        private readonly INotificationService _notificationService;
        private readonly ICustomerActivityService _customerActivityService;

        #endregion

        #region Constructor
        public PaymentOKDollarController(IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            IPaymentService paymentService,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            IPermissionService permissionService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            ILogger logger,
            IWebHelper webHelper,
            PaymentSettings paymentSettings,
            OKDollarPaymentSettings oKDollarPaymentSettings,
            ShoppingCartSettings shoppingCartSettings,
            IHttpContextAccessor httpContextAccessor,
            ICustomerService customerService,
            IPaymentTransactionHistoryService paymentTransactionHistoryService,
            INotificationService notificationService,
            ICustomerActivityService customerActivityService)
        {

            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._paymentService = paymentService;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._permissionService = permissionService;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._storeContext = storeContext;
            this._logger = logger;
            this._webHelper = webHelper;
            this._paymentSettings = paymentSettings;
            this._oKDollarPaymentSettings = oKDollarPaymentSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._httpContextAccessor = httpContextAccessor;
            this._customerService = customerService;
            this._paymentTransactionHistoryService = paymentTransactionHistoryService;
            _notificationService = notificationService;
            _customerActivityService = customerActivityService;
        }
        #endregion

        #region Methods
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            //var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var oKDollarPaymentSettings = _settingService.LoadSetting<OKDollarPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                //Amount = oKDollarPaymentSettings.Amount,
                ApiKey = oKDollarPaymentSettings.ApiKey,
                Destination = oKDollarPaymentSettings.Destination,
                MerchantName = oKDollarPaymentSettings.MerchantName,
                //RefNumber = oKDollarPaymentSettings.RefNumber,
                //Source = oKDollarPaymentSettings.Source
                AdditionalFee = oKDollarPaymentSettings.AdditionalFee,
                AdditionalFeePercentage = oKDollarPaymentSettings.AdditionalFeePercentage,
                PassProductNamesAndTotals = oKDollarPaymentSettings.PassProductNamesAndTotals,
                UseSandbox = oKDollarPaymentSettings.UseSandbox,
                SecretKey = oKDollarPaymentSettings.SecretKey,
                IVValue = oKDollarPaymentSettings.IVValue,
                RedirectAndPostDataURL = oKDollarPaymentSettings.RedirectAndPostDataURL

            };
            if (storeScope > 0)
            {
                //model.Amount_OverrideForStore = _settingService.SettingExists(oKDollarPaymentSettings, x => x.Amount, storeScope);
                model.ApiKey_OverrideForStore = _settingService.SettingExists(oKDollarPaymentSettings, x => x.ApiKey, storeScope);
                model.Destination_OverrideForStore = _settingService.SettingExists(oKDollarPaymentSettings, x => x.Destination, storeScope);
                model.MerchantName_OverrideForStore = _settingService.SettingExists(oKDollarPaymentSettings, x => x.MerchantName, storeScope);
                //model.RefNumber_OverrideForStore = _settingService.SettingExists(oKDollarPaymentSettings, x => x.RefNumber, storeScope);
                //model.Source_OverrideForStore = _settingService.SettingExists(oKDollarPaymentSettings, x => x.Source, storeScope);
                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(oKDollarPaymentSettings, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(oKDollarPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
                model.PassProductNamesAndTotals_OverrideForStore = _settingService.SettingExists(oKDollarPaymentSettings, x => x.PassProductNamesAndTotals, storeScope);
                model.UseSandbox_OverrideForStore = _settingService.SettingExists(oKDollarPaymentSettings, x => x.UseSandbox, storeScope);
                model.SecretKey_OverrideForStore = _settingService.SettingExists(oKDollarPaymentSettings, x => x.SecretKey, storeScope);
                model.IVValue_OverrideForStore = _settingService.SettingExists(oKDollarPaymentSettings, x => x.IVValue, storeScope);
                model.RedirectAndPostDataURL_OverrideForStore = _settingService.SettingExists(oKDollarPaymentSettings, x => x.RedirectAndPostDataURL, storeScope);
            }

            return View("~/Plugins/Payments.OKDollar/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            //var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var oKDollarPaymentSettings = _settingService.LoadSetting<OKDollarPaymentSettings>(storeScope);

            //save settings
            //oKDollarPaymentSettings.Amount = model.Amount;
            oKDollarPaymentSettings.ApiKey = model.ApiKey;
            oKDollarPaymentSettings.Destination = model.Destination;
            oKDollarPaymentSettings.MerchantName = model.MerchantName;
            //oKDollarPaymentSettings.RefNumber = model.RefNumber;
            //oKDollarPaymentSettings.Source = model.Source;
            oKDollarPaymentSettings.AdditionalFee = model.AdditionalFee;
            oKDollarPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            oKDollarPaymentSettings.PassProductNamesAndTotals = model.PassProductNamesAndTotals;
            oKDollarPaymentSettings.UseSandbox = model.UseSandbox;
            oKDollarPaymentSettings.SecretKey = model.SecretKey;
            oKDollarPaymentSettings.IVValue = model.IVValue;
            oKDollarPaymentSettings.RedirectAndPostDataURL = model.RedirectAndPostDataURL;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            //_settingService.SaveSettingOverridablePerStore(oKDollarPaymentSettings, x => x.Amount, model.Amount_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(oKDollarPaymentSettings, x => x.ApiKey, model.ApiKey_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(oKDollarPaymentSettings, x => x.Destination, model.Destination_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(oKDollarPaymentSettings, x => x.MerchantName, model.MerchantName_OverrideForStore, storeScope, false);
            //_settingService.SaveSettingOverridablePerStore(oKDollarPaymentSettings, x => x.RefNumber, model.RefNumber_OverrideForStore, storeScope, false);
            //_settingService.SaveSettingOverridablePerStore(oKDollarPaymentSettings, x => x.Source, model.Source_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(oKDollarPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(oKDollarPaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(oKDollarPaymentSettings, x => x.PassProductNamesAndTotals, model.PassProductNamesAndTotals_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(oKDollarPaymentSettings, x => x.UseSandbox, model.UseSandbox_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(oKDollarPaymentSettings, x => x.SecretKey, model.SecretKey_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(oKDollarPaymentSettings, x => x.IVValue, model.IVValue_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(oKDollarPaymentSettings, x => x.RedirectAndPostDataURL, model.RedirectAndPostDataURL_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        //action displaying notification (warning) to a store owner about inaccurate OKDollar rounding
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult RoundingWarning(bool passProductNamesAndTotals)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //prices and total aren't rounded, so display warning
            if (passProductNamesAndTotals && !_shoppingCartSettings.RoundPricesDuringCalculation)
                return Json(new { Result = _localizationService.GetResource("Plugins.Payments.OKDollar.RoundingWarning") });

            return Json(new { Result = string.Empty });
        }

        public IActionResult PDTHandler()
        {

                var data = HttpContext.Request.Form["PayMentDet"].ToString();
                var part = data.Split(',')[0];
                var key = _oKDollarPaymentSettings.SecretKey;    //  "B1234567b1234567"; //
                var ivValue = _oKDollarPaymentSettings.IVValue;  //  "1234567890123456"; // 

                var decryptObj = new AESDecrypt();
                var decrypted = decryptObj.DecryptStringAES(part, ivValue, key);
               var MainDecrpteddata = "";
            //if (!decrypted.Contains("{\"Comments"))
            MainDecrpteddata = decrypted.Replace("z!Bhlndauz", "{\"Comments");

            //FileStream objFilestream = new FileStream(string.Format("{0}\\{1}", Path.GetTempPath(),
            //    "ConsoleLog"), FileMode.Append, FileAccess.Write);
            //StreamWriter objStreamWriter = new StreamWriter((Stream)objFilestream);
            //objStreamWriter.WriteLine(data+ decrypted + MainDecrpteddata);
            //objStreamWriter.Close();
            //objFilestream.Close();

            var responseObj = JsonConvert.DeserializeObject<ResponseFromOKDollarPaymentGateway>(MainDecrpteddata);

            return RedirectToAction("OKDollarResponse", "OKDollarResponse", new
            {
                ResponseJson = MainDecrpteddata,
                MsgString = responseObj.ResponseCode
            });

            //var responseObj = JsonConvert.DeserializeObject<ResponseFromOKDollarPaymentGateway>(dummyData);

            //var responseObj = JsonConvert.DeserializeObject<ResponseFromOKDollarPaymentGateway>(dummyData);



            ////if (responseObj == null)
            ////    throw new ArgumentNullException(nameof(responseObj));
            ////return RedirectToAction("OKTest", "Home", new
            ////{
            ////    dataforok = responseObj,
            ////    reststring = decrypted
            ////});


            ////var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.OKDollar") as OKDollarPaymentProcessor;
            //////if (processor == null)
            //////    throw new ArgumentNullException(nameof(processor));
            ////if (processor == null || !_paymentService.IsPaymentMethodActive(processor) || !processor.PluginDescriptor.Installed)
            ////    throw new NopException("OK Dollar module cannot be loaded");

            //if (responseObj != null)
            //{
            //    //----Start
            //    //place order
            //    var processPaymentRequest = HttpContext.Session.Get<ProcessPaymentRequest>("OrderPaymentInfo");

            //    //if (processPaymentRequest == null)
            //    //    throw new ArgumentNullException("Payment request issue");

            //    //objStreamWriter.WriteLine(processPaymentRequest.CustomerId);

            //    //if (processPaymentRequest == null)
            //    //    throw new ArgumentNullException(nameof(processPaymentRequest));
            //    if (processPaymentRequest == null)
            //    {
            //        processPaymentRequest = new ProcessPaymentRequest();
            //    }

            //    //var customerId = Convert.ToInt32(responseObj.MerRefNo.Split('-')[0]);
            //    var customerId = 90107;
            //    objStreamWriter.WriteLine(customerId);

            //    _workContext.CurrentCustomer = _customerService.GetCustomerById(customerId);

            //    objStreamWriter.WriteLine(_workContext.CurrentCustomer.Id);

            //    if (_workContext.CurrentCustomer == null)
            //        throw new ArgumentNullException(nameof(_workContext.CurrentCustomer));

            //    processPaymentRequest.StoreId = _storeContext.CurrentStore.Id;
            //    objStreamWriter.WriteLine("Storeid"+ processPaymentRequest.StoreId);

            //    //if (processPaymentRequest.StoreId == null)
            //    //    throw new ArgumentNullException(nameof(processPaymentRequest.StoreId));


            //    processPaymentRequest.CustomerId = _workContext.CurrentCustomer.Id;

            //    objStreamWriter.WriteLine("Customerid" + processPaymentRequest.CustomerId);

            //    //if (processPaymentRequest.CustomerId == null)
            //    //    throw new ArgumentNullException(nameof(processPaymentRequest.CustomerId));


            //    processPaymentRequest.PaymentMethodSystemName = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer,
            //            NopCustomerDefaults.SelectedPaymentMethodAttribute, _storeContext.CurrentStore.Id);


            //    objStreamWriter.WriteLine("Payment Gateways" + processPaymentRequest.PaymentMethodSystemName);
            //    //if (processPaymentRequest.PaymentMethodSystemName == null)
            //    //    throw new ArgumentNullException(nameof(processPaymentRequest.PaymentMethodSystemName));

            //    // Saving transaction history
            //    var transactionHistory = new PaymentTransactionHistory()
            //    {
            //        CustomerId = customerId,
            //        PaymentMethod = processPaymentRequest.PaymentMethodSystemName,
            //        TransactionAmount = Convert.ToDecimal(responseObj.Amount),
            //        TransactionId = responseObj.TransactionId,
            //        TransactionDescription = responseObj.ResponseCode + " - " + responseObj.Description,
            //        TransactionStatus = responseObj.ResponseCode == "0" ? (int)TransactionStatusType.Success : (int)TransactionStatusType.Failed,
            //        IsNew = responseObj.ResponseCode == "0" ? false : true
            //    };

            //    objStreamWriter.Close();
            //    objFilestream.Close();

            //    _paymentTransactionHistoryService.InsertTransactionHistory(transactionHistory);

            //    if (responseObj.ResponseCode != "0")
            //    {
            //        return RedirectToRoute("CheckoutCompleted", new { orderId = 0, errorMessage = "Due to " + responseObj.Description + " your order not processed." });
            //    }

            //    var placeOrderResult = _orderProcessingService.PlaceOrder(processPaymentRequest);

            //    //if (placeOrderResult == null)
            //    //    throw new ArgumentNullException(nameof(placeOrderResult));

            //    //----End

            //    //var orderNumber = responseObj.MerRefNo;
            //    //var getOrder = _orderService.GetOrderByCustomOrderNumber(orderNumber);
            //    //var orders = _orderService.GetOrdersByOrderGroupNumber(getOrder.CustomerId, getOrder.OrderGroupNumber);
            //    //var orderId = orders[0].Id;
            //    var orderId = placeOrderResult.PlacedOrder.Id;

            //    //if (orderId == null)
            //    //    throw new ArgumentNullException(nameof(orderId));
            //    foreach (var order in placeOrderResult.PlacedOrders)
            //    {
            //        if (order != null)
            //        {
            //            var sb = new StringBuilder();
            //            sb.AppendLine("OK$ PDT:");
            //            sb.AppendLine("txn_id: " + responseObj.TransactionId);
            //            sb.AppendLine("payment_fee: " + responseObj.Amount);
            //            var payment_status = "completed";
            //            //if (responseObj.ResponseCode != "0")
            //            //{
            //            //    payment_status = "failed";
            //            //}
            //            var txn_id = responseObj.TransactionId;
            //            var mc_gross = decimal.Zero;
            //            mc_gross = Convert.ToDecimal(responseObj.Amount);
            //            var newPaymentStatus = OKDollarHelper.GetPaymentStatus(payment_status, string.Empty);

            //            //if (newPaymentStatus == null)
            //            //    throw new ArgumentNullException(nameof(newPaymentStatus));


            //            sb.AppendLine("New payment status: " + newPaymentStatus);

            //            //order note
            //            order.OrderNotes.Add(new OrderNote
            //            {
            //                Note = sb.ToString(),
            //                DisplayToCustomer = false,
            //                CreatedOnUtc = DateTime.UtcNow
            //            });
            //            _orderService.UpdateOrder(order);

            //            //validate order total
            //            var orderTotalSentToOKDollar = _genericAttributeService.GetAttribute<decimal?>(order, OKDollarHelper.OrderTotalSentToOKDollar);

            //            //if (orderTotalSentToOKDollar == null)
            //            //    throw new ArgumentNullException(nameof(orderTotalSentToOKDollar));


            //            if (orderTotalSentToOKDollar.HasValue && mc_gross != orderTotalSentToOKDollar.Value)
            //            {
            //                var errorStr =
            //                    $"OKDollar PDT. Returned order total {mc_gross} doesn't equal order total {order.OrderTotal}. Order# {order.Id}.";
            //                //log
            //                _logger.Error(errorStr);
            //                //order note
            //                order.OrderNotes.Add(new OrderNote
            //                {
            //                    Note = errorStr,
            //                    DisplayToCustomer = false,
            //                    CreatedOnUtc = DateTime.UtcNow
            //                });
            //                _orderService.UpdateOrder(order);

            //                return RedirectToAction("Index", "Home", new { area = "" });
            //            }
            //            //clear attribute

            //            //if (orderTotalSentToOKDollar.HasValue == null)
            //            //    throw new ArgumentNullException(nameof(orderTotalSentToOKDollar.HasValue));
            //            if (orderTotalSentToOKDollar.HasValue)
            //                _genericAttributeService.SaveAttribute<decimal?>(order, OKDollarHelper.OrderTotalSentToOKDollar, null);

            //            //mark order as paid
            //            if (newPaymentStatus == PaymentStatus.Paid)
            //            {
            //                order.AuthorizationTransactionId = txn_id;
            //                order.AuthorizationTransactionCode = responseObj.ResponseCode + "-" + responseObj.MerRefNo;
            //                order.AuthorizationTransactionResult = responseObj.Description;
            //                //_orderService.UpdateOrder(order);
            //                _orderProcessingService.MarkOrderAsPaid(order);
            //                //send order id to warehouse management
            //                //_notificationService.SendNoticationToWHM(order);
            //            }
            //            else
            //            {
            //                order.AuthorizationTransactionId = txn_id;
            //                order.AuthorizationTransactionCode = responseObj.ResponseCode + "-" + responseObj.MerRefNo;
            //                order.AuthorizationTransactionResult = responseObj.Description;
            //                _orderService.UpdateOrder(order);
            //            }
            //        }
            //    }


            //    return RedirectToRoute("CheckoutCompleted", new { orderId = orderId });
            //}
            //else
            //{
            //    var orderNumber = string.Empty;
            //    var orderNumberGuid = Guid.Empty;
            //    try
            //    {
            //        orderNumberGuid = new Guid(orderNumber);
            //    }
            //    catch { }
            //    var order = _orderService.GetOrderByGuid(orderNumberGuid);
            //    if (order != null)
            //    {
            //        //order note
            //        order.OrderNotes.Add(new OrderNote
            //        {
            //            Note = "OKDollar PDT failed. ",
            //            DisplayToCustomer = false,
            //            CreatedOnUtc = DateTime.UtcNow
            //        });
            //        _orderService.UpdateOrder(order);
            //    }
            //    return RedirectToAction("Index", "Home", new { area = "" });
            //}



        }

        public IActionResult IPNHandler()
        {
            byte[] parameters;
            using (var stream = new MemoryStream())
            {
                this.Request.Body.CopyTo(stream);
                parameters = stream.ToArray();
            }
            var strRequest = Encoding.ASCII.GetString(parameters);

            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.OKDollar") as OKDollarPaymentProcessor;
            if (processor == null ||
                //!processor.IsPaymentMethodActive(_paymentSettings) || !processor.PluginDescriptor.Installed)
                !_paymentService.IsPaymentMethodActive(processor) || !processor.PluginDescriptor.Installed)
                throw new NopException("OKDollar module cannot be loaded");

            if (processor.VerifyIpn(strRequest, out Dictionary<string, string> values))
            {
                #region values
                var mc_gross = decimal.Zero;
                try
                {
                    mc_gross = decimal.Parse(values["mc_gross"], new CultureInfo("en-US"));
                }
                catch { }

                values.TryGetValue("payer_status", out string payer_status);
                values.TryGetValue("payment_status", out string payment_status);
                values.TryGetValue("pending_reason", out string pending_reason);
                values.TryGetValue("mc_currency", out string mc_currency);
                values.TryGetValue("txn_id", out string txn_id);
                values.TryGetValue("txn_type", out string txn_type);
                values.TryGetValue("rp_invoice_id", out string rp_invoice_id);
                values.TryGetValue("payment_type", out string payment_type);
                values.TryGetValue("payer_id", out string payer_id);
                values.TryGetValue("receiver_id", out string receiver_id);
                values.TryGetValue("invoice", out string _);
                values.TryGetValue("payment_fee", out string payment_fee);

                #endregion

                var sb = new StringBuilder();
                sb.AppendLine("PayPal IPN:");
                foreach (var kvp in values)
                {
                    sb.AppendLine(kvp.Key + ": " + kvp.Value);
                }

                var newPaymentStatus = OKDollarHelper.GetPaymentStatus(payment_status, pending_reason);
                sb.AppendLine("New payment status: " + newPaymentStatus);

                switch (txn_type)
                {
                    case "recurring_payment_profile_created":
                        //do nothing here
                        break;
                    #region Recurring payment
                    case "recurring_payment":
                        {
                            var orderNumberGuid = Guid.Empty;
                            try
                            {
                                orderNumberGuid = new Guid(rp_invoice_id);
                            }
                            catch
                            {
                            }

                            var initialOrder = _orderService.GetOrderByGuid(orderNumberGuid);
                            if (initialOrder != null)
                            {
                                var recurringPayments = _orderService.SearchRecurringPayments(initialOrderId: initialOrder.Id);
                                foreach (var rp in recurringPayments)
                                {
                                    switch (newPaymentStatus)
                                    {
                                        case PaymentStatus.Authorized:
                                        case PaymentStatus.Paid:
                                            {
                                                var recurringPaymentHistory = rp.RecurringPaymentHistory;
                                                if (!recurringPaymentHistory.Any())
                                                {
                                                    //first payment
                                                    var rph = new RecurringPaymentHistory
                                                    {
                                                        RecurringPaymentId = rp.Id,
                                                        OrderId = initialOrder.Id,
                                                        CreatedOnUtc = DateTime.UtcNow
                                                    };
                                                    rp.RecurringPaymentHistory.Add(rph);
                                                    _orderService.UpdateRecurringPayment(rp);
                                                }
                                                else
                                                {
                                                    //next payments
                                                    var processPaymentResult = new ProcessPaymentResult
                                                    {
                                                        NewPaymentStatus = newPaymentStatus
                                                    };
                                                    if (newPaymentStatus == PaymentStatus.Authorized)
                                                        processPaymentResult.AuthorizationTransactionId = txn_id;
                                                    else
                                                        processPaymentResult.CaptureTransactionId = txn_id;

                                                    _orderProcessingService.ProcessNextRecurringPayment(rp, processPaymentResult);
                                                }
                                            }
                                            break;
                                        case PaymentStatus.Voided:
                                            //failed payment
                                            var failedPaymentResult = new ProcessPaymentResult
                                            {
                                                Errors = new[] { $"PayPal IPN. Recurring payment is {payment_status} ." },
                                                RecurringPaymentFailed = true
                                            };
                                            _orderProcessingService.ProcessNextRecurringPayment(rp, failedPaymentResult);
                                            break;
                                    }
                                }

                                //this.OrderService.InsertOrderNote(newOrder.OrderId, sb.ToString(), DateTime.UtcNow);
                                _logger.Information("PayPal IPN. Recurring info", new NopException(sb.ToString()));
                            }
                            else
                            {
                                _logger.Error("PayPal IPN. Order is not found", new NopException(sb.ToString()));
                            }
                        }
                        break;
                    case "recurring_payment_failed":
                        if (Guid.TryParse(rp_invoice_id, out Guid orderGuid))
                        {
                            var initialOrder = _orderService.GetOrderByGuid(orderGuid);
                            if (initialOrder != null)
                            {
                                var recurringPayment = _orderService.SearchRecurringPayments(initialOrderId: initialOrder.Id).FirstOrDefault();
                                //failed payment
                                if (recurringPayment != null)
                                    _orderProcessingService.ProcessNextRecurringPayment(recurringPayment, new ProcessPaymentResult { Errors = new[] { txn_type }, RecurringPaymentFailed = true });
                            }
                        }
                        break;
                    #endregion
                    default:
                        #region Standard payment
                        {
                            values.TryGetValue("custom", out string orderNumber);
                            var orderNumberGuid = Guid.Empty;
                            try
                            {
                                orderNumberGuid = new Guid(orderNumber);
                            }
                            catch
                            {
                            }

                            var order = _orderService.GetOrderByGuid(orderNumberGuid);
                            if (order != null)
                            {

                                //order note
                                order.OrderNotes.Add(new OrderNote
                                {
                                    Note = sb.ToString(),
                                    DisplayToCustomer = false,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                                _orderService.UpdateOrder(order);

                                switch (newPaymentStatus)
                                {
                                    case PaymentStatus.Pending:
                                        {
                                        }
                                        break;
                                    case PaymentStatus.Authorized:
                                        {
                                            //validate order total
                                            if (Math.Round(mc_gross, 2).Equals(Math.Round(order.OrderTotal, 2)))
                                            {
                                                //valid
                                                if (_orderProcessingService.CanMarkOrderAsAuthorized(order))
                                                {
                                                    _orderProcessingService.MarkAsAuthorized(order);
                                                }
                                            }
                                            else
                                            {
                                                //not valid
                                                var errorStr =
                                                    $"OKDollar IPN. Returned order total {mc_gross} doesn't equal order total {order.OrderTotal}. Order# {order.Id}.";
                                                //log
                                                _logger.Error(errorStr);
                                                //order note
                                                order.OrderNotes.Add(new OrderNote
                                                {
                                                    Note = errorStr,
                                                    DisplayToCustomer = false,
                                                    CreatedOnUtc = DateTime.UtcNow
                                                });
                                                _orderService.UpdateOrder(order);
                                            }
                                        }
                                        break;
                                    case PaymentStatus.Paid:
                                        {
                                            //validate order total
                                            if (Math.Round(mc_gross, 2).Equals(Math.Round(order.OrderTotal, 2)))
                                            {
                                                //valid
                                                if (_orderProcessingService.CanMarkOrderAsPaid(order))
                                                {
                                                    order.AuthorizationTransactionId = txn_id;
                                                    _orderService.UpdateOrder(order);

                                                    _orderProcessingService.MarkOrderAsPaid(order);
                                                }
                                            }
                                            else
                                            {
                                                //not valid
                                                var errorStr =
                                                    $"OKDollar IPN. Returned order total {mc_gross} doesn't equal order total {order.OrderTotal}. Order# {order.Id}.";
                                                //log
                                                _logger.Error(errorStr);
                                                //order note
                                                order.OrderNotes.Add(new OrderNote
                                                {
                                                    Note = errorStr,
                                                    DisplayToCustomer = false,
                                                    CreatedOnUtc = DateTime.UtcNow
                                                });
                                                _orderService.UpdateOrder(order);
                                            }
                                        }
                                        break;
                                    case PaymentStatus.Refunded:
                                        {
                                            var totalToRefund = Math.Abs(mc_gross);
                                            if (totalToRefund > 0 && Math.Round(totalToRefund, 2).Equals(Math.Round(order.OrderTotal, 2)))
                                            {
                                                //refund
                                                if (_orderProcessingService.CanRefundOffline(order))
                                                {
                                                    _orderProcessingService.RefundOffline(order);
                                                }
                                            }
                                            else
                                            {
                                                //partial refund
                                                if (_orderProcessingService.CanPartiallyRefundOffline(order, totalToRefund))
                                                {
                                                    _orderProcessingService.PartiallyRefundOffline(order, totalToRefund);
                                                }
                                            }
                                        }
                                        break;
                                    case PaymentStatus.Voided:
                                        {
                                            if (_orderProcessingService.CanVoidOffline(order))
                                            {
                                                _orderProcessingService.VoidOffline(order);
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                _logger.Error("OKDollar IPN. Order is not found", new NopException(sb.ToString()));
                            }
                        }
                        #endregion
                        break;
                }
            }
            else
            {
                _logger.Error("OKDollar IPN failed.", new NopException(strRequest));
            }

            //nothing should be rendered to visitor
            return Content("");
        }

        public IActionResult CancelOrder()
        {
            var order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1).FirstOrDefault();
            if (order != null)
                return RedirectToRoute("OrderDetails", new { orderId = order.Id });

            return RedirectToRoute("HomePage");
        }

        #endregion

    }
}

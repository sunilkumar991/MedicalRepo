using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using My2C2PPKCS7;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Payments._2C2P.Models;
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

namespace Nop.Plugin.Payments._2C2P.Controllers
{
    // Created by Alexandar Rajavel on 14-Mar-2019
    public class Payment2C2PController : BasePaymentController
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
        private readonly _2C2PPaymentSettings _2C2PPaymentSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICustomerService _customerService;
        private readonly IPaymentTransactionHistoryService _paymentTransactionHistoryService;
        private readonly INotificationService _notificationService;

        #endregion

        #region Constructor
        public Payment2C2PController(IWorkContext workContext,
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
            _2C2PPaymentSettings twoC2PPaymentSettings,
            ShoppingCartSettings shoppingCartSettings,
            IHttpContextAccessor httpContextAccessor,
            ICustomerService customerService,
            IPaymentTransactionHistoryService paymentTransactionHistoryService,
            INotificationService notificationService)
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
            _2C2PPaymentSettings = twoC2PPaymentSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._httpContextAccessor = httpContextAccessor;
            this._customerService = customerService;
            this._paymentTransactionHistoryService = paymentTransactionHistoryService;
            _notificationService = notificationService;
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
            var paymentSettings2C2P = _settingService.LoadSetting<_2C2PPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                UseSandbox = paymentSettings2C2P.UseSandbox,
                Version = paymentSettings2C2P.Version,
                MerchantId = paymentSettings2C2P.MerchantId,
                SecretKey = paymentSettings2C2P.SecretKey,
                ResultUrl = paymentSettings2C2P.ResultUrl,
                Currency = paymentSettings2C2P.Currency,
                AdditionalFee = paymentSettings2C2P.AdditionalFee,
                AdditionalFeePercentage = paymentSettings2C2P.AdditionalFeePercentage,
                PassProductNamesAndTotals = paymentSettings2C2P.PassProductNamesAndTotals,
                RedirectAndPostDataURL = paymentSettings2C2P.RedirectAndPostDataURL,
                PrivatePfxFilePath = paymentSettings2C2P.PrivatePfxFilePath,
                PrivatePfxFilePassword = paymentSettings2C2P.PrivatePfxFilePassword
            };
            if (storeScope > 0)
            {
                model.UseSandbox_OverrideForStore = _settingService.SettingExists(paymentSettings2C2P, x => x.UseSandbox, storeScope);
                model.Version_OverrideForStore = _settingService.SettingExists(paymentSettings2C2P, x => x.Version, storeScope);
                model.MerchantId_OverrideForStore = _settingService.SettingExists(paymentSettings2C2P, x => x.MerchantId, storeScope);
                model.SecretKey_OverrideForStore = _settingService.SettingExists(paymentSettings2C2P, x => x.SecretKey, storeScope);
                model.ResultUrl_OverrideForStore = _settingService.SettingExists(paymentSettings2C2P, x => x.ResultUrl, storeScope);
                model.Currency_OverrideForStore = _settingService.SettingExists(paymentSettings2C2P, x => x.Currency, storeScope);
                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(paymentSettings2C2P, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(paymentSettings2C2P, x => x.AdditionalFeePercentage, storeScope);
                model.PassProductNamesAndTotals_OverrideForStore = _settingService.SettingExists(paymentSettings2C2P, x => x.PassProductNamesAndTotals, storeScope);
                model.RedirectAndPostDataURL_OverrideForStore = _settingService.SettingExists(paymentSettings2C2P, x => x.RedirectAndPostDataURL, storeScope);
                model.PrivatePfxFilePath_OverrideForStore = _settingService.SettingExists(paymentSettings2C2P, x => x.PrivatePfxFilePath, storeScope);
                model.PrivatePfxFilePassword_OverrideForStore = _settingService.SettingExists(paymentSettings2C2P, x => x.PrivatePfxFilePassword, storeScope);
            }

            return View("~/Plugins/Payments.2C2P/Views/Configure.cshtml", model);
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
            var paymentSettings2C2P = _settingService.LoadSetting<_2C2PPaymentSettings>(storeScope);

            //save settings
            paymentSettings2C2P.UseSandbox = model.UseSandbox;
            paymentSettings2C2P.Version = model.Version;
            paymentSettings2C2P.MerchantId = model.MerchantId;
            paymentSettings2C2P.SecretKey = model.SecretKey;
            paymentSettings2C2P.ResultUrl = model.ResultUrl;
            paymentSettings2C2P.Currency = model.Currency;
            paymentSettings2C2P.AdditionalFee = model.AdditionalFee;
            paymentSettings2C2P.AdditionalFeePercentage = model.AdditionalFeePercentage;
            paymentSettings2C2P.PassProductNamesAndTotals = model.PassProductNamesAndTotals;
            paymentSettings2C2P.RedirectAndPostDataURL = model.RedirectAndPostDataURL;
            paymentSettings2C2P.PrivatePfxFilePath = model.PrivatePfxFilePath;
            paymentSettings2C2P.PrivatePfxFilePassword = model.PrivatePfxFilePassword;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(paymentSettings2C2P, x => x.UseSandbox, model.UseSandbox_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(paymentSettings2C2P, x => x.Version, model.Version_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(paymentSettings2C2P, x => x.MerchantId, model.MerchantId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(paymentSettings2C2P, x => x.SecretKey, model.SecretKey_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(paymentSettings2C2P, x => x.ResultUrl, model.ResultUrl_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(paymentSettings2C2P, x => x.Currency, model.Currency_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(paymentSettings2C2P, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(paymentSettings2C2P, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(paymentSettings2C2P, x => x.PassProductNamesAndTotals, model.PassProductNamesAndTotals_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(paymentSettings2C2P, x => x.RedirectAndPostDataURL, model.RedirectAndPostDataURL_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(paymentSettings2C2P, x => x.PrivatePfxFilePath, model.PrivatePfxFilePath_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(paymentSettings2C2P, x => x.PrivatePfxFilePassword, model.PrivatePfxFilePassword_OverrideForStore, storeScope, false);

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
                return Json(new { Result = _localizationService.GetResource("Plugins.Payments.2C2P.RoundingWarning") });

            return Json(new { Result = string.Empty });
        }

        public IActionResult PDTHandler(_2C2PResponseData responseObj)
        {
            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.2C2P") as _2C2PPaymentProcessor;
            if (processor == null || !_paymentService.IsPaymentMethodActive(processor) || !processor.PluginDescriptor.Installed)
                throw new NopException("2C2P module cannot be loaded");

            if (responseObj != null)
            {
                //----Start
                //place order
                var processPaymentRequest = HttpContext.Session.Get<ProcessPaymentRequest>("OrderPaymentInfo");
                if (processPaymentRequest == null)
                {
                    processPaymentRequest = new ProcessPaymentRequest();
                }

                var customerId = Convert.ToInt32(responseObj.order_id.Split('-')[0]);
                _workContext.CurrentCustomer = _customerService.GetCustomerById(customerId);
                processPaymentRequest.StoreId = _storeContext.CurrentStore.Id;
                processPaymentRequest.CustomerId = _workContext.CurrentCustomer.Id;
                processPaymentRequest.PaymentMethodSystemName = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, _storeContext.CurrentStore.Id);

                var transactionStatus = (int)TransactionStatusType.Failed;
                var isNewFlag = true;
                switch (responseObj.payment_status)
                {
                    case PAYMENT_STATUS_SUCCESS:
                        transactionStatus = (int)TransactionStatusType.Success;
                        isNewFlag = false;
                        break;
                    case PAYMENT_STATUS_PENDING:
                        transactionStatus = (int)TransactionStatusType.Pending;
                        break;
                }

                // Saving transaction history
                var transactionHistory = new PaymentTransactionHistory()
                {
                    CustomerId = customerId,
                    PaymentMethod = processPaymentRequest.PaymentMethodSystemName,
                    TransactionAmount = Convert.ToDecimal(responseObj.amount),
                    TransactionId = responseObj.transaction_ref,
                    TransactionDescription = responseObj.channel_response_code + " - " + responseObj.channel_response_desc,
                    TransactionStatus = transactionStatus,
                    IsNew = isNewFlag
                };
                _paymentTransactionHistoryService.InsertTransactionHistory(transactionHistory);

                if (responseObj.payment_status != PAYMENT_STATUS_SUCCESS && responseObj.payment_status != PAYMENT_STATUS_PENDING)
                {
                    return RedirectToRoute("CheckoutCompleted", new { orderId = 0, errorMessage = "Due to " + responseObj.channel_response_desc + " your order not processed." });
                }

                var placeOrderResult = _orderProcessingService.PlaceOrder(processPaymentRequest);
                //----End

                var orderId = placeOrderResult.PlacedOrder.Id;
                foreach (var order in placeOrderResult.PlacedOrders)
                {
                    if (order != null)
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine("2C2P PDT:");
                        sb.AppendLine("txn_id: " + responseObj.transaction_ref);
                        sb.AppendLine("payment_fee: " + responseObj.amount);
                        var payment_status = responseObj.payment_status == PAYMENT_STATUS_SUCCESS ? COMPLETED : PENDING;
                        //if (responseObj.ResponseCode != "0")
                        //{
                        //    payment_status = "failed";
                        //}
                        var txn_id = responseObj.transaction_ref;
                        var mc_gross = decimal.Zero;
                        mc_gross = Convert.ToDecimal(responseObj.amount);
                        var newPaymentStatus = _2C2PHelper.GetPaymentStatus(payment_status, string.Empty);
                        sb.AppendLine("New payment status: " + newPaymentStatus);

                        //order note
                        order.OrderNotes.Add(new OrderNote
                        {
                            Note = sb.ToString(),
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                        _orderService.UpdateOrder(order);

                        //validate order total
                        var orderTotalSentToOKDollar = _genericAttributeService.GetAttribute<decimal?>(order, _2C2PHelper.OrderTotalSentTo2C2P);
                        if (orderTotalSentToOKDollar.HasValue && mc_gross != orderTotalSentToOKDollar.Value)
                        {
                            var errorStr =
                                $"2C2P PDT. Returned order total {mc_gross} doesn't equal order total {order.OrderTotal}. Order# {order.Id}.";
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

                            return RedirectToAction("Index", "Home", new { area = "" });
                        }
                        //clear attribute
                        if (orderTotalSentToOKDollar.HasValue)
                            _genericAttributeService.SaveAttribute<decimal?>(order, _2C2PHelper.OrderTotalSentTo2C2P, null);

                        //mark order as paid
                        if (newPaymentStatus == PaymentStatus.Paid)
                        {
                            order.AuthorizationTransactionId = txn_id;
                            order.AuthorizationTransactionCode = responseObj.channel_response_code + "-" + responseObj.order_id;
                            order.AuthorizationTransactionResult = responseObj.channel_response_desc;
                            //_orderService.UpdateOrder(order);
                            _orderProcessingService.MarkOrderAsPaid(order);
                            //send order id to warehouse management
                            //_notificationService.SendNoticationToWHM(order);
                        }
                        else
                        {
                            order.AuthorizationTransactionId = txn_id;
                            order.AuthorizationTransactionCode = responseObj.channel_response_code + "-" + responseObj.order_id;
                            order.AuthorizationTransactionResult = responseObj.channel_response_desc;
                            order.PaymentStatusId = (int)PaymentStatus.Pending;
                            _orderService.UpdateOrder(order);
                        }
                    }
                }
                return RedirectToRoute("CheckoutCompleted", new { orderId = orderId });
            }
            else
            {
                var orderNumber = string.Empty;
                var orderNumberGuid = Guid.Empty;
                try
                {
                    orderNumberGuid = new Guid(orderNumber);
                }
                catch { }
                var order = _orderService.GetOrderByGuid(orderNumberGuid);
                if (order != null)
                {
                    //order note
                    order.OrderNotes.Add(new OrderNote
                    {
                        Note = "2C2P PDT failed.",
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _orderService.UpdateOrder(order);
                }
                return RedirectToAction("Index", "Home", new { area = "" });
            }

        }

        // For Web--This will receive response from 2C2P for Counter and Internet payment
        // Created By Alexandar Rajavel on 06-Apr-2019
        public void BackendResponseFrom2C2P(_2C2PResponseData responseObj)
        {
            CustomerRemark remark = new CustomerRemark();
            var orders = _orderService.GetOrdersByTransactionId(responseObj.transaction_ref);
            var ordCount = 0;
            foreach (var order in orders)
            {
                ordCount++;
                if (responseObj.payment_status == PAYMENT_STATUS_SUCCESS)
                {
                    //order.OrderStatusId = (int)OrderStatus.Processing;
                    order.AuthorizationTransactionCode = responseObj.payment_status;
                    order.AuthorizationTransactionResult = responseObj.channel_response_desc;
                    _orderProcessingService.MarkOrderAsPaid(order);
                    //_notificationService.SendNoticationToWHM(order);
                }
                if (ordCount <= 1)
                {
                    remark.CustomerId = order.CustomerId;
                    remark.NetworkRemark = JsonConvert.SerializeObject(responseObj);
                    remark.CreatedOnUtc = DateTime.UtcNow;
                    _customerService.InsertCustomerRemark(remark);
                }
            }
        }

        // For SDK(Mobile App)--This will receive response from 2C2P for Counter and Internet payment
        // Created By Alexandar Rajavel on 06-Apr-2019
        public void SDKBackendResponseFrom2C2P()
        {
            CustomerRemark remark = new CustomerRemark();
            var encryptedPaymentResponse = Request.Form["paymentResponse"];
            PKCS7 pkcs7 = new PKCS7();
            //var paymentResponse = pkcs7.decryptMessage(encryptedPaymentResponse, pkcs7.getPrivateCert("D:/2C2P/private.pfx", "Okdollar@1234"));
            var paymentResponse = pkcs7.decryptMessage(encryptedPaymentResponse, pkcs7.getPrivateCert(_2C2PPaymentSettings.PrivatePfxFilePath, _2C2PPaymentSettings.PrivatePfxFilePassword));
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(paymentResponse);
            var jsonText = JsonConvert.SerializeXmlNode(doc);
            var responseObj = JsonConvert.DeserializeObject<BackendResponseDataFrom2C2P>(jsonText);

            var orders = _orderService.GetOrdersByTransactionId(responseObj.PaymentResponse.uniqueTransactionCode);
            var ordCount = 0;
            foreach (var order in orders)
            {
                ordCount++;
                if (responseObj.PaymentResponse.respCode == PAYMENT_STATUS_SUCCESS)
                {
                    //order.OrderStatusId = (int)OrderStatus.Processing;
                    order.AuthorizationTransactionCode = responseObj.PaymentResponse.respCode;
                    order.AuthorizationTransactionResult = responseObj.PaymentResponse.failReason;
                    _orderProcessingService.MarkOrderAsPaid(order);
                    //_notificationService.SendNoticationToWHM(order);
                }
                if (ordCount <= 1)
                {
                    remark.CustomerId = order.CustomerId;
                    remark.NetworkRemark = JsonConvert.SerializeObject(responseObj);
                    remark.CreatedOnUtc = DateTime.UtcNow;
                    _customerService.InsertCustomerRemark(remark);
                }
            }
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

            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.2C2P") as _2C2PPaymentProcessor;
            if (processor == null ||
                //!processor.IsPaymentMethodActive(_paymentSettings) || !processor.PluginDescriptor.Installed)
                !_paymentService.IsPaymentMethodActive(processor) || !processor.PluginDescriptor.Installed)
                throw new NopException("2C2P module cannot be loaded");

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
                sb.AppendLine("2C2P IPN:");
                foreach (var kvp in values)
                {
                    sb.AppendLine(kvp.Key + ": " + kvp.Value);
                }

                var newPaymentStatus = _2C2PHelper.GetPaymentStatus(payment_status, pending_reason);
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
                                                Errors = new[] { $"2C2P IPN. Recurring payment is {payment_status} ." },
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
                                _logger.Error("2C2P IPN. Order is not found", new NopException(sb.ToString()));
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
                                                    $"2C2P IPN. Returned order total {mc_gross} doesn't equal order total {order.OrderTotal}. Order# {order.Id}.";
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
                                                    $"2C2P IPN. Returned order total {mc_gross} doesn't equal order total {order.OrderTotal}. Order# {order.Id}.";
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
                                _logger.Error("2C2P IPN. Order is not found", new NopException(sb.ToString()));
                            }
                        }
                        #endregion
                        break;
                }
            }
            else
            {
                _logger.Error("2C2P IPN failed.", new NopException(strRequest));
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

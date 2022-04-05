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
    [Route("api/[controller]")]
    [ApiController]
    public class OKDollarResponseController: ControllerBase
    {
        //private readonly IWorkContext _workContext;
        //private readonly IStoreService _storeService;
        //private readonly ISettingService _settingService;
        //private readonly IPaymentService _paymentService;
        //private readonly IOrderService _orderService;
        //private readonly IOrderProcessingService _orderProcessingService;
        //private readonly IPermissionService _permissionService;
        //private readonly IGenericAttributeService _genericAttributeService;
        //private readonly ILocalizationService _localizationService;
        //private readonly IStoreContext _storeContext;
        //private readonly ILogger _logger;
        //private readonly IWebHelper _webHelper;
        //private readonly PaymentSettings _paymentSettings;
        //private readonly OKDollarPaymentSettings _oKDollarPaymentSettings;
        //private readonly ShoppingCartSettings _shoppingCartSettings;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly ICustomerService _customerService;
        //private readonly IPaymentTransactionHistoryService _paymentTransactionHistoryService;
        //private readonly INotificationService _notificationService;

        // GET: api/OKDollarResponse
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost]
        public IActionResult PDTHandler([FromBody]string model)
        {

            return Ok("Sucess");
            //var data = HttpContext.Request.Form["PayMentDet"].ToString();
            //var key = _oKDollarPaymentSettings.SecretKey;    //  "B1234567b1234567"; //
            //var ivValue = _oKDollarPaymentSettings.IVValue;  //  "1234567890123456"; // 
            //var part = data.Split(',')[0];
            //var decryptObj = new AESDecrypt();
            //var decrypted = decryptObj.DecryptStringAES(part, ivValue, key);
            //var responseObj = JsonConvert.DeserializeObject<ResponseFromOKDollarPaymentGateway>(decrypted);

            //var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.OKDollar") as OKDollarPaymentProcessor;
            //if (processor == null || !_paymentService.IsPaymentMethodActive(processor) || !processor.PluginDescriptor.Installed)
            //    throw new NopException("OK Dollar module cannot be loaded");

            //if (responseObj != null)
            //{
            //    //----Start
            //    //place order
            //    var processPaymentRequest = HttpContext.Session.Get<ProcessPaymentRequest>("OrderPaymentInfo");
            //    if (processPaymentRequest == null)
            //    {
            //        processPaymentRequest = new ProcessPaymentRequest();
            //    }

            //    var customerId = Convert.ToInt32(responseObj.MerRefNo.Split('-')[0]);
            //    _workContext.CurrentCustomer = _customerService.GetCustomerById(customerId);
            //    processPaymentRequest.StoreId = _storeContext.CurrentStore.Id;
            //    processPaymentRequest.CustomerId = _workContext.CurrentCustomer.Id;
            //    processPaymentRequest.PaymentMethodSystemName = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer,
            //        NopCustomerDefaults.SelectedPaymentMethodAttribute, _storeContext.CurrentStore.Id);

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
            //    _paymentTransactionHistoryService.InsertTransactionHistory(transactionHistory);

            //    if (responseObj.ResponseCode != "0")
            //    {
            //        return RedirectToRoute("CheckoutCompleted", new { orderId = 0, errorMessage = "Due to " + responseObj.Description + " your order not processed." });
            //    }

            //    var placeOrderResult = _orderProcessingService.PlaceOrder(processPaymentRequest);
            //    //----End

            //    //var orderNumber = responseObj.MerRefNo;
            //    //var getOrder = _orderService.GetOrderByCustomOrderNumber(orderNumber);
            //    //var orders = _orderService.GetOrdersByOrderGroupNumber(getOrder.CustomerId, getOrder.OrderGroupNumber);
            //    //var orderId = orders[0].Id;
            //    var orderId = placeOrderResult.PlacedOrder.Id;
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

    }
}

using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Factories;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using BS.Plugin.NopStation.MobileWebApi.Models.PickUp;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Tax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using QueuedNotification = Nop.Core.Domain.Messages.QueuedNotification;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public partial class PickupapiController : BaseApiController
    {
        #region Fields

        private IPickUpOrderModelFactoryApi _pickUpOrderModelFactoryApi;
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly IDeviceService _deviceService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IProductService _productService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ITaxService _taxService;
        #endregion

        #region Constructors

        public PickupapiController(IPickUpOrderModelFactoryApi pickUpOrderModelFactoryApi,
            IOrderService orderService, IWorkContext workContext, IDeviceService deviceService,
            ILocalizationService localizationService, INotificationService notificationService,
            IProductService productService, IOrderProcessingService orderProcessingService,IPriceCalculationService priceCalculationService,
            ITaxService taxService)
        {
            _pickUpOrderModelFactoryApi = pickUpOrderModelFactoryApi;
            _orderService = orderService;
            _workContext = workContext;
            _deviceService = deviceService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _productService = productService;
            _orderProcessingService = orderProcessingService;
            _priceCalculationService = priceCalculationService;
            _taxService = taxService;

        }

        #endregion

        #region Methods

        /// Created By: Sunil Kumar
        /// Created on : 15 April 2020
        //Get All Incomplete Orders 
        [HttpPost]
        [Route("api/order/IncompleteOrders")]
        public IActionResult IncompleteOrders([FromBody]PickUpOrderSearchModelApi model, int pageNumber = 1, int pageSize = 10)
        {
            //prepare model
            if (model == null)
            {
                model = (new PickUpOrderSearchModelApi
                {
                    //OrderStatusIds = null,
                    PaymentStatusIds = null,
                    ShippingStatusIds = null
                });
            }
            PickUpOrderListModel pickUpOrderModel = _pickUpOrderModelFactoryApi.PrepareCustomerOrderListModel(model, pageNumber, pageSize);

            return Ok(pickUpOrderModel);
        }


        /// Created By: Sunil Kumar
        /// Created on : 20 April 2020
        //Get Order Status ID
        [HttpGet]
        [Route("api/order/OrderStatusChecking/{orderId}/{deviceId}")]
        public IActionResult OrderStatusChecking(int orderId, string deviceId)
        {
            OrderStatusDetailsResponse orderStatus = new OrderStatusDetailsResponse();
            if (orderId != 0)
            {
                Order order = _orderService.GetOrderById(orderId);
                if (order == null || order.Deleted)
                {
                    return Unauthorized();
                }

                if (order.DeviceId == null || order.DeviceId == string.Empty)
                {

                    order.DeviceId = deviceId;
                    _orderService.UpdateOrder(order);
                }
                orderStatus.DeviceId = order.DeviceId != null ? order.DeviceId : string.Empty;
                orderStatus.OrderStatusId = order.OrderStatusId;
            }
            else
            {
                return Unauthorized();
            }
            return Ok(orderStatus);
        }


        /// Created By: Sunil Kumar
        /// Created on : 06-02-2020 2020
        //Get Order Status ID
        [HttpGet]
        [Route("api/v1/order/OrderStatusChecking/{orderId}/{deviceId}/{EditDateTime}")]
        public IActionResult OrderStatusChecking(int orderId, string deviceId, DateTime EditDateTime)
        {
            OrderStatusDetailsResponse orderStatus = new OrderStatusDetailsResponse();
            if (orderId != 0)
            {
                Order order = _orderService.GetOrderById(orderId);
                if (order == null || order.Deleted)
                {
                    return Unauthorized();
                }
                order.EditDateTime = EditDateTime;
                if (order.DeviceId == null || order.DeviceId == string.Empty)
                {
                    order.DeviceId = deviceId;
                    _orderService.UpdateOrder(order);
                }
                orderStatus.DeviceId = order.DeviceId != null ? order.DeviceId : string.Empty;
                orderStatus.OrderStatusId = order.OrderStatusId;
            }
            else
            {
                return Unauthorized();
            }
            return Ok(orderStatus);
        }

        /// Created By: Sunil Kumar
        /// Created on : 20 April 2020
        //Update Order Details 
        [Route("api/order/UpdateOrderDetails")]
        [HttpPost]
        public IActionResult UpdateOrderDetails([FromBody]PickUpOrderModel model)
        {
            int quantity = 0;
            BaseResponse responseModel = new BaseResponse();
            if (model == null)
            {
                responseModel.StatusCode = (int)ErrorType.NotFound;
                responseModel.ErrorList.Add(ORDER_NOT_FOUND);
                return NotFound(responseModel);
            }
            Order order = _orderService.GetOrderById(model.Id);
            if (order == null || order.Deleted)
            {
                responseModel.StatusCode = (int)ErrorType.NotFound;
                responseModel.ErrorList.Add(ORDER_NOT_FOUND);
                return NotFound(responseModel);
            }
            foreach (OrderItemModel modelOrderItem in model.Items)
            {
                OrderItem updateOrderItem = order.OrderItems.FirstOrDefault(x => x.Id == modelOrderItem.Id);
                if (updateOrderItem != null)
                {
                    //quantity = updateOrderItem.Quantity;
                    //updateOrderItem.ProductId = modelOrderItem.ProductId;
                    //updateOrderItem.UnitPriceInclTax = modelOrderItem.UnitPriceInclTaxValue;
                    //updateOrderItem.UnitPriceExclTax = modelOrderItem.UnitPriceExclTaxValue;
                    //updateOrderItem.Quantity = Convert.ToInt32(modelOrderItem.Quantity);
                    //updateOrderItem.DiscountAmountInclTax = modelOrderItem.DiscountInclTaxValue;
                    //updateOrderItem.DiscountAmountExclTax = modelOrderItem.DiscountExclTaxValue;
                    ////updateOrderItem.PriceExclTax = (modelOrderItem.SubTotalExclTaxValue;
                    ////updateOrderItem.PriceInclTax = modelOrderItem.SubTotalInclTaxValue;
                    //updateOrderItem.PriceExclTax = modelOrderItem.UnitPriceExclTaxValue;
                    //updateOrderItem.PriceInclTax = modelOrderItem.UnitPriceInclTaxValue;

                    ////updateOrderItem.PriceExclTax = (modelOrderItem.SubTotalExclTaxValue* Convert.ToInt32(modelOrderItem.Quantity));
                    ////updateOrderItem.PriceInclTax = (modelOrderItem.SubTotalInclTaxValue * Convert.ToInt32(modelOrderItem.Quantity));
                    //updateOrderItem.DownloadCount = Convert.ToInt32(modelOrderItem.DownloadCount);
                    //updateOrderItem.IsDownloadActivated = modelOrderItem.IsDownloadActivated;


                    //quantity = updateOrderItem.Quantity;
                    //updateOrderItem.ProductId = modelOrderItem.ProductId;
                    //updateOrderItem.UnitPriceInclTax = modelOrderItem.UnitPriceInclTaxValue;
                    //updateOrderItem.UnitPriceExclTax = modelOrderItem.UnitPriceExclTaxValue;
                    //updateOrderItem.Quantity = Convert.ToInt32(modelOrderItem.Quantity);
                    //updateOrderItem.DiscountAmountInclTax = modelOrderItem.DiscountInclTaxValue;
                    //updateOrderItem.DiscountAmountExclTax = modelOrderItem.DiscountExclTaxValue;
                    ////updateOrderItem.PriceExclTax = (modelOrderItem.SubTotalExclTaxValue;
                    ////updateOrderItem.PriceInclTax = modelOrderItem.SubTotalInclTaxValue;
                    //updateOrderItem.PriceExclTax = (modelOrderItem.UnitPriceExclTaxValue * Convert.ToInt32(modelOrderItem.Quantity));
                    //updateOrderItem.PriceInclTax = (modelOrderItem.UnitPriceInclTaxValue * Convert.ToInt32(modelOrderItem.Quantity));

                    ////updateOrderItem.PriceExclTax = (modelOrderItem.SubTotalExclTaxValue* Convert.ToInt32(modelOrderItem.Quantity));
                    ////updateOrderItem.PriceInclTax = (modelOrderItem.SubTotalInclTaxValue * Convert.ToInt32(modelOrderItem.Quantity));
                    //updateOrderItem.DownloadCount = Convert.ToInt32(modelOrderItem.DownloadCount);
                    //updateOrderItem.IsDownloadActivated = modelOrderItem.IsDownloadActivated;

                    int mainquantity = 0;
                    string mynumber = Regex.Replace(updateOrderItem.AttributeDescription, @"\D", "");
                    if (mynumber != null && mynumber != "")
                    {
                        if (Convert.ToInt32(mynumber) > 0)
                        {
                            //mainquantity = orderitem.Quantity;
                            mainquantity = (Convert.ToInt32(mynumber) * updateOrderItem.Quantity);
                        }
                        else
                        {
                            mainquantity = updateOrderItem.Quantity;
                        }
                    }
                    else
                    {
                        mainquantity = updateOrderItem.Quantity;
                    }
                    if (updateOrderItem.Quantity == 0)
                    {
                        _productService.AdjustInventory(updateOrderItem.Product, mainquantity, updateOrderItem.AttributesXml,
                   string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.DeleteOrderItem"), order.Id));
                        
                        //delete item
                        _orderService.DeleteOrderItem(updateOrderItem);

                        //update order totals
                        UpdateOrderParameters updateOrderParameters = new UpdateOrderParameters
                        {
                            UpdatedOrder = order,
                            UpdatedOrderItem = updateOrderItem
                        };
                        _orderProcessingService.UpdateOrderTotals(updateOrderParameters);

                        //add a note
                        order.OrderNotes.Add(new OrderNote
                        {
                            Note = "Order item has been deleted",
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                        _orderService.UpdateOrder(order);
                    }
                    else
                    {
                        
                      
                        if (modelOrderItem.Quantity > 0)
                        {
                            var qtyDifference = updateOrderItem.Quantity - modelOrderItem.Quantity ;

                          

                            //adjust inventory
                            _productService.AdjustInventory(updateOrderItem.Product, qtyDifference, updateOrderItem.AttributesXml,
                                string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.EditOrder"), order.Id));
                        }

                        var updateOrderParameters = new UpdateOrderParameters
                        {
                            UpdatedOrder = order,
                            UpdatedOrderItem = updateOrderItem,
                            PriceInclTax = updateOrderItem.UnitPriceInclTax,
                            PriceExclTax = updateOrderItem.UnitPriceExclTax,
                            DiscountAmountInclTax = updateOrderItem.DiscountAmountInclTax,
                            DiscountAmountExclTax = updateOrderItem.DiscountAmountExclTax,
                            SubTotalInclTax = (Convert.ToInt32(modelOrderItem.Quantity)*modelOrderItem.SubTotalInclTaxValue),
                            SubTotalExclTax = (Convert.ToInt32(modelOrderItem.Quantity) * modelOrderItem.SubTotalExclTaxValue),
                            Quantity = modelOrderItem.Quantity
                        };
                        _orderProcessingService.UpdateOrderTotals(updateOrderParameters);



                        //order.OrderItems.Add(updateOrderItem);
                   //     _productService.AdjustInventory(updateOrderItem.Product, mainquantity, updateOrderItem.AttributesXml,
                   //string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.DeleteOrderItem"), order.Id));

                   //     //delete item
                   //    // _orderService.DeleteOrderItem(updateOrderItem);

                   //     //update order totals
                   //     UpdateOrderParameters updateOrderParameters = new UpdateOrderParameters
                   //     {
                   //         UpdatedOrder = order,
                   //         UpdatedOrderItem = updateOrderItem
                   //     };
                   //     _orderProcessingService.UpdateOrderTotals(updateOrderParameters);

                   //     //add a note
                   //     order.OrderNotes.Add(new OrderNote
                   //     {
                   //         Note = "Order item has been deleted",
                   //         DisplayToCustomer = false,
                   //         CreatedOnUtc = DateTime.UtcNow
                   //     });
                   //     _orderService.UpdateOrder(order);
                    }
                }
                else
                {

                    var product = _productService.GetProductById(modelOrderItem.ProductId)
               ?? throw new ArgumentException("No product found with the specified id");
                    //Order ordern = _orderService.GetOrderById(model.Id);
                    var orderItem = new OrderItem
                    {
                        OrderItemGuid = Guid.NewGuid(),
                        Order = order,
                        ProductId = modelOrderItem.ProductId,
                        UnitPriceInclTax = modelOrderItem.UnitPriceInclTaxValue,
                        UnitPriceExclTax = modelOrderItem.UnitPriceExclTaxValue,
                        PriceInclTax = modelOrderItem.UnitPriceInclTaxValue,
                        PriceExclTax = modelOrderItem.UnitPriceInclTaxValue,
                        OriginalProductCost = decimal.Zero,
                        AttributeDescription = "",
                        AttributesXml ="",
                        Quantity = Convert.ToInt32(modelOrderItem.Quantity),
                        DiscountAmountInclTax = decimal.Zero,
                        DiscountAmountExclTax = decimal.Zero,
                        DownloadCount = 0,
                        IsDownloadActivated = false,
                        LicenseDownloadId = 0,
                        ItemWeight = decimal.Zero,
                        RentalStartDateUtc = null,
                        RentalEndDateUtc = null,
                        Product= product
                    };
                    
                    order.OrderItems.Add(orderItem);
                    _orderService.UpdateOrder(order);

                    _productService.AdjustInventory(orderItem.Product, -orderItem.Quantity, orderItem.AttributesXml,
                  string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.EditOrder"), order.Id));

                    var presetPrice = _priceCalculationService.GetFinalPrice(product, order.Customer, decimal.Zero, true, orderItem.Quantity);
                    var presetPriceInclTax = _taxService.GetProductPrice(product, presetPrice, true, order.Customer, out _);
                    var presetPriceExclTax = _taxService.GetProductPrice(product, presetPrice, false, order.Customer, out _);

                    //update order totals
                    var updateOrderParameters = new UpdateOrderParameters
                    {
                        UpdatedOrder = order,
                        UpdatedOrderItem = orderItem,
                        PriceInclTax = presetPriceInclTax,
                        PriceExclTax = presetPriceExclTax,
                        SubTotalInclTax = presetPriceInclTax,
                        SubTotalExclTax = presetPriceExclTax,
                        Quantity = Convert.ToInt32(modelOrderItem.Quantity)
                    };
                    _orderProcessingService.UpdateOrderTotals(updateOrderParameters);
                }
            }
            try
            {
                order.EditEndDate = model.EditEndDate;
                order.OrderStatusId = model.OrderStatusId;
                _orderService.UpdateOrder(order);

                IList<Domain.Device> deviceDetails = _deviceService.GetDevicesByCustomerId(order.CustomerId);
                if (deviceDetails.Any())
                {
                    QueuedNotification notification = new QueuedNotification()
                    {
                        DeviceType = (DeviceType)deviceDetails[0].DeviceType,
                        SubscriptionId = deviceDetails[0].SubscriptionId,
                        Message = _localizationService.GetResource("Notification.OrderProcessingForPickup")
                    };
                    //_queuedNotificationApiService.SendNotication(notification);
                    _notificationService.SendNotication(notification);
                }

            }
            catch
            {
                responseModel.StatusCode = (int)ErrorType.NotFound;
                responseModel.ErrorList.Add("Error in Update Order");
                return NotFound(responseModel);
            }
            responseModel.StatusCode = (int)ErrorType.Ok;
            responseModel.SuccessMessage = SUCCESS;
            return Ok(responseModel);
        }

        #endregion
    }
}

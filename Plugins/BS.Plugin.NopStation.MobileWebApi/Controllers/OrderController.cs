using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using BS.Plugin.NopStation.MobileWebApi.Factories;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Microsoft.AspNetCore.Mvc;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Order;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Customer;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Order;
using Nop.Core.Domain.Messages;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Nop.Services.Messages;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public partial class OrderController : BaseApiController
    {
        #region Fields
        private readonly IDeviceService _deviceService;
        private readonly INotificationService _notificationService;
        private IOrderModelFactoryApi _orderModelFactoryApi;
        private readonly IOrderService _orderService;
        private readonly IShipmentService _shipmentService;
        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPaymentService _paymentService;
        private readonly ILocalizationService _localizationService;
        private readonly IPdfService _pdfService;
        private readonly IShippingService _shippingService;
        private readonly ICountryService _countryService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IWebHelper _webHelper;
        private readonly IDownloadService _downloadService;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IStoreContext _storeContext;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPictureService _pictureService;
        private readonly OrderSettings _orderSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly AddressSettings _addressSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly PdfSettings _pdfSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly ICacheManager _cacheManager;
        #endregion

        #region Constructors

        public OrderController(IOrderModelFactoryApi orderModelFactoryApi,
            IOrderService orderService,
            IShipmentService shipmentService,
            IWorkContext workContext,
            ICurrencyService currencyService,
            IPriceFormatter priceFormatter,
            IOrderProcessingService orderProcessingService,
            IDateTimeHelper dateTimeHelper,
            IPaymentService paymentService,
            ILocalizationService localizationService,
            IPdfService pdfService,
            IShippingService shippingService,
            ICountryService countryService,
            IProductAttributeParser productAttributeParser,
            IWebHelper webHelper,
            IDownloadService downloadService,
            IAddressAttributeFormatter addressAttributeFormatter,
            IStoreContext storeContext,
            IOrderTotalCalculationService orderTotalCalculationService,
            CatalogSettings catalogSettings,
            OrderSettings orderSettings,
            TaxSettings taxSettings,
            ShippingSettings shippingSettings,
            AddressSettings addressSettings,
            RewardPointsSettings rewardPointsSettings,
            PdfSettings pdfSettings,
            IPictureService pictureService,
            MediaSettings mediaSettings,
            ICacheManager cacheManager,
            IDeviceService deviceService,
            INotificationService notificationService)
        {
            this._orderModelFactoryApi = orderModelFactoryApi;
            this._orderService = orderService;
            this._shipmentService = shipmentService;
            this._workContext = workContext;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._orderProcessingService = orderProcessingService;
            this._dateTimeHelper = dateTimeHelper;
            this._paymentService = paymentService;
            this._localizationService = localizationService;
            this._pdfService = pdfService;
            this._shippingService = shippingService;
            this._countryService = countryService;
            this._productAttributeParser = productAttributeParser;
            this._webHelper = webHelper;
            this._downloadService = downloadService;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._storeContext = storeContext;
            this._orderTotalCalculationService = orderTotalCalculationService;
            _deviceService = deviceService;
            _notificationService = notificationService;
            this._catalogSettings = catalogSettings;
            this._orderSettings = orderSettings;
            this._taxSettings = taxSettings;
            this._shippingSettings = shippingSettings;
            this._addressSettings = addressSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._pdfSettings = pdfSettings;
            this._pictureService = pictureService;
            this._mediaSettings = mediaSettings;
            this._cacheManager = cacheManager;
        }

        #endregion



        #region Methods

        //My account / Orders api
        [HttpGet]
        [Route("api/order/customerorders")]
        public IActionResult CustomerOrders()
        {
            // Commented by Alexandar Rajavel on 06-Mar-2019 for Guest user
            //if (!_workContext.CurrentCustomer.IsRegistered())
            //    return Challenge(HttpStatusCode.Unauthorized.ToString());

            var model = _orderModelFactoryApi.PrepareCustomerOrderListModel();
            return Ok(model);
        }


        [HttpGet]
        [Route("api/order/details/{orderId}")]
        public IActionResult Details(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return Unauthorized();

            var model = _orderModelFactoryApi.PrepareOrderDetailsModel(order);

            return Ok(model);
        }

        // For Ware house management
        [HttpGet]
        [Route("api/order/detailsforwhm/{orderId}")]
        public IActionResult DetailsForWHM(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            var response = new NewRegisterResponseModel<OrderDetailsForWHM>();
            if (order == null || order.Deleted)
            {
                response.StatusCode = (int)ErrorType.NotFound;
                response.ErrorList.Add(ID_NOT_FOUND);
            }
            else
            {
                response.SuccessMessage = SUCCESS;
                response.Data = _orderModelFactoryApi.PrepareOrderDetailsModelForWHM(order);
            }
            return Ok(response);
        }


        ////My account / Order details page / PDF invoice
        [HttpGet]
        [Route("api/order/getpdfinvoice/{orderId}")]
        //My account / Order details page / PDF invoice
        public virtual IActionResult GetPdfInvoice(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return Challenge();

            var orders = new List<Order>();
            orders.Add(order);
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintOrdersToPdf(stream, orders, _workContext.WorkingLanguage.Id);
                bytes = stream.ToArray();
            }
            return File(bytes, MimeTypes.ApplicationPdf, $"order_{order.Id}.pdf");
        }

        ////My account / Order details page / re-order
        [HttpGet]
        [Route("api/order/reorder/{orderId}")]
        public IActionResult ReOrder(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return Challenge();

            var response = new GeneralResponseModel<bool>()
            {
                Data = true
            };
            _orderProcessingService.ReOrder(order);

            return Ok(response);
        }




        [HttpGet]
        [Route("api/order/shipmentdetails/{shipmentId}")]
        public IActionResult ShipmentDetails(int shipmentId)
        {
            var shipment = _shipmentService.GetShipmentById(shipmentId);
            if (shipment == null)
                return Challenge();

            var order = shipment.Order;
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return Challenge();
            var model = _orderModelFactoryApi.PrepareShipmentDetailsModel(shipment);

            return Ok(model);

        }

        /// Created By: Sunil Kumar
        /// Created on : 3 Oct 2018
        /// Get Order List By Delivery BoyId
        [HttpGet]
        [Route("api/order/GetOrderListByDeliveryBoyId/{deliveryBoyId}")]
        public IActionResult GetOrderListByDeliveryBoyId(int deliveryBoyId)
        {
            var order = _orderService.GetOrderListByDeliveryBoyId(deliveryBoyId);
            if (order == null)
                return Challenge(HttpStatusCode.Unauthorized.ToString());
            var model = _orderModelFactoryApi.PrepareOrderDetailsModel(order);

            return Ok(model);
        }

        /// Created By: Sunil Kumar
        /// Created on : 2nd Jan 2020
        //Delivery Status from Ware House
        [HttpPost]
        [Route("api/order/DeliveryStatusFromWH")]
        public IActionResult DeliveryStatusFromWH([FromBody]OrderDetailsQueryModel model)
        {
            var orderID = model.OrderID;
            var order = _orderService.GetOrderById(Convert.ToInt32(orderID));
            var response = new NewRegisterResponseModel<DeliveryStatusFromWHResponseModel>();

            var deliveryStatus = new DeliveryStatusFromWHResponseModel();
            if (order == null || order.Deleted || !(model.ReadForDelivery.ToLower() == "true"))
            {
                response.StatusCode = (int)ErrorType.NotFound;
                response.ErrorList.Add(ID_NOT_FOUND);
            }
            else
            {
                deliveryStatus.OrderID = orderID;
                deliveryStatus.SentToDelivery = "true";
                response.SuccessMessage = SUCCESS;
                response.Data = deliveryStatus;
            }
            return Ok(response);
        }

        ////My account / Order details page / re-order
        [HttpPost]
        [Route("api/order/PushNotification/{orderId}")]
        public IActionResult DeliveryPushNotification(int orderId)
        {

            var baseResponse = new BaseResponse();
            var order = _orderService.GetOrderById(orderId);
         

            var deviceDetails = _deviceService.GetDevicesByCustomerId(order.CustomerId);
            if (deviceDetails.Any())
            {
                var notification = new QueuedNotification()
                {
                    DeviceType = (DeviceType)deviceDetails[0].DeviceType,
                    SubscriptionId = deviceDetails[0].SubscriptionId,
                    Message = _localizationService.GetResource("Notification.DeliveryNoti"),
                };
                _notificationService.SendNotication(notification);
                baseResponse.StatusCode = (int)ErrorType.Ok;
                return Ok(baseResponse);
            }
            else
            {
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Device Not Registered with Mobile App");
                return Ok(baseResponse);
            }
        }
        #endregion
    }
}

using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Factories;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Customer;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Customer;
using BS.Plugin.NopStation.MobileWebApi.Models.DeliveryBoy;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.DeliveryBoy;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Review;
using Nop.Core.Domain.Shipping;
using Nop.Services.Customers;
using Nop.Services.DeliveryBoy;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    // Added by Alexandar Rajavel on 17-Jan-2019
    public class DeliveryBoyController : BaseApiController
    {
        #region Field
        private readonly CustomerSettings _customerSettings;
        private readonly ILocalizationService _localizationService;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly ILanguageService _languageService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerModelFactoryApi _customerModelFactoryApi;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IOrderService _orderService;
        private readonly IAppAndAddressReviewService _AppAndAddressReviewService;
        private readonly INotificationService _notificationService;
        private readonly ShippingSettings _shippingSettings;
        private readonly IAddressModelFactoryApi _addressModelFactoryApi;
        #endregion

        #region Constructor
        public DeliveryBoyController(
            CustomerSettings customerSettings,
            ILocalizationService localizationService,
            RewardPointsSettings rewardPointsSettings,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            ILanguageService languageService,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            IWorkContext workContext,
            ICustomerModelFactoryApi customerModelFactoryApi,
            IShoppingCartService shoppingCartService,
            ICustomerActivityService customerActivityService,
            IOrderService orderService,
            IAppAndAddressReviewService AppAndAddressReviewService,
            INotificationService notificationService,
            ShippingSettings shippingSettings,
            IAddressModelFactoryApi addressModelFactoryApi
        )
        {
            _customerSettings = customerSettings;
            _localizationService = localizationService;
            _rewardPointsSettings = rewardPointsSettings;
            _externalAuthenticationSettings = externalAuthenticationSettings;
            _languageService = languageService;
            _customerRegistrationService = customerRegistrationService;
            _customerService = customerService;
            _workContext = workContext;
            _customerModelFactoryApi = customerModelFactoryApi;
            _shoppingCartService = shoppingCartService;
            _customerActivityService = customerActivityService;
            _orderService = orderService;
            _AppAndAddressReviewService = AppAndAddressReviewService;
            _notificationService = notificationService;
            _shippingSettings = shippingSettings;
            _addressModelFactoryApi = addressModelFactoryApi;
        }
        #endregion

        #region Action methods
        [Route("api/deliveryboy/login")]
        [HttpPost]
        public IActionResult Login([FromBody]LoginQueryModel model)
        {
            var customerLoginModel = new LogInPostResponseModel();
            customerLoginModel.StatusCode = (int)ErrorType.NotOk;
            ValidationExtension.LoginValidatorForDeliveryApp(ModelState, model, _localizationService);
            if (ModelState.IsValid)
            {
                model.Username = model.Username.Trim();
                var loginResult = _customerRegistrationService.ValidateCustomer(model.Username, model.Password);
                switch (loginResult)
                {
                    case CustomerLoginResults.Successful:
                        {
                            var customer = _customerService.GetCustomerByUsername(model.Username);
                            customerLoginModel = _customerModelFactoryApi.PrepareCustomerLoginModel(customerLoginModel, customer);
                            customerLoginModel.StatusCode = (int)ErrorType.Ok;
                            break;
                        }
                    case CustomerLoginResults.CustomerNotExist:
                        customerLoginModel.ErrorList = new List<string>
                        {
                            _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist")
                        };
                        break;
                    case CustomerLoginResults.Deleted:

                        customerLoginModel.ErrorList = new List<string>
                        {
                            _localizationService.GetResource("Account.Login.WrongCredentials.Deleted")
                        };
                        break;
                    case CustomerLoginResults.NotActive:

                        customerLoginModel.ErrorList = new List<string>
                        {
                            _localizationService.GetResource("Account.Login.WrongCredentials.NotActive")
                        };
                        break;
                    case CustomerLoginResults.NotRegistered:

                        customerLoginModel.ErrorList = new List<string>
                        {
                            _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered")
                        };
                        break;
                    case CustomerLoginResults.WrongPassword:
                    default:

                        customerLoginModel.ErrorList = new List<string>
                        {
                            _localizationService.GetResource("Account.Login.WrongCredentials")
                        };
                        break;
                }
            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        customerLoginModel.ErrorList.Add(error.ErrorMessage);
                    }
                }
            }
            //If we got this far, something failed, redisplay form
            return Ok(customerLoginModel);
        }

        [Route("api/deliveryboy/DeliveryDetailsByOrderNumber/{orderNumber}")]
        [HttpGet]
        public IActionResult DeliveryDetailsByOrderNumber(string orderNumber)
        {
            var order = _orderService.GetOrderByCustomOrderNumberForDelivery(orderNumber);
            if (order != null)
            {
                var orderList = new List<Order>();
                orderList.Add(order);
                return Ok(PrepareResponseModel(orderList));
            }
            else
            {
                var orderResult = new DeliveryProductsResponse();
                orderResult.StatusCode = (int)ErrorType.NotFound;
                orderResult.ErrorList.Add(NO_DATA);
                return Ok(orderResult);
            }
        }

        [Route("api/deliveryboy/DeliveryProductList")]
        [HttpGet]
        public IActionResult DeliveryProductList()
        {
            var orders = _orderService.GetOrderListByDeliveryBoyId(_workContext.CurrentCustomer.Id);
            if (orders != null && orders.Any())
            {
                return Ok(PrepareResponseModel(orders));
            }
            else
            {
                var orderResult = new DeliveryProductsResponse();
                orderResult.StatusCode = (int)ErrorType.NotFound;
                orderResult.ErrorList.Add(NO_DATA);
                return Ok(orderResult);
            }
        }

        [Route("api/deliveryboy/AcceptToDeliver/{orderNumber}/{locationName}")]
        [HttpPost]
        public IActionResult AcceptToDeliver(string orderNumber, string locationName)
        {
            var responseModel = new BaseResponse();
            if (string.IsNullOrEmpty(orderNumber) || string.IsNullOrEmpty(locationName))
            {
                responseModel.StatusCode = (int)ErrorType.NotOk;
                responseModel.ErrorList.Add(ORDERNO_AND_LOCATION_CAN_NOT_BE_EMPTY);
                return BadRequest(responseModel);
            }
            else
            {
                var order = _orderService.GetOrderByCustomOrderNumber(orderNumber);
                if (order != null)
                {
                    order.IsDeliveryAccepted = true;
                    order.DeliveryBoyId = _workContext.CurrentCustomer.Id;
                    order.OrderNotes.Add(new Nop.Core.Domain.Orders.OrderNote { CreatedOnUtc = DateTime.UtcNow, OrderId = order.Id, DisplayToCustomer = true, Note = _localizationService.GetResource("Delivery.Products.Location") + " " + locationName });
                    _orderService.UpdateOrder(order);
                    var smsObj = new SMSRequest()
                    {
                        DestinationNumber = order.BillingAddress.PhoneNumber.Remove(1, 3),
                        Message = string.Format(_localizationService.GetResource("SMSDeliveryCode"), order.DeliveryCode),
                        Application = _localizationService.GetResource("Application_Name")
                    };
                    // Send delivery code to customer via SMS
                    //_queuedNotificationApiService.SendSMS(smsObj);
                    _notificationService.SendSMS(smsObj);
                    responseModel.StatusCode = (int)ErrorType.Ok;
                    responseModel.SuccessMessage = SUCCESS;
                    return Ok(responseModel);
                }
                else
                {
                    responseModel.StatusCode = (int)ErrorType.NotFound;
                    responseModel.ErrorList.Add(ORDER_NOT_FOUND);
                    return NotFound(responseModel);
                }
            }
        }

        [Route("api/deliveryboy/VerifyDeliveryCode/{orderNumber}/{deliveryCode}")]
        [HttpPost]
        public IActionResult VerifyDeliveryCode(string orderNumber, string deliveryCode)
        {
            var responseModel = new BaseResponse();
            if (string.IsNullOrEmpty(orderNumber) || string.IsNullOrEmpty(deliveryCode))
            {
                responseModel.StatusCode = (int)ErrorType.NotOk;
                responseModel.ErrorList.Add(ORDERNO_AND_DELIVERYCODE_CAN_NOT_BE_EMPTY);
                return BadRequest(responseModel);
            }
            else
            {
                var order = _orderService.GetOrderByCustomOrderNumber(orderNumber);
                if (order != null)
                {
                    if (order.DeliveryCode == deliveryCode)
                    {
                        order.DeliveryBoyId = _workContext.CurrentCustomer.Id;
                        order.ShippingStatusId = (int)ShippingStatus.Delivered;
                        order.OrderNotes.Add(new Nop.Core.Domain.Orders.OrderNote { CreatedOnUtc = DateTime.UtcNow, OrderId = order.Id, DisplayToCustomer = true, Note = _localizationService.GetResource("Enums.Nop.Core.Domain.Shipping.ShippingStatus.Delivered") });
                        _orderService.UpdateOrder(order);

                        var smsObj = new SMSRequest()
                        {
                            DestinationNumber = order.BillingAddress.PhoneNumber.Remove(1, 3),
                            Message = string.Format(_localizationService.GetResource("Delivered_Success_Message"), order.CustomOrderNumber),
                            Application = _localizationService.GetResource("Application_Name")
                        };
                        // Send delivered message
                        _notificationService.SendSMS(smsObj);

                        responseModel.StatusCode = (int)ErrorType.Ok;
                        responseModel.SuccessMessage = SUCCESS;
                        return Ok(responseModel);
                    }
                    else
                    {
                        responseModel.StatusCode = (int)ErrorType.NotFound;
                        responseModel.ErrorList.Add(DELIVERYCODE_MISMATCH);
                        return NotFound(responseModel);
                    }
                }
                else
                {
                    responseModel.StatusCode = (int)ErrorType.NotFound;
                    responseModel.ErrorList.Add(ORDER_NOT_FOUND);
                    return NotFound(responseModel);
                }
            }
        }


        [Route("api/deliveryboy/AssignOrderToDeliveryBoy/{OrderId}")]
        [HttpPost]
        public IActionResult AssignOrderToDeliveryBoy(int OrderId)
        {
            var responseModel = new BaseResponse();
            if (OrderId==0)
            {
                responseModel.StatusCode = (int)ErrorType.NotOk;
                responseModel.ErrorList.Add(ORDERNO_AND_DELIVERYCODE_CAN_NOT_BE_EMPTY);
                return BadRequest(responseModel);
            }
            else
            {
                var order = _orderService.GetOrderById(OrderId);
                if (order != null)
                {
                    if (order.DeliveryBoyId == 0 || order.DeliveryBoyId==null)
                    {
                        order.DeliveryBoyId = _workContext.CurrentCustomer.Id;
                         _orderService.UpdateOrder(order);

                      

                        responseModel.StatusCode = (int)ErrorType.Ok;
                        responseModel.SuccessMessage = SUCCESS;
                        return Ok(responseModel);
                    }
                    else
                    {
                        responseModel.StatusCode = (int)ErrorType.Ok;
                        responseModel.ErrorList.Add(ORDER_ALREADY_ASSIGNED);
                        return Ok(responseModel);
                    }
                }
                else
                {
                    responseModel.StatusCode = (int)ErrorType.Ok;
                    responseModel.ErrorList.Add(ORDER_NOT_FOUND);
                    return Ok(responseModel);
                }
            }
        }

        [Route("api/reviews/ReviewsOfAppAndAddress")]
        [HttpPost]
        public IActionResult ReviewsOfAppAndAddress([FromBody]ReviewRequest queryModel)
        {
            ValidationExtension.WriteReviewRequestValidator(ModelState, queryModel, _localizationService);
            var responseModel = new BaseResponse();
            int rating = queryModel.Rating;
            if (rating < 0 || rating > 5)
            {
                responseModel.StatusCode = (int)ErrorType.NotOk;
                responseModel.ErrorList.Add(INCORRECT_REVIEW_RATING);
                return Ok(responseModel);
            }
            if (ModelState.IsValid)
            {
                var order = _orderService.GetOrderByCustomOrderNumber(queryModel.OrderNo);
                if (order != null)
                {
                    var reviewRequest = new AppNAddressReview();
                    reviewRequest.Rating = queryModel.Rating;
                    reviewRequest.ReviewText = queryModel.ReviewText.Trim();

                    reviewRequest.OrderNo = queryModel.OrderNo;
                    reviewRequest.AddressId = order.BillingAddress.Id;
                    reviewRequest.ReviewTypeId = (int)(ReviewTypeEnum)queryModel.ReviewType;
                    reviewRequest.CustomerId = _workContext.CurrentCustomer.Id;

                    var review = _AppAndAddressReviewService.GetAppAndAddressReview(queryModel.OrderNo, _workContext.CurrentCustomer.Id, (int)(ReviewTypeEnum)queryModel.ReviewType);
                    if (review != null)
                    {
                        review.Rating = queryModel.Rating;
                        review.ReviewText = queryModel.ReviewText.Trim();
                        review.AddressId = order.BillingAddress.Id;
                        _AppAndAddressReviewService.UpdateAppAndAddressReview(review);
                    }
                    else
                    {
                        _AppAndAddressReviewService.InsertAppAndAddressReview(reviewRequest);
                    }
                    return Ok(responseModel);
                }
                else
                {
                    responseModel.StatusCode = (int)ErrorType.NotFound;
                    responseModel.ErrorList.Add(ORDER_NOT_FOUND);
                }
            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        responseModel.ErrorList.Add(error.ErrorMessage);
                    }
                }
                responseModel.StatusCode = (int)ErrorType.NotOk;
            }
            return Ok(responseModel);
        }

        [Route("api/deliveryboy/MyDeliveredList")]
        [HttpGet]
        public IActionResult MyDeliveredList()
        {
            var orders = _orderService.GetMyDeliveredList(_workContext.CurrentCustomer.Id);
            if (orders != null && orders.Any())
            {
                return Ok(PrepareResponseModel(orders.OrderByDescending(o => o.CreatedOnUtc).Take(_shippingSettings.ShowDeliveredListCount).ToList()));
            }
            else
            {
                var orderResult = new DeliveryProductsResponse();
                orderResult.StatusCode = (int)ErrorType.NotFound;
                orderResult.ErrorList.Add(NO_DATA);
                return Ok(orderResult);
            }
        }

        private DeliveryProductsResponse PrepareResponseModel(IList<Order> orders)
        {
            var orderResult = new DeliveryProductsResponse();
            foreach (var order in orders)
            {
                var orderDetails = new DeliveryOrderDetail();
                orderDetails.Address = _addressModelFactoryApi.AddressConcatenate(order.BillingAddress);
                orderDetails.Latitude = order.BillingAddress.Latitude;
                orderDetails.Longitude = order.BillingAddress.Longitude;
                orderDetails.CustomerName = order.BillingAddress.FirstName;
                orderDetails.MobileNumber = order.BillingAddress.PhoneNumber;
                orderDetails.OrderNumber = order.CustomOrderNumber;
                orderDetails.DeliveryCode = order.DeliveryCode;
                var productIds = order.OrderItems.Select(l => l.ProductId).ToList();
                orderDetails.ProductIds = productIds.Select(x => x.ToString()).ToArray();
                orderResult.OrderDetails.Add(orderDetails);
            }
            return orderResult;
        }
        #endregion
    }
}

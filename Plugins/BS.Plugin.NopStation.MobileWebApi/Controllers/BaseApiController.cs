using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using Nop.Services.Common;
using Nop.Services.Logging;
using Nop.Services.Stores;
using Nop.Web.Framework;
using BS.Plugin.NopStation.MobileWebApi.Infrastructure;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{

    //[Route("api/[controller]")]
    [TokenAuthorize]
    [CustomerLastActivityAuthorize]
    [NstAuthorization]
    public class BaseApiController : Controller
    {
        // Added by Alexandar Rajavel
        public const string INTERNAL_SERVER_ERROR = "Internal server error.";
        public const string CONTACTS_INSERTED_SUCCESSFULLY = "Contacts inserted Successfully.";
        public const string CONTACTS_UPDATED_SUCCESSFULLY = "Contacts inserted Successfully.";
        public const string NO_DATA = "No data.";
        public const string ID_NOT_FOUND = "Id not found";
        public const string NOT_APPROVED = "Not approved your requestId";
        public const string ORDER_NOT_FOUND = "Order not found.";
        public const string INCORRECT_REVIEW_RATING = "Incorrect Review Rating";
        public const string DELIVERYCODE_MISMATCH = "Delivery Code Mismatch.";
        public const string ORDERNO_AND_LOCATION_CAN_NOT_BE_EMPTY = "OrderNumber and LocationName can not be empty.";
        public const string ORDERNO_AND_DELIVERYCODE_CAN_NOT_BE_EMPTY = "OrderNumber and DeliveryCode can not be empty.";
        public const string SUCCESS = "Success.";
        public const string FAILURE = "Failed";
        public const string MOBILENO_AND_APPLICATION = "Application and DestinationNumber is required";
        public const string ERROR_IN_DESTINATION_NUMBER = "DestinationNumber should start with 0095. E.g., 00959876543210";
        public const string BARCODE_NOT_FOUND = "Barcode not found";
        public const string COMMA = ", ";
        public const string STARTING_NUMBER = "0095";
        public const string PAYMENT_STATUS_SUCCESS = "000";
        public const string PAYMENT_STATUS_PENDING = "001";
        public const string ORDER_ALREADY_ASSIGNED = "This is order is already assigned for Delivery, Scan another order.";

        /// <summary>
        /// Log exception
        /// </summary>
        /// <param name="exc">Exception</param>
        protected void LogException(Exception exc)
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var logger = EngineContext.Current.Resolve<ILogger>();

            var customer = workContext.CurrentCustomer;
            logger.Error(exc.Message, exc, customer);
        }
        public int GetCustomerIdFromHeader()
        {
            StringValues headerValues;
            var secretKey = Constant.SecretKey;

            Request.Headers.TryGetValue(Constant.TokenName, out headerValues);
            var token = headerValues.FirstOrDefault();
            var payload = JwtHelper.JwtDecoder.DecodeToObject(token, secretKey, true) as IDictionary<string, object>;
            if (payload != null) return Convert.ToInt32(payload[Constant.CustomerIdName]);
            return 0;
        }

        public string GetDeviceIdFromHeader()
        {
            StringValues headerValues;
            var secretKey = Constant.SecretKey;
            var keyFound = Request.Headers.TryGetValue(Constant.DeviceIdName, out headerValues);
            if (headerValues.Count > 0)
            {
                var device = headerValues.FirstOrDefault();
                if (device != null) return device;
            }
            return string.Empty;
        }
        protected internal bool TryUpdateModel<TModel>(TModel model, string prefix) where TModel : class
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            //var bindingContext = new ModelBindingContext
            //{
            //    Model = model,
            //    ModelName = prefix,
            //    ModelState = ModelState
            //};
            return ModelState.IsValid;
        }
        public List<string> GetModelStateErrors(ModelStateDictionary model)
        {
            var erroList = model.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).Distinct();
            return erroList.ToList();
        }
    }
}

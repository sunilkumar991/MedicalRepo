﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileWebApi.Core;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Models.FacebookExternalAuth;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Customer;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Authentication.External;
using Nop.Services.Customers;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Microsoft.AspNetCore.Mvc;
using JWT;
using BS.Plugin.NopStation.MobileWebApi.Infrastructure;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class FacebookExternalAuthController : BaseApiController
    {
        #region Fields
        private readonly IExternalAuthorizerApi _authorizer;
        private readonly ICustomerService _customerService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDeviceService _deviceService;
        private readonly IGenericAttributeService _genericAttributeService;
        #endregion

        #region Constructor
        public FacebookExternalAuthController(IExternalAuthorizerApi authorizer,
            ICustomerService customerService,
            IShoppingCartService shoppingCartService,
            IWorkContext workContext,
            ILocalizationService localizationService,
            ICustomerActivityService customerActivityService,
            IDeviceService deviceService,
            IGenericAttributeService genericAttributeService)
        {
            this._authorizer = authorizer;
            this._customerService = customerService;
            this._shoppingCartService = shoppingCartService;
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._customerActivityService = customerActivityService;
            this._deviceService = deviceService;
            _genericAttributeService = genericAttributeService;
        }
        #endregion

        #region Utilities

        //private void ParseClaims(FacebookExternalAuthModelApi facebookModel, OAuthAuthenticationParametersApi parameters)
        //{
        //    var claims = new UserClaims();
        //    claims.Contact = new ContactClaims();

        //    claims.Contact.Email = facebookModel.Email;

        //    claims.Name = new NameClaims();
        //    if (!String.IsNullOrEmpty(facebookModel.Name))
        //    {
        //        var name = facebookModel.Name;
        //        var nameSplit = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        //        if (nameSplit.Length >= 2)
        //        {
        //            claims.Name.First = nameSplit[0];
        //            claims.Name.Last = nameSplit[1];
        //        }
        //        else
        //        {
        //            claims.Name.Last = nameSplit[0];
        //        }
        //    }

        //    parameters.AddClaim(claims);
        //}

        protected string GetToken(int customerId)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var now = Math.Round((DateTime.UtcNow.AddDays(180) - unixEpoch).TotalSeconds);


            var payload = new Dictionary<string, object>()
                                {
                                    { Constant.CustomerIdName, customerId },
                                    { "exp", now }
                                };
            string secretKey = Constant.SecretKey;
            var token = JwtHelper.JwtEncoder.Encode(payload, secretKey);

            return token;
        }

        void PrepareCustomerInfoModel(LogInPostResponseModel model, Customer customer)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (customer == null)
                throw new ArgumentNullException("customer");
            model.FirstName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute);
            model.LastName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute);
            model.City = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CityAttribute);
            model.StreetAddress = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddressAttribute);
            model.StreetAddress2 = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddress2Attribute);
            model.CountryId = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CountryIdAttribute);
            model.StateProvinceId = _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.StateProvinceIdAttribute);
            model.Phone = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PhoneAttribute);
            model.CustomerId = customer.Id;
            model.Email = customer.Email;
            model.Username = customer.Username;
            model.Token = GetToken(customer.Id);

        }
        #endregion

        #region Action
        [Route("api/facebookLogin")]
        [HttpPost]
        public IActionResult FacebookLogin([FromBody]FacebookExternalAuthModelApi facebookModel)
        {
            //var fromCart = _workContext.CurrentCustomer.ShoppingCartItems.ToList();
            //var parameters = new OAuthAuthenticationParametersApi("ExternalAuth.Facebook")
            //{
            //    ExternalIdentifier = facebookModel.ProviderUserId,
            //    OAuthToken = facebookModel.AccessToken,
            //    OAuthAccessToken = facebookModel.ProviderUserId,
            //};

            //ParseClaims(facebookModel, parameters);

            //var result = _authorizer.Authorize(parameters);
            //var status = result.Status;

            var customerModel = new LogInPostResponseModel();
            //customerModel.StatusCode = (int)ErrorType.NotOk;

            //if (status == OpenAuthenticationStatus.Error)
            //{
            //    foreach (var error in result.Errors)
            //    {
            //        customerModel.ErrorList.Add(error);
            //    }
            //}

            //else if(status == OpenAuthenticationStatus.AutoRegisteredStandard)
            //{
            //    var customer = _customerService.GetCustomerByEmail(facebookModel.Email);
            //    PrepareCustomerInfoModel(customerModel, customer);
            //    customerModel.StatusCode = (int)ErrorType.Ok;
            //    //migrate shopping cart
            //    //_shoppingCartService.MigrateShoppingCart(lastGuestCustomer, customer, true);
            //    for (int i = 0; i < fromCart.Count; i++)
            //    {
            //        var sci = fromCart[i];
            //        _shoppingCartService.AddToCart(customer, sci.Product, sci.ShoppingCartType, sci.StoreId,
            //            sci.AttributesXml, sci.CustomerEnteredPrice,
            //            sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, false);
            //    }
            //    for (int i = 0; i < fromCart.Count; i++)
            //    {
            //        var sci = fromCart[i];
            //        _shoppingCartService.DeleteShoppingCartItem(sci);
            //    }
            //    //activity log
            //    _customerActivityService.InsertActivity("PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"), customer);
            //    string deviceId = GetDeviceIdFromHeader();
            //    var device = _deviceService.GetDeviceByDeviceToken(deviceId);
            //    if (device != null)
            //    {
            //        device.CustomerId = customer.Id;
            //        device.IsRegistered = customer.IsRegistered();
            //        _deviceService.UpdateDevice(device);
            //    }
            //}

            //else if(status == OpenAuthenticationStatus.Authenticated)
            //{
            //    var customer = _customerService.GetCustomerById(result.CustomerId);
            //    PrepareCustomerInfoModel(customerModel, customer);
            //    customerModel.StatusCode = (int)ErrorType.Ok;
            //    //migrate shopping cart
            //    //_shoppingCartService.MigrateShoppingCart(lastGuestCustomer, customer, true);
            //    for (int i = 0; i < fromCart.Count; i++)
            //    {
            //        var sci = fromCart[i];
            //        _shoppingCartService.AddToCart(customer, sci.Product, sci.ShoppingCartType, sci.StoreId,
            //            sci.AttributesXml, sci.CustomerEnteredPrice,
            //            sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, false);
            //    }
            //    for (int i = 0; i < fromCart.Count; i++)
            //    {
            //        var sci = fromCart[i];
            //        _shoppingCartService.DeleteShoppingCartItem(sci);
            //    }
            //    //activity log
            //    _customerActivityService.InsertActivity("PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"), customer);
            //    string deviceId = GetDeviceIdFromHeader();
            //    var device = _deviceService.GetDeviceByDeviceToken(deviceId);
            //    if (device != null)
            //    {
            //        device.CustomerId = customer.Id;
            //        device.IsRegistered = customer.IsRegistered();
            //        _deviceService.UpdateDevice(device);
            //    }
            //}

            return Ok(customerModel);
        }
        #endregion
    }
}

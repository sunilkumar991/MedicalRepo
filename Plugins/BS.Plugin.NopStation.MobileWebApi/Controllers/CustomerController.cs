using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Customers;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Customer;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Customer;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Orders;
using Nop.Services.Orders;
using Nop.Services.Media;
using Nop.Core;
using System.Net;
using Nop.Core.Domain.Tax;
using Nop.Services.Helpers;
using Nop.Services.Messages;
using Nop.Services.Directory;
using Nop.Services.Authentication.External;
using Nop.Core.Domain.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using Nop.Core.Domain.Messages;
using Nop.Services.Authentication;
using Nop.Services.Tax;
using Nop.Core.Domain.Localization;
using System.Collections.Specialized;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using Nop.Services.Stores;
using Nop.Core.Domain.Media;
using BS.Plugin.NopStation.MobileWebApi.Factories;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using Microsoft.AspNetCore.Mvc;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Catalog;
using Nop.Services.Topics;
using Newtonsoft.Json;
using BS.Plugin.NopStation.MobileWebApi.Infrastructure;
using Microsoft.AspNetCore.Http;
using Nop.Services.HelpNSupport;
using BS.Plugin.NopStation.MobileWebApi.Models.HelpNSupport;
using Nop.Services.Payments;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Payment;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Payment;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class CustomerController : BaseApiController
    {
        #region Field
        private readonly ICustomerModelFactoryApi _customerModelFactoryApi;
        private readonly IAddressModelFactoryApi _addressModelFactoryApi;
        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly AddressSettings _addressSettings;
        private readonly ForumSettings _forumSettings;
        private readonly OrderSettings _orderSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly TaxSettings _taxSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly IOrderService _orderService;
        //private readonly IPictureService _pictureService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ITaxService _taxService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IAddressService _addressService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IDownloadService _downloadService;
        private readonly IDeviceService _deviceService;
        private readonly ILanguageService _languageService;
        private readonly ICurrencyService _currencyService;
        private readonly ITopicService _topicService;
        private readonly ICommonModelFactoryApi _commonModelFactoryApi;
        private readonly ICityService _cityService;
        private readonly IContactDetailService _contactDetailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly IQueuedNotificationApiService _queuedNotificationApiService;
        private readonly IHelpandSupportService _helpAndSupportService;
        private readonly IPaymentTransactionHistoryService _paymentTransactionHistoryService;
        private readonly INotificationService _notificationService;

        #endregion

        #region Ctor
        public CustomerController(ICustomerModelFactoryApi customerModelFactoryApi,
            IAddressModelFactoryApi addressModelFactoryApi,
            CustomerSettings customerSettings,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            RewardPointsSettings rewardPointsSettings,
            AddressSettings addressSettings,
            ForumSettings forumSettings,
            OrderSettings orderSettings,
            DateTimeSettings dateTimeSettings,
            TaxSettings taxSettings,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            LocalizationSettings localizationSettings,
            MediaSettings mediaSettings,
            IOrderService orderService,
            IShoppingCartService shoppingCartService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IDateTimeHelper dateTimeHelper,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ICustomerAttributeService customerAttributeService,
            IExternalAuthenticationService externalAuthenticationService,
            ICustomerAttributeParser customerAttributeParser,
            IGenericAttributeService genericAttributeService,
            IAuthenticationService authenticationService,
            ITaxService taxService,
            IWorkflowMessageService workflowMessageService,
            IStoreMappingService storeMappingService,
            IAddressService addressService,
            IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            IAddressAttributeFormatter addressAttributeFormatter,
            IDownloadService downloadService,
            IReturnRequestService returnRequestService,
            IDeviceService deviceService,
            ILanguageService languageService,
            ICurrencyService currencyService,
            ITopicService topicService,
            ICommonModelFactoryApi commonModelFactoryApi,
            ICityService cityService,
            IContactDetailService contactDetailService,
            IHttpContextAccessor httpContextAccessor,
            //IQueuedNotificationApiService queuedNotificationApiService,
            IHelpandSupportService helpAndSupportService,
            IPaymentTransactionHistoryService paymentTransactionHistoryService,
            INotificationService notificationService
        )
        {
            this._customerModelFactoryApi = customerModelFactoryApi;
            this._addressModelFactoryApi = addressModelFactoryApi;
            this._customerSettings = customerSettings;
            this._customerRegistrationService = customerRegistrationService;
            this._customerService = customerService;
            this._customerActivityService = customerActivityService;
            this._localizationService = localizationService;
            this._rewardPointsSettings = rewardPointsSettings;
            this._addressSettings = addressSettings;
            this._forumSettings = forumSettings;
            this._orderSettings = orderSettings;
            this._dateTimeSettings = dateTimeSettings;
            this._taxSettings = taxSettings;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._localizationSettings = localizationSettings;
            this._mediaSettings = mediaSettings;
            this._orderService = orderService;
            this._shoppingCartService = shoppingCartService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._dateTimeHelper = dateTimeHelper;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._customerAttributeService = customerAttributeService;
            this._externalAuthenticationService = externalAuthenticationService;
            this._customerAttributeParser = customerAttributeParser;
            this._genericAttributeService = genericAttributeService;
            this._authenticationService = authenticationService;
            this._taxService = taxService;
            this._workflowMessageService = workflowMessageService;
            this._storeMappingService = storeMappingService;
            this._addressService = addressService;
            this._addressAttributeParser = addressAttributeParser;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._addressAttributeService = addressAttributeService;
            this._downloadService = downloadService;
            this._returnRequestService = returnRequestService;
            this._deviceService = deviceService;
            this._languageService = languageService;
            this._currencyService = currencyService;
            this._topicService = topicService;
            this._commonModelFactoryApi = commonModelFactoryApi;
            this._cityService = cityService;
            this._contactDetailService = contactDetailService;
            _httpContextAccessor = httpContextAccessor;
            //_queuedNotificationApiService = queuedNotificationApiService;
            _helpAndSupportService = helpAndSupportService;
            _paymentTransactionHistoryService = paymentTransactionHistoryService;
            _notificationService = notificationService;
        }
        #endregion

        #region Utility
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

        protected virtual string ParseCustomCustomerAttributes(NameValueCollection form)
        {
            if (form == null)
                throw new ArgumentNullException("form");

            string attributesXml = "";
            var attributes = _customerAttributeService.GetAllCustomerAttributes();
            foreach (var attribute in attributes)
            {
                string controlId = string.Format("customer_attribute_{0}", attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _customerAttributeService.GetCustomerAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                string enteredText = ctrlAttributes.Trim();
                                attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.FileUpload:
                    //not supported customer attributes
                    default:
                        break;
                }
            }

            return attributesXml;
        }

        public void AddressBind(Address model, Address destination, bool trimFields = true)
        {


            if (trimFields)
            {
                if (model.FirstName != null)
                    model.FirstName = model.FirstName.Trim();
                if (model.LastName != null)
                    model.LastName = model.LastName.Trim();
                if (model.Email != null)
                    model.Email = model.Email.Trim();
                if (model.Company != null)
                    model.Company = model.Company.Trim();
                //if (model.City != null)
                //    model.City = model.City.Trim(); //Removed By Ankur On 31/08/2018
                if (model.Address1 != null)
                    model.Address1 = model.Address1.Trim();
                if (model.Address2 != null)
                    model.Address2 = model.Address2.Trim();
                if (model.ZipPostalCode != null)
                    model.ZipPostalCode = model.ZipPostalCode.Trim();
                if (model.PhoneNumber != null)
                    model.PhoneNumber = model.PhoneNumber.Trim();
                if (model.FaxNumber != null)
                    model.FaxNumber = model.FaxNumber.Trim();
            }

            destination.FirstName = model.FirstName;
            destination.LastName = model.LastName;
            destination.Email = model.Email;
            destination.Company = model.Company;
            destination.CountryId = model.CountryId;
            destination.StateProvinceId = model.StateProvinceId;
            destination.CityId = model.CityId;
            destination.Address1 = model.Address1;
            destination.Address2 = model.Address2;
            destination.ZipPostalCode = model.ZipPostalCode;
            destination.PhoneNumber = model.PhoneNumber;
            destination.FaxNumber = model.FaxNumber;
            //added by Sunil Kumar at 8/1/19
            destination.Latitude = model.Latitude;
            destination.Longitude = model.Longitude;



        }

        /// <summary>
        /// This is using to fill response data after login or register
        /// Created By : Alexandar Rajavel
        /// Created On : 24-Sep-2018
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="address">Address</param>
        /// <returns>responseData</returns>
        protected RegisterFromOKDollarQueryModel FillResponseData(ref Customer customer, ref Address address)
        {
            //var data= _genericAttributeService.GetAttributeById()
            var attributes = _genericAttributeService.GetAttributeByCustomerId(customer.Id).ToList();
            var dobAttribute = attributes.Where(g => g.Key.ToLower().Contains("dateofbirth")).ToList();
            var dob = dobAttribute.Any() ? dobAttribute[0].Value : "";
            var genderAttribute = attributes.Where(g => g.Key.ToLower().Contains("gender")).ToList();
            var gender = genderAttribute.Any() ? genderAttribute[0].Value : "";
            DateTime dateOfBirthvalue = DateTime.Now;
            if (!string.IsNullOrEmpty(dob))
            {
                dateOfBirthvalue = Convert.ToDateTime(dob);
            }
            var simId = customer.SimId;

            if (simId == null || simId == "")
            {
                simId = customer.Username;
            }

            var password = simId.Substring(simId.Length - 6);
            var responseData = new RegisterFromOKDollarQueryModel()
            {
                //Added code by Sunil Kumar for empty address validation on 14-04-2020
                Address1 = address != null ? address.Address1 ?? address.Address1 : string.Empty,
                Address2 = address != null ? address.Address2 ?? address.Address2 : string.Empty,
                City = Convert.ToString(address != null ? address.CityId ?? address.CityId : 0),
                Country = Convert.ToString(address != null ? address.CountryId ?? address.CountryId : 0),
                FirstName = address != null ? address.FirstName ?? address.FirstName : string.Empty,
                LastName = address != null ? address.LastName ?? address.LastName : string.Empty,
                MobileNumber = address != null ? address.PhoneNumber ?? address.PhoneNumber : string.Empty,
                DeviceID = customer.DeviceId,
                Email = address != null ? address.Email ?? address.Email : string.Empty,
                Gender = gender,
                OtherEmail = customer.OtherEmailAddresses,
                State = Convert.ToString(address != null ? address.StateProvinceId ?? address.StateProvinceId : 0),
                HouseNo = address != null ? address.HouseNo ?? address.HouseNo : string.Empty,
                FloorNo = address != null ? address.FloorNo ?? address.FloorNo : string.Empty,
                RoomNo = address != null ? address.RoomNo ?? address.RoomNo : string.Empty,
                UserName = customer.Username,
                DateOfBirthDay = dateOfBirthvalue.Day,
                DateofBirthMonth = dateOfBirthvalue.Month,
                DateOfBirthYear = dateOfBirthvalue.Year,
                Simid = simId,
                Password = password,
                Token = GetToken(customer.Id),
                ProfilePictureUrl = customer.ProfilePictureUrl,
                ViberNumber = customer.ViberNumber,
                DisplayAvatar = customer.DisplayAvatar,
                MaritalStatus = customer.MaritalStatus,
                CountryCode = address != null ? address.CountryCode ?? address.CountryCode : string.Empty,
                //added By Sunil Kumar at 29-04-17
                VersionCode = customer.VersionCode,
            };
            responseData.DeviceInfo = null;
            //newResponse.Data = dataObj;
            //update login details
            customer.FailedLoginAttempts = 0;
            customer.CannotLoginUntilDateUtc = null;
            customer.RequireReLogin = false;
            customer.LastLoginDateUtc = DateTime.UtcNow;
            _customerService.UpdateCustomer(customer);
            return responseData;
        }

        protected Customer GetCustomerFromDeviceId()
        {
            var deviceId = GetDeviceIdFromHeader();
            var getGuidForDevice = HelperExtension.GetGuid(deviceId);
            var customer = _customerService.GetCustomerByGuid(getGuidForDevice);
            return customer;
        }

        #endregion

        #region Login
        [Route("api/login")]
        [HttpPost]
        public IActionResult Login([FromBody]LoginQueryModel model)
        {
            var customerLoginModel = new LogInPostResponseModel();
            customerLoginModel.StatusCode = (int)ErrorType.NotOk;
            ValidationExtension.LoginValidator(ModelState, model, _localizationService, _customerSettings);
            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }
                if (model.Username == null && model.Email.Trim() != null)
                {
                    model.Username = model.Email.Trim();
                }
                var loginResult = _customerRegistrationService.ValidateCustomer(_customerSettings.UsernamesEnabled ? model.Username : model.Email, model.Password, model.SimId);

                switch (loginResult)
                {
                    case CustomerLoginResults.Successful:
                        {
                            var customer = _customerSettings.UsernamesEnabled ? _customerService.GetCustomerByUsername(model.Username) : _customerService.GetCustomerByEmail(model.Email);
                            customerLoginModel = _customerModelFactoryApi.PrepareCustomerLoginModel(customerLoginModel, customer);
                            customerLoginModel.StatusCode = (int)ErrorType.Ok;
                            //migrate shopping cart
                            _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);
                            //activity log
                            _customerActivityService.InsertActivity("PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"), customer);
                            //string deviceId = GetDeviceIdFromHeader();
                            //var device = _deviceService.GetDeviceByDeviceToken(deviceId);
                            //if (device != null)
                            //{
                            //    device.CustomerId = customer.Id;
                            //    device.IsRegistered = customer.IsRegistered();
                            //    _deviceService.UpdateDevice(device);
                            //}
                            if (customer.Id > 0)
                            {
                                CustomerRemark remark = new CustomerRemark();
                                remark.CustomerId = customer.Id;
                                remark.NetworkRemark = JsonConvert.SerializeObject(model.DeviceInfo);
                                remark.Published = false;
                                remark.CreatedOnUtc = DateTime.UtcNow;
                                _customerService.InsertCustomerRemark(remark);
                            }
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
        #endregion

        #region Change password

        //public IHttpActionResult ChangePassword()
        //{
        //    if (!_workContext.CurrentCustomer.IsRegistered())
        //        throw new HttpResponseException(HttpStatusCode.Unauthorized);

        //    var model = new ChangePasswordModel();
        //    return Ok(model);
        //}

        [HttpPost]
        [Route("api/customer/changepass")]
        public IActionResult ChangePassword([FromBody]ChangePasswordQueryModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                throw new Exception(message: HttpStatusCode.Unauthorized.ToString());

            var customer = _workContext.CurrentCustomer;
            var response = new GeneralResponseModel<string>();
            if (ModelState.IsValid)
            {
                var changePasswordRequest = new ChangePasswordRequest(customer.Email,
                    true, _customerSettings.DefaultPasswordFormat, model.NewPassword, model.OldPassword);
                var changePasswordResult = _customerRegistrationService.ChangePassword(changePasswordRequest);
                if (changePasswordResult.Success)
                {
                    response.Data = _localizationService.GetResource("Account.ChangePassword.Success");
                    return Ok(response);
                }

                //errors
                foreach (var error in changePasswordResult.Errors)
                {
                    response.StatusCode = (int)ErrorType.NotOk;
                    response.ErrorList.Add(error);
                }

            }


            //If we got this far, something failed, redisplay form
            return Ok(model);
        }

        #endregion

        #region Register
        [HttpGet]
        [Route("api/customer/attributes")]
        public IActionResult CustomerAttributes()
        {
            var model = new List<CustomerAttributeModel>();

            var customerAttributes = _customerAttributeService.GetAllCustomerAttributes();
            foreach (var attribute in customerAttributes)
            {
                var attributeModel = new CustomerAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.Name,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    Type = attribute.AttributeControlType.ToString()
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _customerAttributeService.GetCustomerAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new CustomerAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.Name,
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }
                model.Add(attributeModel);
            }

            var result = new GeneralResponseModel<IList<CustomerAttributeModel>>()
            {
                Data = model
            };

            return Ok(result);
        }

        /*
         * 
         * Old registration method commented by Ankur on 20.Sep.2018
        [HttpPost]
        [Route("api/customer/register")]
        public IActionResult Register([FromBody]RegisterQueryModel model)
        {

            //var customer = _workContext.CurrentCustomer;
            //var customer = new Customer();

            
            var customer = new Customer();

            var form = model.FormValue.ToNameValueCollection();
            var response = new RegisterResponseModel();
            //custom customer attributes
            var customerAttributesXml = ParseCustomCustomerAttributes(form);
            var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
            foreach (var error in customerAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            //Added By Sunil Kumar Kutti
            //insert  DeviceId,SimId (if possible)
            customer.DeviceId = model.DeviceID;
            customer.SimId = model.Simid;

            ValidationExtension.RegisterValidator(ModelState, model, _localizationService, _stateProvinceService, _customerSettings);
            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }

                bool isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
                var registrationRequest = new CustomerRegistrationRequest(customer,
                   model.Email,
                   _customerSettings.UsernamesEnabled ? model.Username : model.Email,
                   model.Password,
                   _customerSettings.DefaultPasswordFormat,
                   _storeContext.CurrentStore.Id,
                   isApproved);
                var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
                if (registrationResult.Success)
                {
                    //customer.CustomerGuid = new Guid();
                    //_customerService.UpdateCustomer(customer);
                    //properties
                    if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    {
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.TimeZoneIdAttribute, model.TimeZoneId);
                    }
                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.VatNumberAttribute, model.VatNumber);

                        string vatName;
                        string vatAddress;
                        var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out vatName, out vatAddress);
                        _genericAttributeService.SaveAttribute(customer,
                            NopCustomerDefaults.VatNumberStatusIdAttribute,
                            (int)vatNumberStatus);
                        //send VAT number admin notification
                        if (!String.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                            _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer, model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);

                    }

                    //form fields
                    if (_customerSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.GenderAttribute, model.Gender);
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FirstNameAttribute, model.FirstName);
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.LastNameAttribute, model.LastName);
                    if (_customerSettings.DateOfBirthEnabled)
                    {
                        DateTime? dateOfBirth = model.ParseDateOfBirth();
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.DateOfBirthAttribute, dateOfBirth);
                    }
                    if (_customerSettings.CompanyEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CompanyAttribute, model.Company);
                    if (_customerSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddressAttribute, model.StreetAddress);
                    if (_customerSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddress2Attribute, model.StreetAddress2);
                    if (_customerSettings.ZipPostalCodeEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.ZipPostalCodeAttribute, model.ZipPostalCode);
                    if (_customerSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CityAttribute, model.City);
                    if (_customerSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CountryIdAttribute, model.CountryId);
                    if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StateProvinceIdAttribute, model.StateProvinceId);
                    if (_customerSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PhoneAttribute, model.Phone);
                    if (_customerSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FaxAttribute, model.Fax);

                    //newsletter
                    if (_customerSettings.NewsletterEnabled)
                    {
                        //save newsletter value
                        var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(model.Email, _storeContext.CurrentStore.Id);
                        if (newsletter != null)
                        {
                            if (model.Newsletter)
                            {
                                newsletter.Active = true;
                                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);
                            }
                            //else
                            //{
                            //When registering, not checking the newsletter check box should not take an existing email address off of the subscription list.
                            //_newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
                            //}
                        }
                        else
                        {
                            if (model.Newsletter)
                            {
                                _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = model.Email,
                                    Active = true,
                                    StoreId = _storeContext.CurrentStore.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                            }
                        }
                    }

                    //save customer attributescustomer/info
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CustomCustomerAttributes, customerAttributesXml);

                    //login customer now
                    if (isApproved)
                        _authenticationService.SignIn(customer, true);

                    //associated with external account (if possible)
                    //TryAssociateAccountWithExternalAccount(customer);

                    //insert default address (if possible)
                    var defaultAddress = new Address
                    {
                        FirstName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute),
                        LastName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute),
                        Email = customer.Email,
                        Company = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CompanyAttribute),
                        CountryId = _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.CountryIdAttribute) > 0 ?
                            (int?)_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.CountryIdAttribute) : null,
                        StateProvinceId = _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.StateProvinceIdAttribute) > 0 ?
                            (int?)_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.StateProvinceIdAttribute) : null,
                        CityId = _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.CityIdAttribute),  //Changed By Ankur Shrivastava on 31/8/2018
                        Address1 = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddressAttribute),
                        Address2 = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddress2Attribute),
                        ZipPostalCode = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.ZipPostalCodeAttribute),
                        PhoneNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PhoneAttribute),
                        FaxNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FaxAttribute),
                        CreatedOnUtc = customer.CreatedOnUtc
                    };
                    if (this._addressService.IsAddressValid(defaultAddress))
                    {
                        //some validation
                        if (defaultAddress.CountryId == 0)
                            defaultAddress.CountryId = null;
                        if (defaultAddress.StateProvinceId == 0)
                            defaultAddress.StateProvinceId = null;
                        //set default address
                        customer.Addresses.Add(defaultAddress);
                        customer.BillingAddress = defaultAddress;
                        customer.ShippingAddress = defaultAddress;
                        _customerService.UpdateCustomer(customer);
                    }

                    //notifications
                    if (_customerSettings.NotifyNewCustomerRegistration)
                        _workflowMessageService.SendCustomerRegisteredNotificationMessage(customer, _localizationSettings.DefaultAdminLanguageId);

                    switch (_customerSettings.UserRegistrationType)
                    {
                        case UserRegistrationType.EmailValidation:
                            {
                                //email validation message
                                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
                                _workflowMessageService.SendCustomerEmailValidationMessage(customer, _workContext.WorkingLanguage.Id);
                                response.SuccessMessage = _localizationService.GetResource("Account.Register.Result.EmailValidation");
                                break;
                                //result
                                //return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.EmailValidation });
                            }
                        case UserRegistrationType.AdminApproval:
                            {
                                response.SuccessMessage = _localizationService.GetResource("Account.Register.Result.AdminApproval");
                                break;
                                //return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.AdminApproval });
                            }
                        case UserRegistrationType.Standard:
                        default:
                            {

                                //send customer welcome message
                                _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);
                                response.SuccessMessage = _localizationService.GetResource("Account.Register.Result.Standard");
                                break;
                                //var redirectUrl = Url.RouteUrl("RegisterResult", new { resultId = (int)UserRegistrationType.Standard });
                                //if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                                //    redirectUrl = _webHelper.ModifyQueryString(redirectUrl, "returnurl=" + HttpUtility.UrlEncode(returnUrl), null);
                                //return Redirect(redirectUrl);
                            }


                    }
                }

                //errors
                foreach (var error in registrationResult.Errors)
                {
                    response.StatusCode = (int)ErrorType.NotOk;
                    response.ErrorList.Add(error);
                }

            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        response.ErrorList.Add(error.ErrorMessage);
                    }
                }
                response.StatusCode = (int)ErrorType.NotOk;
            }
            //If we got this far, something failed, redisplay form
            _workContext.CurrentCustomer = new Customer();
            return Ok(response);
        }
        */

        /// <summary>
        /// This is using to customer register
        /// Created By : Alexandar Rajavel
        /// Created On : 10-Sep-2018
        /// </summary>
        /// <param name="model">Customer to register</param>
        /// <returns>Result</returns>
        [HttpPost]
        [Route("api/customer/register")]
        public IActionResult Register([FromBody]RegisterFromOKDollarQueryModel model)
        {
            //To get id of city, state and country
            var country = _countryService.GetCountryByName(model.Country);
            var countryId = country != null ? country.Id : 0;
            var stateId = 0;
            var cityId = 0;
            var state = _stateProvinceService.GetStateByCountryIdAndStateName(countryId, model.State);
            stateId = state != null ? state.Id : 0;
            var city = _cityService.GetCityByStateIdAndCityName(stateId, model.City);
            cityId = city != null ? city.Id : 0;

            //var response = new RegisterResponseModel();
            //var newResponse = new NewRegisterResponseModel<RegisterFromOKDollarQueryModel>();
            //var address = _addressService.GetAddressByMobileNumber(model.MobileNumber);
            //var customer = new Customer();
            //if (address != null)
            //    customer = _customerService.GetCustomerByAddressId(address.Id);
            //if (customer == null)
            //{
            //insert default address (if possible)
            //var customer = new Customer()
            var customer = new Customer()
            {
                DeviceId = model.DeviceID,
                SimId = model.Simid,
                OtherEmailAddresses = model.OtherEmail,
                Email = model.Email,
                CreatedOnUtc = DateTime.UtcNow // Added By Ankur As Created Date was going default
            };
            var defaultAddress = new Address
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                CountryId = countryId,
                StateProvinceId = stateId,
                CityId = cityId,
                Address1 = model.Address1,
                Address2 = model.Address2,
                PhoneNumber = model.MobileNumber,
                CreatedOnUtc = customer.CreatedOnUtc,
            };
            if (this._addressService.IsAddressValid(defaultAddress))
            {
                //some validation
                if (defaultAddress.CountryId == 0)
                    defaultAddress.CountryId = null;
                if (defaultAddress.StateProvinceId == 0)
                    defaultAddress.StateProvinceId = null;
                //set default address
                customer.Addresses.Add(defaultAddress);
                customer.BillingAddress = defaultAddress;
                customer.ShippingAddress = defaultAddress;
                _workContext.CurrentCustomer = customer;
                _workContext.CurrentCustomer.CustomerAddressMappings.Add(new CustomerAddressMapping { Address = defaultAddress });
            }

            //var form = model.FormValue.ToNameValueCollection();
            var response = new RegisterResponseModel();
            //custom customer attributes
            // var customerAttributesXml = ParseCustomCustomerAttributes(form);
            // var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
            //foreach (var error in customerAttributeWarnings)
            //{
            //    ModelState.AddModelError("", error);
            //}

            ValidationExtension.RegisterValidator(ModelState, model, _localizationService, _stateProvinceService, _customerSettings);
            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.UserName != null)
                {
                    model.UserName = model.UserName.Trim();
                }

                bool isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
                var registrationRequest = new CustomerRegistrationRequest(customer,
                   model.Email,
                   _customerSettings.UsernamesEnabled ? model.UserName : model.Email,
                   model.Password,
                   _customerSettings.DefaultPasswordFormat,
                   _storeContext.CurrentStore.Id,
                   isApproved);
                var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
                if (registrationResult.Success)
                {
                    if (customer.Id > 0)
                    {
                        CustomerRemark remark = new CustomerRemark();
                        remark.CustomerId = customer.Id;
                        remark.NetworkRemark = JsonConvert.SerializeObject(model.DeviceInfo);
                        remark.Published = false;
                        remark.CreatedOnUtc = DateTime.UtcNow;
                        _customerService.InsertCustomerRemark(remark);
                    }
                    //customer.CustomerGuid = new Guid();
                    //_customerService.UpdateCustomer(customer);
                    //properties
                    //if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    //{
                    //    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.TimeZoneIdAttribute, model.TimeZoneId);
                    //}
                    //VAT number
                    //if (_taxSettings.EuVatEnabled)
                    //{
                    //    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.VatNumberAttribute, model.VatNumber);

                    //    string vatName;
                    //    string vatAddress;
                    //    var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out vatName, out vatAddress);
                    //    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.VatNumberStatusIdAttribute, (int)vatNumberStatus);
                    //    send VAT number admin notification
                    //    if (!String.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                    //        _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer, model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);

                    //}

                    //form fields
                    if (_customerSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.GenderAttribute, model.Gender);
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FirstNameAttribute, model.FirstName);
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.LastNameAttribute, model.LastName);
                    if (_customerSettings.DateOfBirthEnabled)
                    {
                        DateTime? dateOfBirth = model.ParseDateOfBirth();
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.DateOfBirthAttribute, dateOfBirth);
                    }
                    //if (_customerSettings.CompanyEnabled)
                    //    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CompanyAttribute, model.Company);
                    if (_customerSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddressAttribute, model.Address1);
                    if (_customerSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddress2Attribute, model.Address2);
                    //if (_customerSettings.ZipPostalCodeEnabled)
                    //    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.ZipPostalCodeAttribute, model.ZipPostalCode);
                    if (_customerSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CityAttribute, model.City);

                    if (_customerSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CountryIdAttribute, countryId);

                    if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StateProvinceIdAttribute, stateId);

                    if (_customerSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PhoneAttribute, model.MobileNumber);
                    //if (_customerSettings.FaxEnabled)
                    //    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FaxAttribute, model.Fax);

                    //newsletter
                    //if (_customerSettings.NewsletterEnabled)
                    //{
                    //    //save newsletter value
                    //    var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(model.Email, _storeContext.CurrentStore.Id);
                    //    if (newsletter != null)
                    //    {
                    //        if (model.Newsletter)
                    //        {
                    //            newsletter.Active = true;
                    //            _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);
                    //        }
                    //        //else
                    //        //{
                    //        //When registering, not checking the newsletter check box should not take an existing email address off of the subscription list.
                    //        //_newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
                    //        //}
                    //    }
                    //    else
                    //    {
                    //        if (model.Newsletter)
                    //        {
                    //            _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
                    //            {
                    //                NewsLetterSubscriptionGuid = Guid.NewGuid(),
                    //                Email = model.Email,
                    //                Active = true,
                    //                StoreId = _storeContext.CurrentStore.Id,
                    //                CreatedOnUtc = DateTime.UtcNow
                    //            });
                    //        }
                    //    }
                    //}

                    //save customer attributes
                    //_genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CustomCustomerAttributes, customerAttributesXml);

                    //login customer now
                    if (isApproved)
                        _authenticationService.SignIn(customer, true);

                    //associated with external account (if possible)
                    //TryAssociateAccountWithExternalAccount(customer);



                    //notifications
                    if (_customerSettings.NotifyNewCustomerRegistration)
                        _workflowMessageService.SendCustomerRegisteredNotificationMessage(customer, _localizationSettings.DefaultAdminLanguageId);

                    switch (_customerSettings.UserRegistrationType)
                    {
                        case UserRegistrationType.EmailValidation:
                            {
                                //email validation message
                                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
                                _workflowMessageService.SendCustomerEmailValidationMessage(customer, _workContext.WorkingLanguage.Id);
                                response.SuccessMessage = _localizationService.GetResource("Account.Register.Result.EmailValidation");
                                break;
                                //result
                                //return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.EmailValidation });
                            }
                        case UserRegistrationType.AdminApproval:
                            {
                                response.SuccessMessage = _localizationService.GetResource("Account.Register.Result.AdminApproval");
                                break;
                                //return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.AdminApproval });
                            }
                        case UserRegistrationType.Standard:
                        default:
                            {

                                //send customer welcome message
                                _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);
                                response.SuccessMessage = _localizationService.GetResource("Account.Register.Result.Standard");
                                break;
                                //var redirectUrl = Url.RouteUrl("RegisterResult", new { resultId = (int)UserRegistrationType.Standard });
                                //if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                                //    redirectUrl = _webHelper.ModifyQueryString(redirectUrl, "returnurl=" + HttpUtility.UrlEncode(returnUrl), null);
                                //return Redirect(redirectUrl);
                            }


                    }
                }

                //errors
                foreach (var error in registrationResult.Errors)
                {
                    response.StatusCode = (int)ErrorType.NotOk;
                    response.ErrorList.Add(error);
                }

            }
            else
            {
                foreach (var states in ModelState)
                {
                    foreach (var error in states.Value.Errors)
                    {
                        response.ErrorList.Add(error.ErrorMessage);
                    }
                }
                response.StatusCode = (int)ErrorType.NotOk;
            }
            //If we got this far, something failed, redisplay form
            //}
            //else
            //{
            //    //var data= _genericAttributeService.GetAttributeById()
            //    var attributes = _genericAttributeService.GetAttributeByCustomerId(customer.Id).ToList();
            //    var gender = attributes.Where(g => g.Key == "Gender");
            //    var dob = attributes.Where(g => g.Key == "DateOfBirth").Select(s => s.Value).ToString();
            //    DateTime dateOfBirthvalue = Convert.ToDateTime(dob.ToString());

            //    var dataObj = new RegisterFromOKDollarQueryModel()
            //    {
            //        Address1 = address.Address1,
            //        Address2 = address.Address2,
            //        City = Convert.ToString(address.CityId),
            //        Country = Convert.ToString(address.CountryId),
            //        FirstName = address.FirstName,
            //        LastName = address.LastName,
            //        MobileNumber = address.PhoneNumber,
            //        DeviceID = customer.DeviceId,
            //        Email = address.Email,
            //        Gender = "",
            //        OtherEmail = customer.OtherEmailAddresses,
            //        State = Convert.ToString(address.StateProvinceId),
            //        UserName = customer.Username,
            //        DateOfBirthDay = dateOfBirthvalue.Day,
            //        DateofBirthMonth = dateOfBirthvalue.Month,
            //        DateOfBirthYear = dateOfBirthvalue.Year,
            //        Simid = customer.SimId,
            //        Password = customer.SimId.Substring(customer.SimId.Length, 6),
            //    };
            //    newResponse.Data = dataObj;
            //}
            return Ok(response);
        }

        /// <summary>
        /// This is using to both register and login purpose based on mode parameter
        /// Created By : Alexandar Rajavel
        /// Created On : 24-Sep-2018
        /// </summary>
        /// <param name="model">Customer to register</param>
        /// <returns>Result</returns>
        [HttpPost]
        [Route("api/customer/registerorlogin")]
        public IActionResult RegisterOrLogin([FromBody]RegisterFromOKDollarQueryModel model)
        {
            //To get id of city, state and country
            var country = _countryService.GetCountryByName(model.Country);
            var countryId = country != null ? country.Id : 0;
            var stateId = 0;
            var cityId = 0;
            var state = _stateProvinceService.GetStateByCountryIdAndStateName(countryId, model.State);
            stateId = state != null ? state.Id : 0;
            var city = _cityService.GetCityByStateIdAndCityName(stateId, model.City);
            cityId = city != null ? city.Id : 0;

            //var response = new RegisterResponseModel();
            var response = new NewRegisterResponseModel<RegisterFromOKDollarQueryModel>();
            var customer = new Customer();
            //if (address != null)
            //    customer = _customerService.GetCustomerByAddressId(address.Id);
            customer = _customerService.GetCustomerByUsername(model.UserName);
            var address = _addressService.GetAddressById(customer != null ? customer.BillingAddressId ?? 0 : 0);
            if (customer == null && model.Mode.ToLower() == "login")
            {
                response.StatusCode = (int)ErrorType.NotOk;
                response.ErrorList.Add("Customer need to register");
                response.SuccessMessage = "Login failed";
                return Ok(response);
            }
            if (customer == null && model.Mode.ToLower() == "register")
            {
                model.Email = model.Email ?? string.Empty;
                customer = new Customer()
                {
                    DeviceId = model.DeviceID,
                    SimId = model.Simid,
                    OtherEmailAddresses = model.OtherEmail,
                    Email = model.Email,
                    CreatedOnUtc = DateTime.UtcNow,
                    ProfilePictureUrl = model.ProfilePictureUrl,
                    ViberNumber = model.ViberNumber,
                    MaritalStatus = model.MaritalStatus,
                    DisplayAvatar = model.DisplayAvatar,
                    IsDisplayEmail = true,
                    //added by Sunil Kumar at 29-04-19
                    VersionCode = 0,
                };

                //insert default address (if possible)
                var defaultAddress = new Address
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    CountryId = countryId == 0 ? 60 : countryId,
                    StateProvinceId = stateId == 0 ? 83 : stateId,
                    CityId = cityId == 0 ? 307 : cityId,
                    Address1 = model.Address1,
                    Address2 = model.Address2,
                    PhoneNumber = model.MobileNumber,
                    CreatedOnUtc = DateTime.UtcNow,
                    HouseNo = model.HouseNo,
                    FloorNo = model.FloorNo,
                    RoomNo = model.RoomNo,
                    CountryCode = model.CountryCode
                };

                //------Start
                var _apiWorkContext = GetCustomerFromDeviceId();
                if (_apiWorkContext != null && string.IsNullOrEmpty(_apiWorkContext.Username))
                {
                    _workContext.CurrentCustomer.Id = _apiWorkContext.Id;
                    _workContext.CurrentCustomer.DeviceId = model.DeviceID;
                    _workContext.CurrentCustomer.SimId = model.Simid;
                    _workContext.CurrentCustomer.OtherEmailAddresses = model.OtherEmail;
                    _workContext.CurrentCustomer.Email = model.Email;
                    _workContext.CurrentCustomer.ProfilePictureUrl = model.ProfilePictureUrl;
                    _workContext.CurrentCustomer.ViberNumber = model.ViberNumber;
                    _workContext.CurrentCustomer.MaritalStatus = model.MaritalStatus;
                    _workContext.CurrentCustomer.DisplayAvatar = model.DisplayAvatar;
                    _workContext.CurrentCustomer.IsDisplayEmail = true;
                }
                else
                {
                    _workContext.CurrentCustomer = customer;
                }

                //_workContext.CurrentCustomer.DeviceId = model.DeviceID;
                //_workContext.CurrentCustomer.SimId = model.Simid;
                //_workContext.CurrentCustomer.OtherEmailAddresses = model.OtherEmail;
                //_workContext.CurrentCustomer.Email = model.Email;
                ////_workContext.CurrentCustomer.CreatedOnUtc = DateTime.UtcNow,
                //_workContext.CurrentCustomer.ProfilePictureUrl = model.ProfilePictureUrl;
                //_workContext.CurrentCustomer.ViberNumber = model.ViberNumber;
                //_workContext.CurrentCustomer.MaritalStatus = model.MaritalStatus;
                //_workContext.CurrentCustomer.DisplayAvatar = model.DisplayAvatar;
                //_workContext.CurrentCustomer.IsDisplayEmail = true;

                _workContext.CurrentCustomer.Addresses.Add(defaultAddress);
                _workContext.CurrentCustomer.BillingAddress = defaultAddress;
                _workContext.CurrentCustomer.ShippingAddress = defaultAddress;
                customer = _workContext.CurrentCustomer;
                //------End

                _workContext.CurrentCustomer.CustomerAddressMappings.Add(new CustomerAddressMapping { Address = defaultAddress });

                ValidationExtension.RegisterValidator(ModelState, model, _localizationService, _stateProvinceService, _customerSettings);
                if (ModelState.IsValid)
                {
                    if (model.UserName != null)
                    {
                        model.UserName = model.UserName.Trim();
                    }

                    bool isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
                    var registrationRequest = new CustomerRegistrationRequest(customer,
                       model.Email,
                       model.UserName,
                       model.Password,
                       _customerSettings.DefaultPasswordFormat,
                       _storeContext.CurrentStore.Id,
                       isApproved);
                    var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
                    if (registrationResult.Success)
                    {
                        if (customer.Id > 0)
                        {
                            CustomerRemark remark = new CustomerRemark();
                            remark.CustomerId = customer.Id;
                            remark.NetworkRemark = JsonConvert.SerializeObject(model.DeviceInfo);
                            remark.Published = false;
                            remark.CreatedOnUtc = DateTime.UtcNow;
                            _customerService.InsertCustomerRemark(remark);
                        }

                        //form fields
                        if (_customerSettings.GenderEnabled)
                            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.GenderAttribute, model.Gender);
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FirstNameAttribute, model.FirstName);
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.LastNameAttribute, model.LastName);
                        if (_customerSettings.DateOfBirthEnabled)
                        {
                            DateTime? dateOfBirth = model.ParseDateOfBirth();
                            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.DateOfBirthAttribute, dateOfBirth);
                        }
                        if (_customerSettings.StreetAddressEnabled)
                            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddressAttribute, model.Address1);
                        if (_customerSettings.StreetAddress2Enabled)
                            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddress2Attribute, model.Address2);
                        if (_customerSettings.CityEnabled)
                            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CityAttribute, model.City);

                        if (_customerSettings.CountryEnabled)
                            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CountryIdAttribute, countryId);

                        if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StateProvinceIdAttribute, stateId);

                        if (_customerSettings.PhoneEnabled)
                            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PhoneAttribute, model.MobileNumber);

                        //login customer now
                        if (isApproved)
                            _authenticationService.SignIn(customer, true);

                        //notifications
                        if (_customerSettings.NotifyNewCustomerRegistration)
                            _workflowMessageService.SendCustomerRegisteredNotificationMessage(customer, _localizationSettings.DefaultAdminLanguageId);

                        switch (_customerSettings.UserRegistrationType)
                        {
                            case UserRegistrationType.EmailValidation:
                                {
                                    //email validation message
                                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
                                    _workflowMessageService.SendCustomerEmailValidationMessage(customer, _workContext.WorkingLanguage.Id);
                                    response.SuccessMessage = _localizationService.GetResource("Account.Register.Result.EmailValidation");
                                    break;
                                }
                            case UserRegistrationType.AdminApproval:
                                {
                                    response.SuccessMessage = _localizationService.GetResource("Account.Register.Result.AdminApproval");
                                    break;
                                }
                            case UserRegistrationType.Standard:
                            default:
                                {

                                    //send customer welcome message
                                    _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);
                                    response.SuccessMessage = _localizationService.GetResource("Account.Register.Result.Standard");
                                    break;
                                }


                        }

                        // Need to fill the response data 
                        address = address == null ? defaultAddress : address;
                        response.Data = FillResponseData(ref customer, ref address);
                        response.Data.Mode = model.Mode;

                        //Added by Alexandar Rajavel on 28-Feb-2019
                        //migrate shopping cart, this will happen when register multiple sim with same device 
                        if (_apiWorkContext != null && !string.IsNullOrEmpty(_apiWorkContext.Username))
                        {
                            _shoppingCartService.MigrateShoppingCartNew(_apiWorkContext, customer);
                        }
                    }

                    //errors
                    foreach (var error in registrationResult.Errors)
                    {
                        response.StatusCode = (int)ErrorType.NotOk;
                        response.ErrorList.Add(error);
                    }

                }
                else
                {
                    foreach (var states in ModelState)
                    {
                        foreach (var error in states.Value.Errors)
                        {
                            response.ErrorList.Add(error.ErrorMessage);
                        }
                    }
                    response.StatusCode = (int)ErrorType.NotOk;
                }
                //If we got this far, something failed, redisplay form
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.Mode) || (model.Mode.ToLower() != "register" && model.Mode.ToLower() != "login"))
                {
                    response.StatusCode = (int)ErrorType.NotOk;
                    response.ErrorList.Add("Mode parameter value should be either login or register");
                    return Ok(response);
                }

                //if it is sim id changed then need to update in db
                if (!string.IsNullOrEmpty(model.Simid) && customer.SimId != model.Simid)
                {
                    customer.SimId = model.Simid;
                    var simId = model.Simid;
                    _customerRegistrationService.UpdateNewPassword(customer, simId.Substring(simId.Length - 6));
                }
                //added at 06-05-19 by Sunil Kumar
                if ((model.VersionCode != null) && customer.VersionCode != model.VersionCode)
                {
                    customer.VersionCode = model.VersionCode ?? 0;
                }
                response.Data = FillResponseData(ref customer, ref address);
                response.Data.Mode = model.Mode;
                response.SuccessMessage = "Successfully loggedin";

                //Added by Alexandar Rajavel on 04-Mar-2019
                //migrate shopping cart, this will happen when registered multiple sim with same device
                var _apiWorkContext = GetCustomerFromDeviceId();
                if (_apiWorkContext != null && _apiWorkContext.Id != customer.Id)
                {
                    _shoppingCartService.MigrateShoppingCartNew(_apiWorkContext, customer);
                }
            }
            return Ok(response);
        }

        /// <summary>
        /// This is using to store customer's contact details
        /// Created By : Alexandar Rajavel
        /// Created On : 09-Oct-2018
        /// </summary>
        /// <param name="model">Customer contacts details</param>
        /// <returns>Result</returns>
        [HttpPost]
        [Route("api/customer/savecontactdetails")]
        public IActionResult SaveContactDetails([FromBody]ContactDetailsQueryModel model)
        {
            var response = new NewRegisterResponseModel<RegisterFromOKDollarQueryModel>();
            try
            {
                if (model != null && !string.IsNullOrEmpty(model.Login.MobileNumber))
                {
                    var conctactDetails = _contactDetailService.GetContactDetailByMobileNumber(model.Login.MobileNumber);
                    if (conctactDetails == null)
                    {
                        conctactDetails = new ContactDetail();
                        conctactDetails.MobileNumber = model.Login.MobileNumber;
                        conctactDetails.ContactDetails = JsonConvert.SerializeObject(model);
                        _contactDetailService.InsertContactDetail(conctactDetails);
                        response.SuccessMessage = CONTACTS_INSERTED_SUCCESSFULLY;
                    }
                    else
                    {
                        conctactDetails.ContactDetails = JsonConvert.SerializeObject(model);
                        _contactDetailService.UpdateContactDetail(conctactDetails);
                        response.SuccessMessage = CONTACTS_UPDATED_SUCCESSFULLY;
                    }
                }
                else
                {
                    response.SuccessMessage = "Mobile number and other details required.";
                    response.StatusCode = (int)ErrorType.NotOk;
                }
            }
            catch (Exception ex)
            {
                response.SuccessMessage = INTERNAL_SERVER_ERROR;
                response.StatusCode = (int)ErrorType.NotOk;
                response.ErrorList.Add(ex.Message);
            }
            return Ok(response);
        }

        /// <summary>
        /// This is using to delete customer by username
        /// Created By : Alexandar Rajavel
        /// Created On : 28-Sep-2018
        /// </summary>
        /// <param name="username">Customer to delete</param>
        /// <returns>Result</returns>
        [HttpDelete]
        [Route("api/customer/deletecustomerbyusername/{username}")]
        public IActionResult DeleteCustomerByUserName(string username)
        {
            var response = new NewRegisterResponseModel<RegisterFromOKDollarQueryModel>();
            try
            {
                var customer = new Customer();
                customer = _customerService.GetCustomerByUsername(username);
                if (customer != null)
                {
                    _customerService.DeleteCustomer(customer);
                    response.SuccessMessage = "User has been deleted.";
                }
                else
                {
                    response.SuccessMessage = "User does not exist.";
                }

            }
            catch (Exception ex)
            {
                response.SuccessMessage = "User has not been deleted.";
                response.StatusCode = (int)ErrorType.NotOk;
                response.ErrorList.Add(ex.Message);
            }
            return Ok(response);
        }

        /// <summary>
        /// Added by Alexandar Rajavel on 06-Mar-2019
        /// This is using to delete customer by DeviceId
        /// </summary>
        /// <param name="deviceId">Customer to delete</param>
        /// <returns>Result</returns>
        [HttpDelete]
        [Route("api/customer/deletecustomerbydeviceid/{deviceId}")]
        public IActionResult DeleteCustomerByDeviceId(string deviceId)
        {
            var response = new NewRegisterResponseModel<RegisterFromOKDollarQueryModel>();
            try
            {
                var getGuidForDevice = HelperExtension.GetGuid(deviceId);
                var customers = _customerService.GetAllCustomerByGuid(getGuidForDevice);
                if (customers.Any())
                {
                    foreach (var customer in customers)
                    {
                        _customerService.DeleteCustomer(customer);
                    }
                    response.SuccessMessage = "User has been deleted.";
                }
                else
                {
                    response.SuccessMessage = "User does not exist.";
                }
            }
            catch (Exception ex)
            {
                response.SuccessMessage = "User has not been deleted.";
                response.StatusCode = (int)ErrorType.NotOk;
                response.ErrorList.Add(ex.Message);
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("api/customer/passwordrecovery")]
        public IActionResult PasswordRecovery([FromBody]PasswordRecoveryModelApi model)
        {
            var customer = _customerService.GetCustomerByEmail(model.Email);
            var result = new BaseResponse();

            if (customer != null && customer.Active && !customer.Deleted)
            {
                //save token and current date
                var passwordRecoveryToken = Guid.NewGuid();
                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute, passwordRecoveryToken.ToString());
                DateTime? generatedDateTime = DateTime.UtcNow;
                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PasswordRecoveryTokenDateGeneratedAttribute, generatedDateTime);

                //send email
                _workflowMessageService.SendCustomerPasswordRecoveryMessage(customer, _workContext.WorkingLanguage.Id);

                result.SuccessMessage = "Email with instructions has been sent to you.";
            }
            else
            {
                result.StatusCode = (int)ErrorType.NotOk;
                result.ErrorList.Add("Email not found.");
            }

            return Ok(result);
        }
        #endregion

        #region My account / Info

        [Route("api/myaccount/menu")]
        public IActionResult CustomerNavigation(int selectedTabId = 0)
        {
            var model = _customerModelFactoryApi.PrepareCustomerNavigationModel(selectedTabId);

            return Ok(model);
        }

        [Route("api/customer/info")]
        [HttpGet]
        public IActionResult Info()
        {
            var model = new CustomerInfoResponseModel();
            if (!_workContext.CurrentCustomer.IsRegistered())
            {
                model.StatusCode = (int)ErrorType.NotOk;
            }
            else
            {
                var customer = _workContext.CurrentCustomer;

                _customerModelFactoryApi.PrepareCustomerInfoModel(model, customer, false);
            }

            return Ok(model);
        }

        [Route("api/Get1StopMartUserDetails/{UserName}")]
        [HttpGet]
        public IActionResult Get1StopMartUserDetails(string UserName)
        {
            var model = new CustomerInfoResponseModel();

            string authorName = UserName.Substring(UserName.Length - 10, 10);
            authorName = "0095" + authorName;
            var customer = _customerService.GetCustomerByUsername(authorName);
            if (customer != null)
            {
                _customerModelFactoryApi.PrepareCustomerInfoModel(model, customer, false);
            }
            else
            {
                model.StatusCode = (int)ErrorType.NotOk;
            }
            return Ok(model);
        }

        [HttpPost]
        [Route("api/customer/info")]
        public IActionResult Info([FromBody]CustomerInfoQueryModel queryModel)
        {
            var model = new CustomerInfoResponseModel();
            model.ErrorList = new List<string>();
            if (!_workContext.CurrentCustomer.IsRegistered())
                throw new Exception(message: HttpStatusCode.Unauthorized.ToString());

            var customer = _workContext.CurrentCustomer;

            var form = queryModel.FormValues.ToNameValueCollection();

            //custom customer attributes
            var customerAttributesXml = ParseCustomCustomerAttributes(form);
            var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
            foreach (var error in customerAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }
            ValidationExtension.CustomerInfoValidator(ModelState, queryModel, _localizationService, _stateProvinceService, _customerSettings);
            try
            {
                if (ModelState.IsValid)
                {
                    model = _customerModelFactoryApi.PrepareCustomerInfoResponseModel(queryModel);
                    customer.ProfilePictureUrl = string.IsNullOrEmpty(queryModel.ProfilePictureUrl) ? customer.ProfilePictureUrl : queryModel.ProfilePictureUrl;
                    customer.DisplayAvatar = queryModel.DisplayAvatar;
                    customer.OtherMobileNumber = string.IsNullOrEmpty(queryModel.OtherMobileNumber) ? (customer.OtherMobileNumber ?? "").Trim() : queryModel.OtherMobileNumber;
                    var address = _addressService.GetAddressById(customer.BillingAddressId ?? 0);
                    address.PhoneNumber = queryModel.Phone;
                    address.Email = queryModel.Email;
                    //Added By Sunil Kumar At 11-03-19
                    address.CountryCode = string.IsNullOrEmpty(queryModel.CountryCode) ? address.CountryCode : queryModel.CountryCode;
                    _addressService.UpdateAddress(address);
                    model.DisplayAvatar = customer.DisplayAvatar;
                    model.ProfilePictureUrl = customer.ProfilePictureUrl;
                    model.Password = customer.SimId.Substring(customer.SimId.Length - 6);
                    model.OtherMobileNumber = customer.OtherMobileNumber;
                    //Added By Sunil Kumar At 11-03-19
                    model.CountryCode = address.CountryCode;
                    //username 
                    if (_customerSettings.UsernamesEnabled && this._customerSettings.AllowUsersToChangeUsernames)
                    {
                        if (
                            !customer.Username.Equals(model.Username.Trim(), StringComparison.InvariantCultureIgnoreCase))
                        {
                            //change username
                            _customerRegistrationService.SetUsername(customer, model.Username.Trim());
                            //re-authenticate
                            _authenticationService.SignIn(customer, true);
                        }
                    }
                    //email
                    if (!customer.Email.Equals(model.Email.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        //added by Sunil Kumar At 22-12-19
                        if (model.Email.Trim() != "")
                        {
                            //change email
                            var requireValidation = _customerSettings.UserRegistrationType ==
                                                    UserRegistrationType.EmailValidation;
                            //change email
                            _customerRegistrationService.SetEmail(customer, model.Email.Trim(), requireValidation);
                        }

                        //re-authenticate (if usernames are disabled)
                        if (!_customerSettings.UsernamesEnabled)
                        {
                            _authenticationService.SignIn(customer, true);
                        }
                    }
                    //added by Sunil Kumar At 22-01-19
                    if (!customer.IsDisplayEmail.Equals(model.IsDisplayEmail))
                    {
                        _customerRegistrationService.SetIsDisplayEmail(customer, model.IsDisplayEmail);
                    }
                    if (!model.IsDisplayEmail)
                    {
                        model.Email = string.Empty;
                    }

                    //properties
                    //if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    //{
                    //    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.TimeZoneId, model.TimeZoneId);
                    //}
                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        var prevVatNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.VatNumberAttribute);

                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.VatNumberAttribute,
                            model.VatNumber);
                        if (prevVatNumber != model.VatNumber)
                        {
                            string vatName;
                            string vatAddress;
                            var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out vatName,
                                out vatAddress);
                            _genericAttributeService.SaveAttribute(customer,
                                NopCustomerDefaults.VatNumberStatusIdAttribute,
                                (int)vatNumberStatus);
                            //send VAT number admin notification
                            if (!String.IsNullOrEmpty(model.VatNumber) &&
                                _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                                _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer,
                                    model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
                        }
                    }

                    //form fields
                    if (_customerSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.GenderAttribute,
                            model.Gender);
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FirstNameAttribute,
                        model.FirstName);
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.LastNameAttribute,
                        model.LastName);

                    if (_customerSettings.DateOfBirthEnabled)
                    {
                        DateTime? dateOfBirth = model.ParseDateOfBirth();
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.DateOfBirthAttribute,
                            dateOfBirth);
                    }
                    if (_customerSettings.CompanyEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CompanyAttribute,
                            model.Company);
                    if (_customerSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddressAttribute,
                            model.StreetAddress);
                    if (_customerSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddress2Attribute,
                            model.StreetAddress2);
                    if (_customerSettings.ZipPostalCodeEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.ZipPostalCodeAttribute,
                            model.ZipPostalCode);
                    if (_customerSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CityAttribute, model.City);
                    if (_customerSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CountryIdAttribute,
                            model.CountryId);
                    if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StateProvinceIdAttribute,
                            model.StateProvinceId);
                    if (_customerSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PhoneAttribute, model.Phone);
                    if (_customerSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FaxAttribute, model.Fax);

                    //newsletter
                    if (_customerSettings.NewsletterEnabled)
                    {
                        //save newsletter value
                        var newsletter =
                            _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email,
                                _storeContext.CurrentStore.Id);
                        if (newsletter != null)
                        {
                            if (model.Newsletter)
                            {
                                newsletter.Active = true;
                                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);
                            }
                            else
                                _newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
                        }
                        else
                        {
                            if (model.Newsletter)
                            {
                                _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = customer.Email,
                                    Active = true,
                                    StoreId = _storeContext.CurrentStore.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                            }
                        }
                    }

                    if (_forumSettings.ForumsEnabled && _forumSettings.SignaturesEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.SignatureAttribute,
                            model.Signature);

                    //save customer attributes
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                        NopCustomerDefaults.CustomCustomerAttributes, customerAttributesXml);

                    return Ok(model);
                }
                else
                {
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            model.ErrorList.Add(error.ErrorMessage);
                        }
                    }
                    model.StatusCode = (int)ErrorType.NotOk;
                    return Ok(model);
                }
            }
            catch (Exception exc)
            {
                // ModelState.AddModelError("", exc.Message);
                model.StatusCode = (int)ErrorType.NotOk;
                model.ErrorList.Add(exc.Message);
            }


            //If we got this far, something failed, redisplay form
            _customerModelFactoryApi.PrepareCustomerInfoModel(model, customer, true, customerAttributesXml);
            return Ok(model);
        }

        public IActionResult RemoveExternalAssociation(int id)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();

            //ensure it's our record
            var ear = _workContext.CurrentCustomer.ExternalAuthenticationRecords.FirstOrDefault(x => x.Id == id);



            var customer = _workContext.CurrentCustomer;

            var model = new CustomerInfoResponseModel();
            _customerModelFactoryApi.PrepareCustomerInfoModel(model, customer, false);

            if (ear == null)
                return Ok(customer);

            _externalAuthenticationService.DeleteExternalAuthenticationRecord(ear);

            return Ok(customer);
        }

        #endregion

        #region My account / Addresses
        [HttpGet]
        [Route("api/customer/addresses")]
        public IActionResult Addresses()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge(HttpStatusCode.Unauthorized.ToString());

            var model = _customerModelFactoryApi.PrepareCustomerAddressListModel();

            return Ok(model);
        }

        [Route("api/customer/address/remove/{addressId}")]
        [HttpGet]
        public IActionResult AddressDelete(int addressId)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge(HttpStatusCode.Unauthorized.ToString());

            var model = new GeneralResponseModel<bool>();

            var customer = _workContext.CurrentCustomer;

            //find address (ensure that it belongs to the current customer)
            var address = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address != null)
            {
                _customerService.RemoveCustomerAddress(customer, address);
                _customerService.UpdateCustomer(customer);
                //now delete the address record
                _addressService.DeleteAddress(address);
                model.Data = true;
            }
            else
            {
                model.StatusCode = (int)ErrorType.NotOk;
                model.Data = false;
            }
            return Ok(model);
        }

        [Route("api/customer/address/add")]
        [HttpGet]
        public IActionResult AddressAdd()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge(HttpStatusCode.Unauthorized.ToString());

            var model = new AddAdressCommonResponseModel();
            _addressModelFactoryApi.PrepareAddressModel(model.Address,
                address: null,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id));

            return Ok(model);
        }

        [Route("api/customer/address/add")]
        [HttpPost]
        public IActionResult AddressAdd([FromBody]List<KeyValueApi> formValues)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge(HttpStatusCode.Unauthorized.ToString());

            var customer = _workContext.CurrentCustomer;

            var form = formValues.ToNameValueCollection();

            var responseModel = new GeneralResponseModel<bool>();
            responseModel.ErrorList = new List<string>();

            //custom address attributes
            var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            string prefix = HelperExtension.GetEnumDescription((AddressType)3);
            Address address = form.AddressFromToModel(prefix);

            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }
            ValidationExtension.AddressValidator(ModelState, address, _localizationService, _addressSettings, _stateProvinceService);
            if (ModelState.IsValid)
            {

                address.CustomAttributes = customAttributes;
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                //commented at 11/1/19 by Sunil Kumar S
                //customer.Addresses.Add(address);
                //_customerService.UpdateCustomer(customer);

                customer.CustomerAddressMappings.Add(new CustomerAddressMapping { Address = address });//Added at 11/1/19 by Sunil Kumar S
                _customerService.UpdateCustomer(customer);

                responseModel.Data = true;
                return Ok(responseModel);
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
            //If we got this far, something failed, redisplay form


            return Ok(responseModel);
        }

        [Route("api/customer/address/edit/{addressId}")]
        [HttpGet]
        public IActionResult AddressEdit(int addressId)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge(HttpStatusCode.Unauthorized.ToString());

            var customer = _workContext.CurrentCustomer;
            var model = new AddAdressCommonResponseModel();
            //find address (ensure that it belongs to the current customer)
            var address = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
            {
                //address is not found
                model.StatusCode = (int)ErrorType.NotOk;
                model.ErrorList.Add("Not Found");

            }

            _addressModelFactoryApi.PrepareAddressModel(model.Address,
                address: address,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id));

            return Ok(model);
        }

        [HttpPost]
        [Route("api/customer/address/edit/{addressId}")]
        public IActionResult AddressEdit(int addressId, [FromBody]List<KeyValueApi> formValues)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge(HttpStatusCode.Unauthorized.ToString());

            var customer = _workContext.CurrentCustomer;

            var form = formValues.ToNameValueCollection();

            //find address (ensure that it belongs to the current customer)
            var addressFirst = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
            var responseModel = new GeneralResponseModel<bool>();

            if (addressFirst == null)
                //address is not found
                return Challenge(HttpStatusCode.Unauthorized.ToString());
            var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            string prefix = HelperExtension.GetEnumDescription((AddressType)3);
            Address address = form.AddressFromToModel(prefix);

            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }
            ValidationExtension.AddressValidator(ModelState, address, _localizationService, _addressSettings, _stateProvinceService);

            if (ModelState.IsValid)
            {
                AddressBind(address, addressFirst);
                addressFirst.CustomAttributes = customAttributes;
                _addressService.UpdateAddress(addressFirst);
                responseModel.Data = true;
                return Ok(responseModel);
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
            //If we got this far, something failed, redisplay form

            return Ok(responseModel);
            //If we got this far, something failed, redisplay form

        }

        #endregion

        #region Downloadable Product
        [HttpGet]
        [Route("api/customer/downloadableproducts")]
        public IActionResult DownloadableProducts()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge(HttpStatusCode.Unauthorized.ToString());

            var model = _customerModelFactoryApi.PrepareCustomerDownloadableProductsModel();

            return Ok(model);
        }
        #endregion

        #region Get Languages for user.
        [Route("api/GetLanguage")]
        [HttpGet]
        public IActionResult GetLanguage()
        {
            // Language Selector
            var langNavModel = _commonModelFactoryApi.PrepareLanguageSelectorModel();
            return Ok(langNavModel);
        }
        #endregion

        #region Set Langugage for user
        [Route("api/SetLanguage/{language_id}")]
        [HttpPost]
        public IActionResult SetLanguage(int language_id)
        {
            var result = new CustomerLaguageAndCurrencyResponceModel();
            var language = _languageService.GetLanguageById(language_id);
            try
            {
                if (language != null && language.Published)
                {
                    _workContext.WorkingLanguage = language;
                    result.Success = true;
                    result.StatusCode = (int)ErrorType.Ok;
                    result.SuccessMessage = "SaveSuccessy";
                }
                else
                {
                    result.Success = false;
                    result.StatusCode = (int)ErrorType.NotOk;
                    result.SuccessMessage = "Not Success";
                }
            }
            catch (Exception)
            {
                result.Success = false;
                result.SuccessMessage = "Not Success";
                result.StatusCode = (int)ErrorType.NotOk;
            }
            return Ok(result);
        }
        #endregion

        #region Language And Currency Save
        [HttpPost]
        [Route("api/SetLanguageAndCurrency")]
        public IActionResult SetLanguageAndCurrency([FromBody]CustomerLanguageAndCurrencyModel languageAndCurrencyModel)
        {
            var result = new CustomerLaguageAndCurrencyResponceModel();
            var languageId = languageAndCurrencyModel.LanguageId != 0 ? languageAndCurrencyModel.LanguageId : 0;
            var language = _languageService.GetLanguageById(languageId);
            var currencyId = languageAndCurrencyModel.CurrencyId != 0
                ? languageAndCurrencyModel.CurrencyId
                : language != null ? language.DefaultCurrencyId : 0;
            var currency = _currencyService.GetCurrencyById(currencyId);
            try
            {
                if (language != null && language.Published && currency != null && currency.Published)
                {
                    _workContext.WorkingLanguage = language;
                    _workContext.WorkingCurrency = currency;
                    result.Success = true;
                    result.StatusCode = (int)ErrorType.Ok;
                    result.SuccessMessage = "SaveSuccessy";
                }
                else if (language != null && language.Published)
                {
                    _workContext.WorkingLanguage = language;
                    result.Success = true;
                    result.StatusCode = (int)ErrorType.Ok;
                    result.SuccessMessage = "SaveSuccessy";
                }
                else if (currency != null && currency.Published)
                {
                    _workContext.WorkingCurrency = currency;
                    result.Success = true;
                    result.StatusCode = (int)ErrorType.Ok;
                    result.SuccessMessage = "SaveSuccessy";
                }
                else
                {
                    result.Success = false;
                    result.StatusCode = (int)ErrorType.NotOk;
                    result.SuccessMessage = "Not Success";
                }
            }
            catch (Exception)
            {
                result.Success = false;
                result.SuccessMessage = "Not Success";
                result.StatusCode = (int)ErrorType.NotOk; ;
            }
            return Ok(result);
        }
        #endregion

        #region Topics 
        [Route("api/topics/{SystemName}")]
        [HttpPost]
        public IActionResult GetTopicMessage(string SystemName)
        {
            var model = new TopicResponseModel();
            if (!string.IsNullOrWhiteSpace(SystemName))
            {
                var _topic = _topicService.GetTopicBySystemName(SystemName);
                if (_topic != null && _topic.Published)
                {
                    model.Data = _topic;
                    model.StatusCode = (int)ErrorType.Ok;
                    model.SuccessMessage = "Success";
                    return Ok(model);
                }
            }
            model.StatusCode = (int)ErrorType.NotOk;
            model.SuccessMessage = "Not Success";
            return Ok(model);
        }
        #endregion

        #region Verify Mobile Number
        /// <summary>
        /// This is using to Verify Mobile Number
        /// Created By : Alexandar Rajavel
        /// Created On : 24-Jan-2019
        /// </summary>
        /// <param name="model">Mobile number to verify</param>
        /// <returns>Result</returns>
        [HttpPost]
        [Route("api/customer/VerifyMobileNumber")]
        public IActionResult VerifyMobileNumber([FromBody]VerifyMobileNumberRequest model)
        {
            var result = new VerifyMobileNumberResponse();
            if (model == null || string.IsNullOrEmpty(model.DestinationNumber) || string.IsNullOrEmpty(model.Application))
            {
                result.StatusCode = (int)ErrorType.NotOk;
                result.Message = MOBILENO_AND_APPLICATION;
                return Ok(result);
            }
            else
            {
                if (model.DestinationNumber.StartsWith(STARTING_NUMBER))
                {
                    //result = _queuedNotificationApiService.SendOTP(model);
                    result = _notificationService.SendOTP(model);
                    var customerFromDevice = GetCustomerFromDeviceId();
                    var customer = _customerService.GetCustomerByUsername(model.DestinationNumber);
                    if (customerFromDevice!=null)
                    {
                        if (customer != null && customerFromDevice.Username != customer.Username)
                        {
                            customer.CustomerGuid = customerFromDevice.CustomerGuid;
                            customer.DeviceId = customerFromDevice.DeviceId;
                            _customerService.UpdateCustomer(customer);
                            //update current device subscription id to send notifications
                            var device = _deviceService.GetDeviceByDeviceToken(customerFromDevice.DeviceId);
                            if (device != null)
                            {
                                device.CustomerId = customer.Id;
                                device.IsRegistered = customer.IsRegistered();
                                _deviceService.UpdateDevice(device);
                            }
                            //Added by Alexandar Rajavel on 04-June-2019
                            //migrate shopping cart, this will happen when login with other device
                            _shoppingCartService.MigrateShoppingCartNew(customerFromDevice, customer);
                            _customerService.DeleteCustomer(customerFromDevice);
                        }
                    }
                   
                }
                else
                {
                    result.StatusCode = (int)ErrorType.NotOk;
                    result.Message = ERROR_IN_DESTINATION_NUMBER;
                }
                return Ok(result);
            }
        }
        #endregion

        #region Verify Mobile Number For huawei Devices
        /// <summary>
        /// This is using to Verify Mobile Number
        /// Created By : Alexandar Rajavel
        /// Created On : 24-Jan-2019
        /// </summary>
        /// <param name="model">Mobile number to verify</param>
        /// <returns>Result</returns>
        [HttpPost]
        [Route("api/customer/VerifyMobileNumberHuawei")]
        public IActionResult VerifyMobileNumberHuawei([FromBody]VerifyMobileNumberRequest model)
        {
            var result = new VerifyMobileNumberResponse();
            if (model == null || string.IsNullOrEmpty(model.DestinationNumber) || string.IsNullOrEmpty(model.Application))
            {
                result.StatusCode = (int)ErrorType.NotOk;
                result.Message = MOBILENO_AND_APPLICATION;
                return Ok(result);
            }
            else
            {
                if (model.DestinationNumber.StartsWith(STARTING_NUMBER))
                {
                    //result = _queuedNotificationApiService.SendOTP(model);
                    result = _notificationService.SendOTPHuwai(model);
                    var customerFromDevice = GetCustomerFromDeviceId();
                    var customer = _customerService.GetCustomerByUsername(model.DestinationNumber);
                    if (customer != null && customerFromDevice.Username != customer.Username)
                    {
                        customer.CustomerGuid = customerFromDevice.CustomerGuid;
                        customer.DeviceId = customerFromDevice.DeviceId;
                        _customerService.UpdateCustomer(customer);
                        //update current device subscription id to send notifications
                        var device = _deviceService.GetDeviceByDeviceToken(customerFromDevice.DeviceId);
                        if (device != null)
                        {
                            device.CustomerId = customer.Id;
                            device.IsRegistered = customer.IsRegistered();
                            _deviceService.UpdateDevice(device);
                        }
                        //Added by Alexandar Rajavel on 04-June-2019
                        //migrate shopping cart, this will happen when login with other device
                        _shoppingCartService.MigrateShoppingCartNew(customerFromDevice, customer);
                        _customerService.DeleteCustomer(customerFromDevice);
                    }
                }
                else
                {
                    result.StatusCode = (int)ErrorType.NotOk;
                    result.Message = ERROR_IN_DESTINATION_NUMBER;
                }
                return Ok(result);
            }
        }
        #endregion

        #region Help and Support
        /// <summary>
        /// Created by Alexandar Rajavel on 08-Feb-2019 for getting help and support details
        /// </summary>
        /// <returns>Result</returns>
        [HttpGet]
        [Route("api/customer/helpandsupport")]
        public IActionResult HelpAndSupport()
        {
            var support = _helpAndSupportService.GetAll();
            var result = new HelpandSupportResponse();
            if (support.Any())
            {
                support.ToList().ForEach(s => result.Data.Add(new ResultData { Title = s.Title, Value = s.Value }));
            }
            else
            {
                return NotFound(new { Message = NO_DATA });
            }
            return Ok(result);
        }

        /// <summary>
        /// Created by Alexandar Rajavel on 12-June-2019 for getting transaction failed customer's count
        /// </summary>
        /// <returns>Result</returns>
        [HttpGet]
        [Route("api/customer/GetTotalRecordsCount")]
        public IActionResult GetTotalRecordsCount()
        {
            var transactions = _paymentTransactionHistoryService.GetTransactionFailedCustoemrList();
            return Ok(new { TotalRecordsCount = transactions.Count() });
        }

        /// <summary>
        /// Created by Alexandar Rajavel on 01-Apr-2019 for getting transaction failed customer list
        /// </summary>
        /// <returns>Result</returns>
        [HttpGet]
        [Route("api/customer/TransactionFailedCustoemrList")]
        public IActionResult TransactionFailedCustoemrList()
        {
            var result = new List<PaymentFailedCustomers>();
            var transactions = _paymentTransactionHistoryService.GetTransactionFailedCustoemrList();
            if (transactions.Any())
            {
                transactions.ToList().ForEach(c => result.Add(new PaymentFailedCustomers
                {
                    Id = c.Id,
                    Comments = c.Comments,
                    CreatedOn = c.CreatedOnUtc.ToString("dddd, dd-MMMM-yyyy hh:mm tt"),
                    IsNew = c.IsNew,
                    MobileNumber = c.Customer.BillingAddress.PhoneNumber.StartsWith(STARTING_NUMBER) ? c.Customer.BillingAddress.PhoneNumber.Replace(STARTING_NUMBER, "0") : c.Customer.BillingAddress.PhoneNumber,
                    Name = c.Customer.BillingAddress.FirstName,
                    PaymentMethod = c.PaymentMethod.Replace("Payments.", ""),
                    Status = c.IssueStatus ?? 0,
                    TransactionAmount = c.TransactionAmount,
                    TransactionErrorMessage = c.TransactionDescription,
                    UpdatedBy = c.UpdatedBy,
                    UpdatedOn = c.UpdatedOnUtc.ToString("dddd, dd-MMMM-yyyy hh:mm tt")
                }));
                return Ok(new { Data = result });
            }
            else
            {
                return NotFound(new { Message = NO_DATA });
            }
        }

        /// <summary>
        /// Created by Alexandar Rajavel on 01-Apr-2019 for updating ticket staus
        /// </summary>
        [HttpPost]
        [Route("api/customer/UpdateTicketStatus")]
        public IActionResult UpdateTicketStatus([FromBody]UpdateTicketStatusRequest model)
        {
            var transaction = _paymentTransactionHistoryService.GetTransactionHistoryById(model.Id);
            if (transaction != null)
            {
                transaction.IsNew = false;
                transaction.IssueStatus = model.Status;
                transaction.Comments = model.Comments;
                transaction.UpdatedBy = model.UpdatedBy;
                transaction.UpdatedOnUtc = DateTime.Now;
                _paymentTransactionHistoryService.UpdateTransactionHistory(transaction);
                return Ok(new { Message = SUCCESS });
            }
            else
            {
                return NotFound(new { Message = NO_DATA });
            }
        }

        #endregion
    }
}
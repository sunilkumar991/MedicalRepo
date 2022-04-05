using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Utility;
using System;
using System.IO;
using System.Web;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.ExportImport;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Tax;
using Nop.Web.Factories;
using Nop.Web.Models.Customer;
using Nop.Services.Security;

namespace Nop.Web.Controllers
{
    public partial class HomeController : BasePublicController
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly IDownloadService _downloadService;
        private readonly ForumSettings _forumSettings;
        private readonly GdprSettings _gdprSettings;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IAddressService _addressService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly ICustomerModelFactory _customerModelFactory;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IExportManager _exportManager;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly IGdprService _gdprService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGiftCardService _giftCardService;
        private readonly ILocalizationService _localizationService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly ICityService _cityService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IEncryptionService _encryptionService;


        private readonly LocalizationSettings _localizationSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public HomeController(AddressSettings addressSettings,
            CaptchaSettings captchaSettings,
            CustomerSettings customerSettings,
            DateTimeSettings dateTimeSettings,
            IDownloadService downloadService,
            ForumSettings forumSettings,
            GdprSettings gdprSettings,
            IAddressAttributeParser addressAttributeParser,
            IAddressModelFactory addressModelFactory,
            IAddressService addressService,
            IAuthenticationService authenticationService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerActivityService customerActivityService,
            ICustomerAttributeParser customerAttributeParser,
            ICustomerAttributeService customerAttributeService,
            ICustomerModelFactory customerModelFactory,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            IExportManager exportManager,
            IExternalAuthenticationService externalAuthenticationService,
            IGdprService gdprService,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            ILocalizationService localizationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IOrderService orderService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            ITaxService taxService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            ICityService cityService,
            IStateProvinceService stateProvinceService,
            IEncryptionService encryptionService,
            LocalizationSettings localizationSettings,
            MediaSettings mediaSettings,
            StoreInformationSettings storeInformationSettings,
            TaxSettings taxSettings)
        {
            this._addressSettings = addressSettings;
            this._captchaSettings = captchaSettings;
            this._customerSettings = customerSettings;
            this._dateTimeSettings = dateTimeSettings;
            this._downloadService = downloadService;
            this._forumSettings = forumSettings;
            this._gdprSettings = gdprSettings;
            this._addressAttributeParser = addressAttributeParser;
            this._addressModelFactory = addressModelFactory;
            this._addressService = addressService;
            this._authenticationService = authenticationService;
            this._countryService = countryService;
            this._currencyService = currencyService;
            this._customerActivityService = customerActivityService;
            this._customerAttributeParser = customerAttributeParser;
            this._customerAttributeService = customerAttributeService;
            this._customerModelFactory = customerModelFactory;
            this._customerRegistrationService = customerRegistrationService;
            this._customerService = customerService;
            this._eventPublisher = eventPublisher;
            this._exportManager = exportManager;
            this._externalAuthenticationService = externalAuthenticationService;
            this._gdprService = gdprService;
            this._genericAttributeService = genericAttributeService;
            this._giftCardService = giftCardService;
            this._localizationService = localizationService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._orderService = orderService;
            this._pictureService = pictureService;
            this._priceFormatter = priceFormatter;
            this._shoppingCartService = shoppingCartService;
            this._storeContext = storeContext;
            this._taxService = taxService;
            this._webHelper = webHelper;
            this._workContext = workContext;
            this._workflowMessageService = workflowMessageService;
            this._localizationSettings = localizationSettings;
            this._mediaSettings = mediaSettings;
            this._storeInformationSettings = storeInformationSettings;
            this._taxSettings = taxSettings;
            this._cityService = cityService;
            this._stateProvinceService = stateProvinceService;
            this._encryptionService = encryptionService;
        }

        #endregion

        #region Utilities

        protected virtual string ParseCustomCustomerAttributes(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = "";
            var attributes = _customerAttributeService.GetAllCustomerAttributes();
            foreach (var attribute in attributes)
            {
                var controlId = $"customer_attribute_{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var selectedAttributeId = int.Parse(item);
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
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var enteredText = ctrlAttributes.ToString().Trim();
                                attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.FileUpload:
                    //not supported customer attributes
                    default:
                        break;
                }
            }

            return attributesXml;
        }

        #endregion

      
        [HttpsRequirement(SslRequirement.No)]
        public virtual IActionResult Index()
        {
            string networkDetail = string.Empty;
            Stream stream = Request.Body;
            StreamReader reader = new StreamReader(stream);
            string inputString = reader.ReadToEnd();

            if (!string.IsNullOrEmpty(inputString))
            {
                if (!string.IsNullOrEmpty(inputString) && inputString.StartsWith("UtilityJsonRequest"))
                {
                    inputString = inputString.Replace("UtilityJsonRequest=", "");
                }

                if (!string.IsNullOrEmpty(inputString))
                {
                    for (var i = 0; i < 10; i++)
                    {
                        inputString = HttpUtility.UrlDecode(inputString);
                    }

                    string data = inputString;
                    string[] dataArr = data.Split(new string[] { "DeviceInfo" }, StringSplitOptions.None);
                    if (dataArr != null && dataArr[1] != null && dataArr[1].ToString().Length > 5)
                    {
                        networkDetail = dataArr[1].ToString();
                        networkDetail = networkDetail.Substring(0, networkDetail.Length - 4).Remove(0, 2);
                    }

                    //Session["UtilityPaymentJsonData"] = inputString;
                    var isExist = inputString.Contains("Language");
                    var isAppVersion = inputString.Contains("AppVersion");
                    var jsonData = JsonConvert.DeserializeObject<UtilityJsonRequest>(inputString);
                    var appVersion = jsonData.Utility.AppVersion;
                    var isNative = jsonData.Utility.IsNative;
                    var isIphone = jsonData.Utility.IsIphone;


                    //Create user registration model from JSON
                    RegisterModel model = new RegisterModel();
                    model.Newsletter = true;
                    model.FirstName = jsonData.Utility.FirstName;
                    model.LastName = jsonData.Utility.LastName;
                    model.Gender = jsonData.Utility.Gender;
                    model.Email = jsonData.Utility.Email;
                    model.Password = jsonData.Utility.SimId.Substring((jsonData.Utility.SimId.Length - 6));
                    model.Phone = jsonData.Utility.MobileNumber;
                    model.Username = jsonData.Utility.Username;
                    model.DateOfBirthDay = Convert.ToInt16(jsonData.Utility.DateofBirthDay);
                    model.DateOfBirthMonth = Convert.ToInt16(jsonData.Utility.DateofBirthMonth);
                    model.DateOfBirthYear = Convert.ToInt16(jsonData.Utility.DateofBirthYear);
                    model.StreetAddress = jsonData.Utility.Address1;
                    model.StreetAddress2 = jsonData.Utility.Address2;
                    model.OtherEmail = jsonData.Utility.OtherEmail;
                    model.DeviceId = jsonData.Utility.DeviceId;
                    model.SimID = jsonData.Utility.SimId;
                    //string DeviceDetail = jsonData.Utility.Longitude;

                    //City,state & Country Calculation
                    int countryId = 0;
                    int stateId = 0;
                    int cityId = 0;

                    if(!string.IsNullOrEmpty(jsonData.Utility.Country))
                        countryId = _countryService.GetCountryByName(jsonData.Utility.Country) == null ? 0 : _countryService.GetCountryByName(jsonData.Utility.Country).Id;
                    if (countryId > 0 && !string.IsNullOrEmpty(jsonData.Utility.State))
                        stateId = _stateProvinceService.GetStateByCountryIdAndStateName(countryId, jsonData.Utility.State) == null? 0 :_stateProvinceService.GetStateByCountryIdAndStateName(countryId, jsonData.Utility.State).Id;
                    if (stateId > 0 && !string.IsNullOrEmpty(jsonData.Utility.City))
                        cityId = _cityService.GetCityByStateIdAndCityName(stateId, jsonData.Utility.City) == null ? 0 :_cityService.GetCityByStateIdAndCityName(stateId, jsonData.Utility.City).Id;

                    model.CityId = cityId;
                    model.StateProvinceId = stateId;
                    model.CountryId = countryId;
                    // user registration model from JSON creation end here

                    //check whether registration is allowed
                    if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                        return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

                    if (_workContext.CurrentCustomer.IsRegistered())
                    {
                        //Already registered customer. 
                        _authenticationService.SignOut();

                        //raise logged out event       
                        _eventPublisher.Publish(new CustomerLoggedOutEvent(_workContext.CurrentCustomer));

                        ////Save a new record
                        //_workContext.CurrentCustomer = _customerService.InsertGuestCustomer();
                    }
                    var customer = _workContext.CurrentCustomer;
                    customer.RegisteredInStoreId = _storeContext.CurrentStore.Id;

                    //if user already registered than login user
                    Customer existingCustomer = _customerService.GetCustomerByUsername(model.Username);
                    if (existingCustomer != null)
                    {
                        if (ModelState.IsValid)
                        {
                            if (_customerSettings.UsernamesEnabled && model.Username != null)
                            {
                                model.Username = model.Username.Trim();
                                //Customer registeredCutomer = _customerService.GetCustomerByUsername(model.Username);
                                //if(registeredCutomer != null)
                                //    model.Password = _customerService.GetCurrentPasswordNotHashed(registeredCutomer.Id).Password;
                                
                            }

                            //Update password if SimId Changed
                            if(existingCustomer.SimId.Trim() != model.SimID)
                            {
                                var customerPassword = new CustomerPassword
                                {
                                    Customer = existingCustomer,
                                    PasswordFormat = PasswordFormat.Hashed,
                                    CreatedOnUtc = DateTime.UtcNow
                                };
                                    
                                var saltKey = _encryptionService.CreateSaltKey(NopCustomerServiceDefaults.PasswordSaltKeySize);
                                customerPassword.PasswordSalt = saltKey;
                                customerPassword.Password = _encryptionService.CreatePasswordHash(model.Password, saltKey, _customerSettings.HashedPasswordFormat);
                                _customerService.UpdateCustomerPassword(customerPassword);

                                //Update SimId
                                existingCustomer.SimId = model.SimID;
                                existingCustomer.LastActivityDateUtc = DateTime.UtcNow;
                                _customerService.UpdateCustomer(existingCustomer);
                            }

                            
                            //-------------------End Here-----------------------

                            var loginResult = _customerRegistrationService.ValidateCustomer(_customerSettings.UsernamesEnabled ? model.Username : model.Email, model.Password);
                            switch (loginResult)
                            {
                                case CustomerLoginResults.Successful:
                                    {
                                        var customerLogin = _customerSettings.UsernamesEnabled
                                            ? _customerService.GetCustomerByUsername(model.Username)
                                            : _customerService.GetCustomerByEmail(model.Email);

                                        //migrate shopping cart
                                        _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customerLogin, true);

                                        //sign in new customer
                                        _authenticationService.SignIn(customerLogin, true);

                                        //raise event       
                                        _eventPublisher.Publish(new CustomerLoggedinEvent(customerLogin));

                                        //activity log
                                        _customerActivityService.InsertActivity(customer, "PublicStore.Login",
                                            _localizationService.GetResource("ActivityLog.PublicStore.Login"), customer);

                                        //Enter customer network remark added by Ankur Shrivastava 21/9/2018
                                        if (customer.Id > 0)
                                        {
                                            CustomerRemark remark = new CustomerRemark();
                                            remark.CustomerId = customer.Id;
                                            remark.NetworkRemark = networkDetail;
                                            remark.Published = false;
                                            remark.CreatedOnUtc = DateTime.UtcNow;
                                            _customerService.InsertCustomerRemark(remark);
                                        }

                                        //if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                                        //    return RedirectToRoute("HomePage");

                                        return RedirectToRoute("HomePage");
                                    }
                                case CustomerLoginResults.CustomerNotExist:
                                    ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist"));
                                    break;
                                case CustomerLoginResults.Deleted:
                                    ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.Deleted"));
                                    break;
                                case CustomerLoginResults.NotActive:
                                    ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotActive"));
                                    break;
                                case CustomerLoginResults.NotRegistered:
                                    ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered"));
                                    break;
                                case CustomerLoginResults.LockedOut:
                                    ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.LockedOut"));
                                    break;
                                case CustomerLoginResults.WrongPassword:
                                default:
                                    ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials"));
                                    break;
                            }
                        }
                    }
                    //End Login Process

                    //custom customer attributes
                    //var customerAttributesXml = ParseCustomCustomerAttributes(model.Form);
                    //var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
                    //foreach (var error in customerAttributeWarnings)
                    //{
                    //    ModelState.AddModelError("", error);
                    //}

                    //validate CAPTCHA
                    //if (_captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage && !captchaValid)
                    //{
                    //    ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
                    //}

                    if (ModelState.IsValid)
                    {
                        if (_customerSettings.UsernamesEnabled && model.Username != null)
                        {
                            model.Username = model.Username.Trim();
                        }

                        var isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;


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
                            Customer newCustomer = _customerService.GetCustomerById(customer.Id);
                            newCustomer.OtherEmailAddresses = model.OtherEmail;
                            newCustomer.DeviceId = model.DeviceId;
                            newCustomer.SimId = model.SimID;
                            _customerService.UpdateCustomer(newCustomer);

                            //Enter customer network remark added by Ankur Shrivastava 21/9/2018
                            if (customer.Id > 0)
                            {
                                CustomerRemark remark = new CustomerRemark();
                                remark.CustomerId = customer.Id;
                                remark.NetworkRemark = networkDetail;
                                remark.Published = false;
                                remark.CreatedOnUtc = DateTime.UtcNow;
                                _customerService.InsertCustomerRemark(remark);
                            }

                            //properties
                            if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                            {
                                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.TimeZoneIdAttribute, model.TimeZoneId);
                            }
                            //VAT number
                            if (_taxSettings.EuVatEnabled)
                            {
                                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.VatNumberAttribute, model.VatNumber);

                                var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out string _, out string vatAddress);
                                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.VatNumberStatusIdAttribute, (int)vatNumberStatus);
                                //send VAT number admin notification
                                if (!string.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                                    _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer, model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
                            }

                            //form fields
                            if (_customerSettings.GenderEnabled)
                                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.GenderAttribute, model.Gender);
                            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FirstNameAttribute, model.FirstName);
                            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.LastNameAttribute, model.LastName);
                            if (!string.IsNullOrEmpty(model.Phone))
                                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PhoneAttribute, model.Phone);
                            if (_customerSettings.DateOfBirthEnabled)
                            {
                                var dateOfBirth = model.ParseDateOfBirth();
                                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.DateOfBirthAttribute, dateOfBirth);
                            }
                            if (_customerSettings.CompanyEnabled)
                                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CompanyAttribute, model.Company);

                            //Save Address Field Do not need to check as fields will be given in JSON By OK$
                            if (cityId > 0)
                            {
                                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddressAttribute, model.StreetAddress);
                                //if (_customerSettings.StreetAddress2Enabled)
                                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddress2Attribute, model.StreetAddress2);
                                //if (_customerSettings.ZipPostalCodeEnabled)
                                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.ZipPostalCodeAttribute, model.ZipPostalCode);
                                //if (_customerSettings.CityEnabled)
                                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CityAttribute, model.City);
                                //if (_customerSettings.CityEnabled)
                                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CityIdAttribute, model.CityId);
                                //if (_customerSettings.CountyEnabled)
                                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CountyAttribute, model.County);
                                //if (_customerSettings.CountryEnabled)
                                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CountryIdAttribute, model.CountryId);
                                //if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StateProvinceIdAttribute, model.StateProvinceId);
                                //if (_customerSettings.PhoneEnabled)
                                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PhoneAttribute, model.Phone);
                            }
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

                                        //GDPR
                                        if (_gdprSettings.GdprEnabled && _gdprSettings.LogNewsletterConsent)
                                        {
                                            _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.Newsletter"));
                                        }
                                    }
                                    else
                                    {
                                        //When registering, not checking the newsletter check box should not take an existing email address off of the subscription list.
                                        //_newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
                                    }
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

                                        //GDPR
                                        if (_gdprSettings.GdprEnabled && _gdprSettings.LogNewsletterConsent)
                                        {
                                            _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.Newsletter"));
                                        }
                                    }
                                }
                            }

                            if (_customerSettings.AcceptPrivacyPolicyEnabled)
                            {
                                //privacy policy is required
                                //GDPR
                                if (_gdprSettings.GdprEnabled && _gdprSettings.LogPrivacyPolicyConsent)
                                {
                                    _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.PrivacyPolicy"));
                                }
                            }

                            //GDPR
                            if (_gdprSettings.GdprEnabled)
                            {
                                var consents = _gdprService.GetAllConsents().Where(consent => consent.DisplayDuringRegistration).ToList();
                                foreach (var consent in consents)
                                {
                                    var controlId = $"consent{consent.Id}";
                                    var cbConsent = model.Form[controlId];
                                    if (!StringValues.IsNullOrEmpty(cbConsent) && cbConsent.ToString().Equals("on"))
                                    {
                                        //agree
                                        _gdprService.InsertLog(customer, consent.Id, GdprRequestType.ConsentAgree, consent.Message);
                                    }
                                    else
                                    {
                                        //disagree
                                        _gdprService.InsertLog(customer, consent.Id, GdprRequestType.ConsentDisagree, consent.Message);
                                    }
                                }
                            }

                            //save customer attributes
                            //_genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CustomCustomerAttributes, customerAttributesXml);

                            //login customer now
                            if (isApproved)
                                _authenticationService.SignIn(customer, true);

                            //insert default address (if possible)
                            var defaultAddress = new Address
                            {
                                FirstName = model.FirstName, //_genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute),
                                LastName = model.LastName, //_genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute),
                                Email = model.Email,  //customer.Email,
                                Company = model.Company, // _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CompanyAttribute),
                                CountryId = model.CountryId, //_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.CountryIdAttribute) > 0
                                                             //? (int?)_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.CountryIdAttribute)
                                                             //: null,
                                StateProvinceId = model.StateProvinceId,  //_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.StateProvinceIdAttribute) > 0
                                                                          //? (int?)_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.StateProvinceIdAttribute)
                                                                          //: null,
                                CityId = model.CityId,
                                //_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.CityIdAttribute) > 0
                                //? (int?)_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.CityIdAttribute) : null,
                                County = "", // _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CountyAttribute),
                                //City = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CityAttribute),

                                Address1 = model.StreetAddress, // _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddressAttribute),
                                Address2 = model.StreetAddress2, //_genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddress2Attribute),
                                ZipPostalCode = "", // _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.ZipPostalCodeAttribute),
                                PhoneNumber = model.Phone, //_genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PhoneAttribute),
                                FaxNumber = "", // _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FaxAttribute),
                                CreatedOnUtc = customer.CreatedOnUtc
                            };
                            if ( cityId > 0) //this._addressService.IsAddressValid(defaultAddress))
                            {
                                //some validation
                                if (defaultAddress.CountryId == 0)
                                    defaultAddress.CountryId = null;
                                if (defaultAddress.StateProvinceId == 0)
                                    defaultAddress.StateProvinceId = null;
                                //set default address
                                //customer.Addresses.Add(defaultAddress);
                                customer.CustomerAddressMappings.Add(new CustomerAddressMapping { Address = defaultAddress });
                                customer.BillingAddress = defaultAddress;
                                customer.ShippingAddress = defaultAddress;
                                _customerService.UpdateCustomer(customer);
                            }

                            //notifications
                            if (_customerSettings.NotifyNewCustomerRegistration)
                                _workflowMessageService.SendCustomerRegisteredNotificationMessage(customer,
                                    _localizationSettings.DefaultAdminLanguageId);

                            //raise event       
                            _eventPublisher.Publish(new CustomerRegisteredEvent(customer));

                            switch (_customerSettings.UserRegistrationType)
                            {
                                case UserRegistrationType.EmailValidation:
                                    {
                                        //email validation message
                                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
                                        _workflowMessageService.SendCustomerEmailValidationMessage(customer, _workContext.WorkingLanguage.Id);

                                        //result
                                        return RedirectToRoute("RegisterResult",
                                            new { resultId = (int)UserRegistrationType.EmailValidation });
                                    }
                                case UserRegistrationType.AdminApproval:
                                    {
                                        return RedirectToRoute("RegisterResult",
                                            new { resultId = (int)UserRegistrationType.AdminApproval });
                                    }
                                case UserRegistrationType.Standard:
                                    {
                                        //send customer welcome message
                                        _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);

                                        var redirectUrl = Url.RouteUrl("RegisterResult", new { resultId = (int)UserRegistrationType.Standard }, _webHelper.CurrentRequestProtocol);
                                        //if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                                        //    redirectUrl = _webHelper.ModifyQueryString(redirectUrl, "returnurl", returnUrl);
                                        return Redirect(redirectUrl);
                                    }
                                default:
                                    {
                                        return RedirectToRoute("HomePage");
                                    }
                            }
                        }

                        //errors
                        foreach (var error in registrationResult.Errors)
                            ModelState.AddModelError("", error);
                    }
                }
            }
            return View();
            
        }

        public IActionResult OKTest(string dataforok, string reststring)
        {
            ViewBag.NEwDataok = dataforok;

            ViewBag.RestSTring = reststring;
            return View();
        }



    }
}
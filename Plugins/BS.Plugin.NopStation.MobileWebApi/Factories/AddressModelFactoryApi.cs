using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;
using BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel;

namespace BS.Plugin.NopStation.MobileWebApi.Factories
{
    /// <summary>
    /// Represents the address model factory
    /// </summary>
    public partial class AddressModelFactoryApi : IAddressModelFactoryApi
    {
        #region Fields

        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly ILocalizationService _localizationService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IGenericAttributeService _genericAttributeService;

        private const string COMMA = ", ";
        private const string HOUSE_NO = "House No:";
        private const string FLOOR_NO = "Floor No:";
        private const string ROOM_NO = "Room No:";
        private const string ROPE_COLOR = "Rope Color:";
        private const string IS_LIFT_OPTION = "Is Lift Option: Yes";
        #endregion

        #region Constructors

        public AddressModelFactoryApi(IAddressAttributeService addressAttributeService,
            IAddressAttributeParser addressAttributeParser,
            ILocalizationService localizationService,
            IStateProvinceService stateProvinceService,
            IAddressAttributeFormatter addressAttributeFormatter,
            IGenericAttributeService genericAttributeService)
        {
            this._addressAttributeService = addressAttributeService;
            this._addressAttributeParser = addressAttributeParser;
            this._localizationService = localizationService;
            this._stateProvinceService = stateProvinceService;
            this._addressAttributeFormatter = addressAttributeFormatter;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare address attributes
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address entity</param>
        /// <param name="overrideAttributesXml">Overridden address attributes in XML format; pass null to use CustomAttributes of address entity</param>
        protected virtual void PrepareCustomAddressAttributes(AddressModel model,
            Address address, string overrideAttributesXml = "")
        {
            var attributes = _addressAttributeService.GetAllAddressAttributes();
            foreach (var attribute in attributes)
            {
                var attributeModel = new AddressAttributeModel
                {
                    Id = attribute.Id,
                    Name = _localizationService.GetLocalized(attribute, x => x.Name),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _addressAttributeService.GetAddressAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new AddressAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = _localizationService.GetLocalized(attributeValue, x => x.Name),
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }

                //set already selected attributes
                var selectedAddressAttributes = !String.IsNullOrEmpty(overrideAttributesXml) ?
                    overrideAttributesXml :
                    (address != null ? address.CustomAttributes : null);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                        {
                            if (!String.IsNullOrEmpty(selectedAddressAttributes))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = _addressAttributeParser.ParseAddressAttributeValues(selectedAddressAttributes);
                                foreach (var attributeValue in selectedValues)
                                    foreach (var item in attributeModel.Values)
                                        if (attributeValue.Id == item.Id)
                                            item.IsPreSelected = true;
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //do nothing
                            //values are already pre-set
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            if (!String.IsNullOrEmpty(selectedAddressAttributes))
                            {
                                var enteredText = _addressAttributeParser.ParseValues(selectedAddressAttributes, attribute.Id);
                                if (enteredText.Any())
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    default:
                        //not supported attribute control types
                        break;
                }

                model.CustomAddressAttributes.Add(attributeModel);
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Prepare address model
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address entity</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="addressSettings">Address settings</param>
        /// <param name="loadCountries">Countries loading function; pass null if countries do not need to load</param>
        /// <param name="prePopulateWithCustomerFields">Whether to populate model properties with the customer fields (used with the customer entity)</param>
        /// <param name="customer">Customer entity; required if prePopulateWithCustomerFields is true</param>
        /// <param name="overrideAttributesXml">Overridden address attributes in XML format; pass null to use CustomAttributes of the address entity</param>
        public virtual void PrepareAddressModel(AddressModel model,
            Address address, bool excludeProperties,
            AddressSettings addressSettings,
            Func<IList<Country>> loadCountries = null,
            bool prePopulateWithCustomerFields = false,
            Customer customer = null,
            string overrideAttributesXml = "")
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (addressSettings == null)
                throw new ArgumentNullException("addressSettings");

            if (!excludeProperties && address != null)
            {
                model.Id = address.Id;
                model.FirstName = address.FirstName;
                model.LastName = address.LastName;
                model.Email = address.Email;
                model.Company = address.Company;
                model.CountryId = address.CountryId;
                model.CountryName = address.Country != null
                    ? _localizationService.GetLocalized(address.Country, x => x.Name)
                    : null;
                model.StateProvinceId = address.StateProvinceId;
                model.StateProvinceName = address.StateProvince != null
                    ? _localizationService.GetLocalized(address.StateProvince, x => x.Name)  //Fixed By Ankur on 18 Oct 2018
                    : null;
                model.City = address.City != null
                    ? _localizationService.GetLocalized(address.City, x => x.Name)  //Fixed By Ankur on 18 Oct 2018
                    : null;
                model.CityId = address.CityId;
                model.Address1 = address.Address1;
                model.Address2 = address.Address2;
                model.ZipPostalCode = address.ZipPostalCode;
                model.PhoneNumber = address.PhoneNumber;
                model.FaxNumber = address.FaxNumber;
                model.IsDeliveryAllowed = address.City != null ? address.City.IsDeliveryAllowed : false; // Added By Ankur on 27th October 2018
                // Added By Sunil Kumar on 7th December 2018
                model.HouseNo = address.HouseNo;
                model.FloorNo = address.FloorNo;
                model.RoomNo = address.RoomNo;
                model.IsLiftOption = address.IsLiftOption;
                model.RopeColor = address.RopeColor;
            }

            if (address == null && prePopulateWithCustomerFields)
            {
                if (customer == null)
                    throw new Exception("Customer cannot be null when prepopulating an address");
                model.Email = customer.Email;
                model.FirstName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute);
                model.LastName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute);
                model.Company = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CompanyAttribute);
                model.Address1 = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddressAttribute);
                model.Address2 = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddress2Attribute);
                model.ZipPostalCode = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.ZipPostalCodeAttribute);
                model.City = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CityAttribute);
                ////ignore country and state for prepopulation. it can cause some issues when posting pack with errors, etc
                //model.CountryId = customer.GetAttribute<int>(NopCustomerDefaults.CountryId);
                //model.StateProvinceId = customer.GetAttribute<int>(NopCustomerDefaults.StateProvinceId);
                model.PhoneNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PhoneAttribute);
                model.FaxNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FaxAttribute);
                // Added By Sunil Kumar on 7th December 2018
                model.HouseNo = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.HouseNoAttribute);
                model.FloorNo = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FloorNoAttribute);
                model.RoomNo = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.RoomNoAttribute);
                model.RopeColor = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.RopeColorAttribute);
            }

            //countries and states
            if (addressSettings.CountryEnabled && loadCountries != null)
            {
                model.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                foreach (var c in loadCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = _localizationService.GetLocalized(c, x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (addressSettings.StateProvinceEnabled)
                {
                    var languageId = EngineContext.Current.Resolve<IWorkContext>().WorkingLanguage.Id;
                    var states = _stateProvinceService
                        .GetStateProvincesByCountryId(model.CountryId.HasValue ? model.CountryId.Value : 0, languageId)
                        .ToList();
                    if (states.Any())
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem
                            {
                                Text = _localizationService.GetLocalized(s, x => x.Name),
                                Value = s.Id.ToString(),
                                Selected = (s.Id == model.StateProvinceId)
                            });
                        }
                    }
                    else
                    {
                        bool anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);
                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = _localizationService.GetResource(anyCountrySelected ? "Address.OtherNonUS" : "Address.SelectState"),
                            Value = "0"
                        });
                    }
                }
            }

            //form fields
            model.CompanyEnabled = addressSettings.CompanyEnabled;
            model.CompanyRequired = addressSettings.CompanyRequired;
            model.StreetAddressEnabled = addressSettings.StreetAddressEnabled;
            model.StreetAddressRequired = addressSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = addressSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = addressSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = addressSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = addressSettings.ZipPostalCodeRequired;
            model.CityEnabled = addressSettings.CityEnabled;
            model.CityRequired = addressSettings.CityRequired;
            model.CountryEnabled = addressSettings.CountryEnabled;
            model.StateProvinceEnabled = addressSettings.StateProvinceEnabled;
            model.PhoneEnabled = addressSettings.PhoneEnabled;
            model.PhoneRequired = addressSettings.PhoneRequired;
            model.FaxEnabled = addressSettings.FaxEnabled;
            model.FaxRequired = addressSettings.FaxRequired;

            //customer attribute services
            if (_addressAttributeService != null && _addressAttributeParser != null)
            {
                PrepareCustomAddressAttributes(model, address, overrideAttributesXml);
            }
            if (_addressAttributeFormatter != null && address != null)
            {
                model.FormattedCustomAddressAttributes = _addressAttributeFormatter.FormatAttributes(address.CustomAttributes);
            }
        }


        public string AddressConcatenate(Address address)
        {
            var houseNumber = string.IsNullOrEmpty(address.HouseNo) ? string.Empty : HOUSE_NO + address.HouseNo;
            var floorNumber = string.IsNullOrEmpty(address.FloorNo) ? string.Empty : FLOOR_NO + address.FloorNo;
            var roomNumber = string.IsNullOrEmpty(address.RoomNo) ? string.Empty : ROOM_NO + address.RoomNo;
            var ropeColor = string.IsNullOrEmpty(address.RopeColor) ? string.Empty : ROPE_COLOR + address.RopeColor;
            var isLiftOption = address.IsLiftOption ? IS_LIFT_OPTION : string.Empty;
            var address1 = string.IsNullOrEmpty(address.Address1) ? string.Empty : address.Address1;
            var address2 = string.IsNullOrEmpty(address.Address2) ? string.Empty : address.Address2;
            var city = address.City == null ? string.Empty : address.City.Name;
            var state = address.StateProvince == null ? string.Empty : address.StateProvince.Name;
            var country = address.Country == null ? string.Empty : address.Country.Name;
            var addressArray = new[] { houseNumber, floorNumber, roomNumber, isLiftOption, ropeColor, address1, address2, city, state, country };
            var addressString = string.Join(COMMA, addressArray.Where(s => !string.IsNullOrEmpty(s)));
            return addressString;
        }

        public string ShippingAddressConcatenate(Address address)
        {
            var houseNumber = string.IsNullOrEmpty(address.HouseNo) ? string.Empty : HOUSE_NO + address.HouseNo;
            var floorNumber = string.IsNullOrEmpty(address.FloorNo) ? string.Empty : FLOOR_NO + address.FloorNo;
            var roomNumber = string.IsNullOrEmpty(address.RoomNo) ? string.Empty : ROOM_NO + address.RoomNo;
            var ropeColor = string.IsNullOrEmpty(address.RopeColor) ? string.Empty : ROPE_COLOR + address.RopeColor;
            var address1 = string.IsNullOrEmpty(address.Address1) ? string.Empty : address.Address1;
            var address2 = string.IsNullOrEmpty(address.Address2) ? string.Empty : address.Address2;
            var city = address.City == null ? string.Empty : address.City.Name;
            var state = address.StateProvince == null ? string.Empty : address.StateProvince.Name;
            var country = address.Country == null ? string.Empty : address.Country.Name;
            var addressArray = new[] { houseNumber, floorNumber, roomNumber,  ropeColor, address1, address2, city, state, country };
            var addressString = string.Join(COMMA, addressArray.Where(s => !string.IsNullOrEmpty(s)));
            return addressString;
        }



        #endregion
    }
}

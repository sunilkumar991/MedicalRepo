using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Services.Directory;
using Nop.Services.Events;

namespace Nop.Services.Common
{
    /// <summary>
    /// Address service
    /// </summary>
    public partial class AddressService : IAddressService
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly ICacheManager _cacheManager;
        private readonly ICountryService _countryService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<Address> _addressRepository;
        private readonly IStateProvinceService _stateProvinceService;

        #endregion

        #region Ctor

        public AddressService(AddressSettings addressSettings,
            IAddressAttributeService addressAttributeService,
            ICacheManager cacheManager,
            ICountryService countryService,
            IEventPublisher eventPublisher,
            IRepository<Address> addressRepository,
            IStateProvinceService stateProvinceService)
        {
            this._addressSettings = addressSettings;
            this._addressAttributeService = addressAttributeService;
            this._cacheManager = cacheManager;
            this._countryService = countryService;
            this._eventPublisher = eventPublisher;
            this._addressRepository = addressRepository;
            this._stateProvinceService = stateProvinceService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes an address
        /// </summary>
        /// <param name="address">Address</param>
        public virtual void DeleteAddress(Address address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            _addressRepository.Delete(address);

            //cache
            _cacheManager.RemoveByPattern(NopCommonDefaults.AddressesPatternCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(address);
        }

        /// <summary>
        /// Gets total number of addresses by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <returns>Number of addresses</returns>
        public virtual int GetAddressTotalByCountryId(int countryId)
        {
            if (countryId == 0)
                return 0;

            var query = from a in _addressRepository.Table
                        where a.CountryId == countryId
                        select a;
            return query.Count();
        }

        /// <summary>
        /// Get addresses by mobile number
        /// Created By : Alexandar Rajavel
        /// Created On : 21-Sep-2018
        /// </summary>
        /// <param name="mobileNumber">Country identifier</param>
        /// <returns>Number of addresses</returns>
        public virtual Address GetAddressByMobileNumber(string mobileNumber)
        {
            if (string.IsNullOrWhiteSpace(mobileNumber))
                return null;

            var query = from a in _addressRepository.Table
                        orderby a.Id
                        where a.PhoneNumber == mobileNumber
                        select a;

            var address = query.FirstOrDefault();
            return address;
        }

        /// <summary>
        /// Gets total number of addresses by state/province identifier
        /// </summary>
        /// <param name="stateProvinceId">State/province identifier</param>
        /// <returns>Number of addresses</returns>
        public virtual int GetAddressTotalByStateProvinceId(int stateProvinceId)
        {
            if (stateProvinceId == 0)
                return 0;

            var query = from a in _addressRepository.Table
                        where a.StateProvinceId == stateProvinceId
                        select a;
            return query.Count();
        }

        /// <summary>
        /// Gets total number of addresses by city identifier
        /// </summary>
        /// <param name="cityId">cityId identifier</param>
        /// <returns>Number of addresses</returns>
        public virtual int GetAddressTotalByCityId(int cityId)
        {
            if (cityId == 0)
                return 0;

            var query = from a in _addressRepository.Table
                        where a.CityId == cityId
                        select a;
            return query.Count();
        }

        /// <summary>
        /// Gets an address by address identifier
        /// </summary>
        /// <param name="addressId">Address identifier</param>
        /// <returns>Address</returns>
        public virtual Address GetAddressById(int addressId)
        {
            if (addressId == 0)
                return null;

            var key = string.Format(NopCommonDefaults.AddressesByIdCacheKey, addressId);
            return _cacheManager.Get(key, () => _addressRepository.GetById(addressId));
        }

        /// <summary>
        /// Inserts an address
        /// </summary>
        /// <param name="address">Address</param>
        public virtual void InsertAddress(Address address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            address.CreatedOnUtc = DateTime.UtcNow;

            //some validation
            if (address.CountryId == 0)
                address.CountryId = null;
            if (address.StateProvinceId == 0)
                address.StateProvinceId = null;

            _addressRepository.Insert(address);

            //cache
            _cacheManager.RemoveByPattern(NopCommonDefaults.AddressesPatternCacheKey);

            //event notification
            _eventPublisher.EntityInserted(address);
        }

        /// <summary>
        /// Updates the address
        /// </summary>
        /// <param name="address">Address</param>
        public virtual void UpdateAddress(Address address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            //some validation
            if (address.CountryId == 0)
                address.CountryId = null;
            if (address.StateProvinceId == 0)
                address.StateProvinceId = null;

            _addressRepository.Update(address);

            //cache
            _cacheManager.RemoveByPattern(NopCommonDefaults.AddressesPatternCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(address);
        }

        /// <summary>
        /// Gets a value indicating whether address is valid (can be saved)
        /// </summary>
        /// <param name="address">Address to validate</param>
        /// <returns>Result</returns>
        public virtual bool IsAddressValid(Address address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            if (string.IsNullOrWhiteSpace(address.FirstName))
                return false;

            if (string.IsNullOrWhiteSpace(address.LastName))
                return false;

            if (string.IsNullOrWhiteSpace(address.Email))
                return false;

            if (_addressSettings.CompanyEnabled &&
                _addressSettings.CompanyRequired &&
                string.IsNullOrWhiteSpace(address.Company))
                return false;

            if (_addressSettings.StreetAddressEnabled &&
                _addressSettings.StreetAddressRequired &&
                string.IsNullOrWhiteSpace(address.Address1))
                return false;

            if (_addressSettings.StreetAddress2Enabled &&
                _addressSettings.StreetAddress2Required &&
                string.IsNullOrWhiteSpace(address.Address2))
                return false;

            if (_addressSettings.ZipPostalCodeEnabled &&
                _addressSettings.ZipPostalCodeRequired &&
                string.IsNullOrWhiteSpace(address.ZipPostalCode))
                return false;

            if (_addressSettings.CountryEnabled)
            {
                if (address.CountryId == null || address.CountryId.Value == 0)
                    return false;

                var country = _countryService.GetCountryById(address.CountryId.Value);
                if (country == null)
                    return false;

                if (_addressSettings.StateProvinceEnabled)
                {
                    var states = _stateProvinceService.GetStateProvincesByCountryId(country.Id);
                    if (states.Any())
                    {
                        if (address.StateProvinceId == null || address.StateProvinceId.Value == 0)
                            return false;

                        var state = states.FirstOrDefault(x => x.Id == address.StateProvinceId.Value);
                        if (state == null)
                            return false;
                    }
                }
            }

            if (_addressSettings.CountyEnabled &&
                _addressSettings.CountyRequired &&
                string.IsNullOrWhiteSpace(address.County))
                return false;

            if (_addressSettings.CityEnabled &&
                _addressSettings.CityRequired &&
                (address.CountryId == null || address.CountryId.Value == 0))
                return false;

            if (_addressSettings.PhoneEnabled &&
                _addressSettings.PhoneRequired &&
                string.IsNullOrWhiteSpace(address.PhoneNumber))
                return false;

            if (_addressSettings.FaxEnabled &&
                _addressSettings.FaxRequired &&
                string.IsNullOrWhiteSpace(address.FaxNumber))
                return false;

            var attributes = _addressAttributeService.GetAllAddressAttributes();
            if (attributes.Any(x => x.IsRequired))
                return false;

            return true;
        }

        /// <summary>
        /// Find an address
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="firstName">First name</param>
        /// <param name="lastName">Last name</param>
        /// <param name="phoneNumber">Phone number</param>
        /// <param name="email">Email</param>
        /// <param name="faxNumber">Fax number</param>
        /// <param name="company">Company</param>
        /// <param name="address1">Address 1</param>
        /// <param name="address2">Address 2</param>
        /// <param name="city">City</param>
        /// <param name="county">County</param>
        /// <param name="stateProvinceId">State/province identifier</param>
        /// <param name="zipPostalCode">Zip postal code</param>
        /// <param name="countryId">Country identifier</param>
        /// <param name="customAttributes">Custom address attributes (XML format)</param>
        /// <returns>Address</returns>
        public virtual Address FindAddress(List<Address> source, string firstName, string lastName, string phoneNumber, string email,
            string faxNumber, string company, string address1, string address2, string county, int? stateProvinceId, int? cityId,
            string zipPostalCode, int? countryId, string customAttributes, string houseNo = "", string floorNo = "", string roomNo = "", string ropeColor = "", bool isLiftOption = false)
        {
            return source.Find(a => ((string.IsNullOrEmpty(a.FirstName) && string.IsNullOrEmpty(firstName)) || a.FirstName == firstName) &&
                ((string.IsNullOrEmpty(a.LastName) && string.IsNullOrEmpty(lastName)) || a.LastName == lastName) &&
                ((string.IsNullOrEmpty(a.PhoneNumber) && string.IsNullOrEmpty(phoneNumber)) || a.PhoneNumber == phoneNumber) &&
                ((string.IsNullOrEmpty(a.Email) && string.IsNullOrEmpty(email)) || a.Email == email) &&
                ((string.IsNullOrEmpty(a.FaxNumber) && string.IsNullOrEmpty(faxNumber)) || a.FaxNumber == faxNumber) &&
                ((string.IsNullOrEmpty(a.Company) && string.IsNullOrEmpty(company)) || a.Company == company) &&
                ((string.IsNullOrEmpty(a.Address1) && string.IsNullOrEmpty(address1)) || a.Address1 == address1) &&
                ((string.IsNullOrEmpty(a.Address2) && string.IsNullOrEmpty(address2)) || a.Address2 == address2) &&
                ((a.CityId == null && cityId == null) || a.CityId.Value == cityId.Value) &&
                ((string.IsNullOrEmpty(a.County) && string.IsNullOrEmpty(county)) || a.County == county) &&
                ((a.StateProvinceId == null && stateProvinceId == null) || a.StateProvinceId.Value == stateProvinceId.Value) &&
                ((string.IsNullOrEmpty(a.ZipPostalCode) && string.IsNullOrEmpty(zipPostalCode)) || a.ZipPostalCode == zipPostalCode) &&
                ((a.CountryId == null && countryId == null) || a.CountryId.Value == countryId.Value) &&
                //actually we should parse custom address attribute (in case if "Display order" is changed) and then compare
                //bu we simplify this process and simply compare their values in XML
                ((string.IsNullOrEmpty(a.CustomAttributes) && string.IsNullOrEmpty(customAttributes)) || a.CustomAttributes == customAttributes) &&

                ((string.IsNullOrEmpty(a.HouseNo) && string.IsNullOrEmpty(houseNo)) || a.HouseNo == houseNo) &&
                ((string.IsNullOrEmpty(a.FloorNo) && string.IsNullOrEmpty(floorNo)) || a.FloorNo == floorNo) &&
                ((string.IsNullOrEmpty(a.RoomNo) && string.IsNullOrEmpty(roomNo)) || a.RoomNo == roomNo) &&
                ((string.IsNullOrEmpty(a.RopeColor) && string.IsNullOrEmpty(ropeColor)) || a.RopeColor == ropeColor) &&
                ((a.IsLiftOption && isLiftOption) || a.IsLiftOption == isLiftOption)
                );
        }


        /// <summary>
        /// Added By Ankur to find Address
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="email"></param>
        /// <param name="faxNumber"></param>
        /// <param name="company"></param>
        /// <param name="address1"></param>
        /// <param name="address2"></param>
        /// <param name="county"></param>
        /// <param name="stateProvinceId"></param>
        /// <param name="cityId"></param>
        /// <param name="zipPostalCode"></param>
        /// <param name="countryId"></param>
        /// <param name="customAttributes"></param>
        /// <returns></returns>
        public virtual Address FindAddress(string firstName, string lastName, string phoneNumber, string email,
            string faxNumber, string company, string address1, string address2, string county, int? stateProvinceId, int? cityId,
            string zipPostalCode, int? countryId, string customAttributes = "")
        {
            if (stateProvinceId == 0)
                return null;

            //var query = from a in _addressRepository.Table
            //            where a.Email == email && a.StateProvinceId == stateProvinceId.Value && a.FirstName == firstName
            //            && a.LastName == lastName && a.PhoneNumber == phoneNumber && a.FaxNumber == faxNumber && a.CityId == cityId &&
            //            a.Address1 == address1 && a.Address2 == a.Address2 && a.z
            //            select a;
            var query = from a in _addressRepository.Table
                        where
((string.IsNullOrEmpty(a.FirstName) && string.IsNullOrEmpty(firstName)) || a.FirstName == firstName) &&
((string.IsNullOrEmpty(a.LastName) && string.IsNullOrEmpty(lastName)) || a.LastName == lastName) &&
((string.IsNullOrEmpty(a.PhoneNumber) && string.IsNullOrEmpty(phoneNumber)) || a.PhoneNumber == phoneNumber) &&
((string.IsNullOrEmpty(a.Email) && string.IsNullOrEmpty(email)) || a.Email == email) &&
((string.IsNullOrEmpty(a.FaxNumber) && string.IsNullOrEmpty(faxNumber)) || a.FaxNumber == faxNumber) &&
((string.IsNullOrEmpty(a.Company) && string.IsNullOrEmpty(company)) || a.Company == company) &&
((string.IsNullOrEmpty(a.Address1) && string.IsNullOrEmpty(address1)) || a.Address1 == address1) &&
((string.IsNullOrEmpty(a.Address2) && string.IsNullOrEmpty(address2)) || a.Address2 == address2) &&
((a.CityId == null && cityId == null) || a.CityId.Value == cityId.Value) &&
((string.IsNullOrEmpty(a.County) && string.IsNullOrEmpty(county)) || a.County == county) &&
((a.StateProvinceId == null && stateProvinceId == null) || a.StateProvinceId.Value == stateProvinceId.Value) &&
((string.IsNullOrEmpty(a.ZipPostalCode) && string.IsNullOrEmpty(zipPostalCode)) || a.ZipPostalCode == zipPostalCode) &&
((a.CountryId == null && countryId == null) || a.CountryId.Value == countryId.Value) &&
//actually we should parse custom address attribute (in case if "Display order" is changed) and then compare
//bu we simplify this process and simply compare their values in XML
((string.IsNullOrEmpty(a.CustomAttributes) && string.IsNullOrEmpty(customAttributes)) || a.CustomAttributes == customAttributes)
                        select a;
            return query == null ? null : query.ToList().First();
        }

        #endregion
    }
}
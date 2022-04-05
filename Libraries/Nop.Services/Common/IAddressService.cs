using System.Collections.Generic;
using Nop.Core.Domain.Common;

namespace Nop.Services.Common
{
    /// <summary>
    /// Address service interface
    /// </summary>
    public partial interface IAddressService
    {
        /// <summary>
        /// Deletes an address
        /// </summary>
        /// <param name="address">Address</param>
        void DeleteAddress(Address address);

        /// <summary>
        /// Gets total number of addresses by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <returns>Number of addresses</returns>
        int GetAddressTotalByCountryId(int countryId);

        /// <summary>
        /// Gets total number of addresses by state/province identifier
        /// </summary>
        /// <param name="stateProvinceId">State/province identifier</param>
        /// <returns>Number of addresses</returns>
        int GetAddressTotalByStateProvinceId(int stateProvinceId);

        /// <summary>
        /// Gets total number of addresses by city identifier
        /// </summary>
        /// <param name="cityId">cityId identifier</param>
        /// <returns>Number of addresses</returns>
        int GetAddressTotalByCityId(int cityId);

        /// <summary>
        /// Gets an address by address identifier
        /// </summary>
        /// <param name="addressId">Address identifier</param>
        /// <returns>Address</returns>
        Address GetAddressById(int addressId);

        /// <summary>
        /// Gets an address by mobile number
        /// Created By : Alexandar Rajavel
        /// Created On : 21-Sep-2018
        /// </summary>
        /// <param name="mobileNumber">Address identifier</param>
        /// <returns>Address</returns>
        Address GetAddressByMobileNumber(string mobileNumber);

        /// <summary>
        /// Inserts an address
        /// </summary>
        /// <param name="address">Address</param>
        void InsertAddress(Address address);

        /// <summary>
        /// Updates the address
        /// </summary>
        /// <param name="address">Address</param>
        void UpdateAddress(Address address);

        /// <summary>
        /// Gets a value indicating whether address is valid (can be saved)
        /// </summary>
        /// <param name="address">Address to validate</param>
        /// <returns>Result</returns>
        bool IsAddressValid(Address address);

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
        Address FindAddress(List<Address> source, string firstName, string lastName, string phoneNumber, string email,
            string faxNumber, string company, string address1, string address2, string county, int? stateProvinceId, int? CityId,
            string zipPostalCode, int? countryId, string customAttributes, string houseNo = "", string floorNo = "", string roomNo = "", string ropeColor = "", bool isLiftOption = false);

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
        Address FindAddress(string firstName, string lastName, string phoneNumber, string email,
            string faxNumber, string company, string address1, string address2, string county, int? stateProvinceId, int? cityId,
            string zipPostalCode, int? countryId, string customAttributes = "");
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Factories
{
    /// <summary>
    /// Represents the interface of the country model factory
    /// </summary>
    public partial interface ICountryModelFactoryApi
    {
        /// <summary>
        /// Get states and provinces by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="addSelectStateItem">Whether to add "Select state" item to list of states</param>
        /// <returns>List of identifiers and names of states and provinces</returns>
        dynamic GetStatesByCountryId(string countryId, bool addSelectStateItem);

        /// <summary>
        /// Get cities by state identifier
        /// </summary>
        /// <param name="stateId">State identifier</param>
        /// <param name="addSelectCityItem">Whether to add "Select city" item to list of cities</param>
        /// <returns>List of identifiers and names of cities</returns>
        dynamic GetCitiesByStateId(string stateId, bool addSelectCityItem);

        /// <summary>
        /// Get shipping allowed cities by state identifier
        /// </summary>
        /// <param name="stateId">State identifier</param>
        /// <param name="addSelectCityItem">Whether to add "Select city" item to list of cities</param>
        /// <returns>List of identifiers and names of cities</returns>
        dynamic GetShippingAllowedCitiesByStateId(string stateId, bool addSelectCityItem);

    }
}

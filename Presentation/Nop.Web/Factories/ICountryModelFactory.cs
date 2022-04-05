using System.Collections.Generic;
using Nop.Web.Models.Directory;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the country model factory
    /// </summary>
    public partial interface ICountryModelFactory
    {
        /// <summary>
        /// Get states and provinces by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="addSelectStateItem">Whether to add "Select state" item to list of states</param>
        /// <returns>List of identifiers and names of states and provinces</returns>
        IList<StateProvinceModel> GetStatesByCountryId(string countryId, bool addSelectStateItem);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="languageId"></param>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        IList<CityModel> GetCitiesByStateIdForShipping(string stateId, bool addSelectStateItem, int languageId = 0, bool showHidden = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="languageId"></param>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        IList<CityModel> GetCitiesByStateId(string stateId, bool addSelectStateItem, int languageId = 0, bool showHidden = false);
    }
}

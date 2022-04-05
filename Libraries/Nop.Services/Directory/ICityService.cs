using System.Collections.Generic;
using Nop.Core.Domain.Directory;

namespace Nop.Services.Directory
{
    public partial interface ICityService
    {
        /*
        Created By : Ankur Shrivastava 
        Created On : 25 August 2018
        Description : City service declaration
        */

        /// <summary>
        /// Deletes a City
        /// </summary>
        /// <param name="city">The city</param>
        void DeleteCity(City city);

        /// <summary>
        /// Gets a City
        /// </summary>
        /// <param name="cityId">The City identifier</param>
        /// <returns>City</returns>
        City GetCityById(int cityId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateID"></param>
        /// <param name="cityName"></param>
        City GetCityByStateIdAndCityName(int stateID, string cityName);

        /// <summary>
        /// Gets a City collection by country identifier
        /// </summary>
        /// <param name="stateId">Country identifier</param>
        /// <param name="languageId">Language identifier. It's used to sort states by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>States</returns>
        IList<City> GetCitiesByStateId(int stateId, int languageId = 0, bool showHidden = false);

        /// <summary>
        /// Gets a City collection by country identifier
        /// </summary>
        /// <param name="stateId">Country identifier</param>
        /// <param name="languageId">Language identifier. It's used to sort states by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>States</returns>
        IList<City> GetCitiesByStateIdForShipping(int stateId, int languageId = 0, bool showHidden = false);
           
        /// <summary>
        /// Inserts a City
        /// </summary>
        /// <param name="City">City</param>
        void InsertCity(City city);

        /// <summary>
        /// Updates a City
        /// </summary>
        /// <param name="City">City</param>
        void UpdateCity(City city);

        /// <summary>
        /// GetShippingandDeliveryEnabledCities
        /// </summary>
        /// <returns></returns>
        IList<City> GetShippingandDeliveryEnabledCities();


        /// <summary>
        /// GetCities
        /// </summary>
        /// <returns></returns>
        IList<City> GetCities();
    }
}


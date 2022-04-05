using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Directory;
using Nop.Services.Events;
using Nop.Services.Localization;

namespace Nop.Services.Directory
{
    public partial class CityService : ICityService
    {
        /*
        Created By : Ankur Shrivastava 
        Created On : 25 August 2018
        Description : City service implementation
        */

        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<City> _cityRepository;

        #endregion

        #region Ctor

        public CityService(ICacheManager cacheManager,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            IRepository<City> cityRepository)
        {
            this._cacheManager = cacheManager;
            this._eventPublisher = eventPublisher;
            this._localizationService = localizationService;
            this._cityRepository = cityRepository;
        }

        #endregion

        /// <summary>
        /// Delete city
        /// </summary>
        /// <param name="city"></param>
        public virtual void DeleteCity(City city)
        {
            if (city == null)
                throw new ArgumentNullException(nameof(city));

            _cityRepository.Delete(city);

            //_cacheManager.RemoveByPattern(NopDirectoryDefaults.StateProvincesPatternCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(city);
        }

        /// <summary>
        /// GetCityById
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public virtual City GetCityById(int cityId)
        {
            if (cityId == 0)
                return null;

            return _cityRepository.GetById(cityId);
        }

        /// <summary>
        /// GetCityByStateIdAndCityName
        /// Created By : Ankur Shrivastava
        /// Date : 6 Sep. 2018
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public virtual City GetCityByStateIdAndCityName(int stateID, string cityName)
        {
            if (stateID == 0)
                return null;

            return _cityRepository.Table.Where(c => c.StateId == stateID && c.Name.ToLower().Contains(cityName.ToLower().Trim())).FirstOrDefault();
        }

        /// <summary>
        /// GetCitiesByStateId
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="languageId"></param>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        public IList<City> GetCitiesByStateId(int stateId, int languageId, bool showHidden)
        {
            //var key = string.Format(NopDirectoryDefaults.StateProvincesAllCacheKey, countryId, languageId, showHidden);
            //return _cacheManager.Get(key, () =>
            //{
            var query = from c in _cityRepository.Table
                        orderby c.DisplayOrder, c.Name
                        where c.StateId == stateId &&
                        (c.Published==true) &&
                        (c.IsDeliveryAllowed==true) &&
                        (c.IsShippingAllowed==true)

                        select c;
            var cities = query.ToList();

            if (languageId > 0)
            {
                //we should sort states by localized names when they have the same display order
                cities = cities
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => _localizationService.GetLocalized(c, x => x.Name, languageId))
                .ToList();
            }

            return cities;
            //});
        }

        /// <summary>
        /// GetCitysByStateIdForShipping
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="languageId"></param>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        public virtual IList<City> GetCitiesByStateIdForShipping(int stateId, int languageId, bool showHidden)
        {
            var query = from c in _cityRepository.Table
                        orderby c.DisplayOrder, c.Name
                        where c.StateId == stateId && c.IsShippingAllowed == true &&
                        (showHidden || c.Published)
                        select c;
            var cities = query.ToList();

            if (languageId > 0)
            {
                //we should sort states by localized names when they have the same display order
                cities = cities
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => _localizationService.GetLocalized(c, x => x.Name, languageId))
                .ToList();
            }

            return cities;
        }

        /// <summary>
        /// InsertCity
        /// </summary>
        /// <param name="city"></param>
        public virtual void InsertCity(City city)
        {
            if (city == null)
                throw new ArgumentNullException(nameof(city));

            _cityRepository.Insert(city);

            //_cacheManager.RemoveByPattern(NopDirectoryDefaults.StateProvincesPatternCacheKey);

            //event notification
            _eventPublisher.EntityInserted(city);
        }

        /// <summary>
        /// UpdateCity
        /// </summary>
        /// <param name="city"></param>
        public virtual void UpdateCity(City city)
        {
            if (city == null)
                throw new ArgumentNullException(nameof(city));

            _cityRepository.Update(city);

            //_cacheManager.RemoveByPattern(NopDirectoryDefaults.StateProvincesPatternCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(city);
        }

        //Added Code By Sunil Kumar at 30-04-2020 for Township List with Shipping & Delivery Enabled and Published
        /// <summary>
        /// GetShippingandDeliveryEnabledCities
        /// </summary>
        /// <returns></returns>
        public IList<City> GetShippingandDeliveryEnabledCities()
        {
            var query = from c in _cityRepository.Table
                        orderby c.DisplayOrder, c.Name
                        where c.IsDeliveryAllowed == true && c.IsShippingAllowed == true &&
                        c.Published == true
                        select c;
            var cities = query.ToList();

            //if (languageId > 0)
            //{
            //    //we should sort states by localized names when they have the same display order
            //    cities = cities
            //    .OrderBy(c => c.DisplayOrder)
            //    .ThenBy(c => _localizationService.GetLocalized(c, x => x.Name, languageId))
            //    .ToList();
            //}

            return cities;
        }

        //Added Code By Sunil Kumar at 04-05-2020 for List of Township
        /// <summary>
        /// GetCities
        /// </summary>
        /// <returns></returns>
        public IList<City> GetCities()
        {
            var query = from c in _cityRepository.Table
                        orderby c.DisplayOrder, c.Name
                        where 
                        c.Published == true
                        select c;
            var cities = query.ToList();

            return cities;
        }
    }
}

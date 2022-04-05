using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Directory;
using Nop.Services.Localization;
using BS.Plugin.NopStation.MobileWebApi.Infrastructure.Cache;

namespace BS.Plugin.NopStation.MobileWebApi.Factories
{
    /// <summary>
    /// Represents the country model factory
    /// </summary>
    public partial class CountryModelFactoryApi : ICountryModelFactoryApi
    {
        #region Fields

        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;
        private readonly ICityService _cityService;

        #endregion

        #region Constructors

        public CountryModelFactoryApi(ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ICityService cityService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ICacheManager cacheManager)
        {
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._cityService = cityService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get states and provinces by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="addSelectStateItem">Whether to add "Select state" item to list of states</param>
        /// <returns>List of identifiers and names of states and provinces</returns>
        public virtual dynamic GetStatesByCountryId(string countryId, bool addSelectStateItem)
        {
            if (String.IsNullOrEmpty(countryId))
                throw new ArgumentNullException("countryId");

            string cacheKey = string.Format(ModelCacheEventConsumer.STATEPROVINCES_BY_COUNTRY_MODEL_KEY, countryId, addSelectStateItem, _workContext.WorkingLanguage.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var country = _countryService.GetCountryById(Convert.ToInt32(countryId));
                var states = _stateProvinceService.GetStateProvincesByCountryId(country != null ? country.Id : 0, _workContext.WorkingLanguage.Id).ToList();
                var result = (from s in states
                              select new { id = s.Id, name = _localizationService.GetLocalized(s, x => x.Name) })
                              .ToList();


                if (country == null)
                {
                    //country is not selected ("choose country" item)
                    if (addSelectStateItem)
                    {
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.SelectState") });
                    }
                    else
                    {
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.OtherNonUS") });
                    }
                }
                else
                {
                    //some country is selected
                    if (!result.Any())
                    {
                        //country does not have states
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.OtherNonUS") });
                    }
                    else
                    {
                        //country has some states
                        if (addSelectStateItem)
                        {
                            result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.SelectState") });
                        }
                    }
                }

                return result;
            });
            return cachedModel;
        }

        /// <summary>
        /// Get city list by state identifier
        /// </summary>
        /// <param name="stateId">State identifier</param>
        /// <returns>List of identifiers and names of cities</returns>
        public virtual dynamic GetCitiesByStateId(string stateId, bool addSelectCityItem)
        {
            if (String.IsNullOrEmpty(stateId))
                throw new ArgumentNullException("stateId");

            string cacheKey = string.Format(ModelCacheEventConsumer.CITIES_BY_STATE_MODEL_KEY, stateId, addSelectCityItem, _workContext.WorkingLanguage.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var state = _stateProvinceService.GetStateProvinceById(Convert.ToInt32(stateId));
                var cities = _cityService.GetCitiesByStateId(state != null ? state.Id : 0, _workContext.WorkingLanguage.Id).ToList();
                var result = (from s in cities
                              select new { id = s.Id, name = _localizationService.GetLocalized(s, x => x.Name) })
                              .ToList();


                if (state == null)
                {
                    //state is not selected ("choose state" item)
                    if (addSelectCityItem)
                    {
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.SelectCity") });
                    }
                    else
                    {
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.OtherNonUS") });
                    }
                }
                else
                {
                    //some state is selected
                    if (!result.Any())
                    {
                        //state does not have cities
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.OtherNonUS") });
                    }
                    else
                    {
                        //state has some cities
                        if (addSelectCityItem)
                        {
                            result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.SelectCity") });
                        }
                    }
                }

                return result;
            });
            return cachedModel;
        }

        /// <summary>
        /// Get shipping allowed city list by state identifier
        /// </summary>
        /// <param name="stateId">State identifier</param>
        /// <returns>List of identifiers and names of cities</returns>
        public virtual dynamic GetShippingAllowedCitiesByStateId(string stateId, bool addSelectCityItem)
        {
            if (String.IsNullOrEmpty(stateId))
                throw new ArgumentNullException("stateId");

            string cacheKey = string.Format(ModelCacheEventConsumer.CITIES_BY_STATE_MODEL_KEY, stateId, addSelectCityItem, _workContext.WorkingLanguage.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var state = _stateProvinceService.GetStateProvinceById(Convert.ToInt32(stateId));
                var cities = _cityService.GetCitiesByStateIdForShipping(state != null ? state.Id : 0, _workContext.WorkingLanguage.Id).ToList();
                var result = (from s in cities
                              select new { id = s.Id, name = _localizationService.GetLocalized(s, x => x.Name) })
                              .ToList();


                if (state == null)
                {
                    //state is not selected ("choose state" item)
                    if (addSelectCityItem)
                    {
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.SelectCity") });
                    }
                    else
                    {
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.OtherNonUS") });
                    }
                }
                else
                {
                    //some state is selected
                    if (!result.Any())
                    {
                        //state does not have cities
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.OtherNonUS") });
                    }
                    else
                    {
                        //state has some cities
                        if (addSelectCityItem)
                        {
                            result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.SelectCity") });
                        }
                    }
                }

                return result;
            });
            return cachedModel;
        }

        #endregion
    }
}

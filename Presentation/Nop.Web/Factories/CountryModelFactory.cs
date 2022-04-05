using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Directory;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the country model factory
    /// </summary>
    public partial class CountryModelFactory : ICountryModelFactory
    {
		#region Fields

        private readonly ICountryService _countryService;
        private readonly ILocalizationService _localizationService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICityService _cityService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IWorkContext _workContext;
       

        #endregion

        #region Ctor

        public CountryModelFactory(ICountryService countryService,
            ILocalizationService localizationService,
            IStateProvinceService stateProvinceService,
            ICityService cityService,
            IStaticCacheManager cacheManager,
            IWorkContext workContext)
        {
            this._countryService = countryService;
            this._localizationService = localizationService;
            this._stateProvinceService = stateProvinceService;
            this._cityService = cityService;
            this._cacheManager = cacheManager;
            this._workContext = workContext;
        }



        #endregion

        #region Methods

        public IList<CityModel> GetCitiesByStateIdForShipping(string stateId, bool addSelectStateItem, int languageId = 0, bool showHidden = false)
        {
            if (string.IsNullOrEmpty(stateId))
                throw new ArgumentNullException(nameof(stateId));

            //var cacheKey = string.Format(ModelCacheEventConsumer.STATEPROVINCES_BY_COUNTRY_MODEL_KEY, stateId, addSelectStateItem, _workContext.WorkingLanguage.Id);
            //var cachedModel = _cacheManager.Get(cacheKey, () =>
            //{
            var state = _stateProvinceService.GetStateProvinceById(Convert.ToInt32(stateId));
            var cities = _cityService.GetCitiesByStateIdForShipping(state != null ? state.Id : 0, _workContext.WorkingLanguage.Id).ToList();
            var result = new List<CityModel>();
            foreach (var city in cities)
                result.Add(new CityModel
                {
                    id = city.Id,
                    name = _localizationService.GetLocalized(city, x => x.Name)
                });

            if (state == null)
            {
                //country is not selected ("choose country" item)
                if (addSelectStateItem)
                {
                    result.Insert(0, new CityModel
                    {
                        id = 0,
                        name = _localizationService.GetResource("Address.SelectCity")
                    });
                }
                else
                {
                    result.Insert(0, new CityModel
                    {
                        id = 0,
                        name = _localizationService.GetResource("Address.SelectCity")
                    });
                }
            }
            else
            {
                //some country is selected
                if (!result.Any())
                {
                    //country does not have states
                    result.Insert(0, new CityModel
                    {
                        id = 0,
                        name = _localizationService.GetResource("Address.SelectCity")
                    });
                }
                else
                {
                    //country has some states
                    if (addSelectStateItem)
                    {
                        result.Insert(0, new CityModel
                        {
                            id = 0,
                            name = _localizationService.GetResource("Address.SelectCity")
                        });
                    }
                }
            }

            return result;
            //});
            //return cachedModel;
        }

        public IList<CityModel> GetCitiesByStateId(string stateId, bool addSelectStateItem, int languageId = 0, bool showHidden = false)
        {
            if (string.IsNullOrEmpty(stateId))
                throw new ArgumentNullException(nameof(stateId));

            //var cacheKey = string.Format(ModelCacheEventConsumer.STATEPROVINCES_BY_COUNTRY_MODEL_KEY, stateId, addSelectStateItem, _workContext.WorkingLanguage.Id);
            //var cachedModel = _cacheManager.Get(cacheKey, () =>
            //{
            var state = _stateProvinceService.GetStateProvinceById(Convert.ToInt32(stateId));
            var cities = _cityService.GetCitiesByStateId(state != null ? state.Id : 0, _workContext.WorkingLanguage.Id).ToList();
            var result = new List<CityModel>();
            foreach (var city in cities)
                result.Add(new CityModel
                {
                    id = city.Id,
                    name = _localizationService.GetLocalized(city, x => x.Name)
                });

            if (state == null)
            {
                //country is not selected ("choose country" item)
                if (addSelectStateItem)
                {
                    result.Insert(0, new CityModel
                    {
                        id = 0,
                        name = _localizationService.GetResource("Address.SelectCity")
                    });
                }
                else
                {
                    result.Insert(0, new CityModel
                    {
                        id = 0,
                        name = _localizationService.GetResource("Address.SelectCity")
                    });
                }
            }
            else
            {
                //some country is selected
                if (!result.Any())
                {
                    //country does not have states
                    result.Insert(0, new CityModel
                    {
                        id = 0,
                        name = _localizationService.GetResource("Address.SelectCity")
                    });
                }
                else
                {
                    //country has some states
                    if (addSelectStateItem)
                    {
                        result.Insert(0, new CityModel
                        {
                            id = 0,
                            name = _localizationService.GetResource("Address.SelectCity")
                        });
                    }
                }
            }

            return result;
            //});
            //return cachedModel;
        }

        /// <summary>
        /// Get states and provinces by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="addSelectStateItem">Whether to add "Select state" item to list of states</param>
        /// <returns>List of identifiers and names of states and provinces</returns>
        public virtual IList<StateProvinceModel> GetStatesByCountryId(string countryId, bool addSelectStateItem)
        {
            if (string.IsNullOrEmpty(countryId))
                throw new ArgumentNullException(nameof(countryId));

            var cacheKey = string.Format(ModelCacheEventConsumer.STATEPROVINCES_BY_COUNTRY_MODEL_KEY, countryId, addSelectStateItem, _workContext.WorkingLanguage.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var country = _countryService.GetCountryById(Convert.ToInt32(countryId));
                var states = _stateProvinceService.GetStateProvincesByCountryId(country != null ? country.Id : 0, _workContext.WorkingLanguage.Id).ToList();
                var result = new List<StateProvinceModel>();
                foreach (var state in states)
                    result.Add(new StateProvinceModel
                    {
                        id = state.Id,
                        name = _localizationService.GetLocalized(state, x => x.Name)
                    });

                if (country == null)
                {
                    //country is not selected ("choose country" item)
                    if (addSelectStateItem)
                    {
                        result.Insert(0, new StateProvinceModel
                        {
                            id = 0,
                            name = _localizationService.GetResource("Address.SelectState")
                        });
                    }
                    else
                    {
                        result.Insert(0, new StateProvinceModel
                        {
                            id = 0,
                            name = _localizationService.GetResource("Address.SelectState")
                        });
                    }
                }
                else
                {
                    //some country is selected
                    if (!result.Any())
                    {
                        //country does not have states
                        result.Insert(0, new StateProvinceModel
                        {
                            id = 0,
                            name = _localizationService.GetResource("Address.SelectState")
                        });
                    }
                    else
                    {
                        //country has some states
                        if (addSelectStateItem)
                        {
                            result.Insert(0, new StateProvinceModel
                            {
                                id = 0,
                                name = _localizationService.GetResource("Address.SelectState")
                            });
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileWebApi.Factories;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Directory;
using BS.Plugin.NopStation.MobileWebApi.Infrastructure.Cache;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Directory;
using Nop.Services.Localization;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class CountryController : BaseApiController
    {
        #region Fields

        private readonly ICountryModelFactoryApi _countryModelFactoryApi;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;
        #endregion

        #region Constructors

        public CountryController(ICountryModelFactoryApi countryModelFactoryApi,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ICacheManager cacheManager)
        {
            this._countryModelFactoryApi = countryModelFactoryApi;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region States / provinces

        [Route("api/country/getstatesbycountryid/{countryId}")]
        [HttpGet]
        public IActionResult GetStatesByCountryId(string countryId, bool addSelectStateItem = false)
        {
            var model = _countryModelFactoryApi.GetStatesByCountryId(countryId, addSelectStateItem);

            var response = new GeneralResponseModel<object>()
            {
                Data = model
            };
            return Ok(response);
        }

        [Route("api/country/getcitiesbystateid/{stateId}")]
        [HttpGet]
        public IActionResult GetCitiesByStateId(string stateId, bool addSelectCityItem = false)
        {
            var model = _countryModelFactoryApi.GetCitiesByStateId(stateId, addSelectCityItem);
            var response = new GeneralResponseModel<object>()
            {
                Data = model
            };
            return Ok(response);
        }

        [Route("api/country/getshippingallowedcitiesbystateid/{stateId}")]
        [HttpGet]
        public IActionResult GetShippingAllowedCitiesByStateId(string stateId, bool addSelectCityItem = false)
        {
            var model = _countryModelFactoryApi.GetShippingAllowedCitiesByStateId(stateId, addSelectCityItem);
            var response = new GeneralResponseModel<object>()
            {
                Data = model
            };
            return Ok(response);
        }
        #endregion
    }
}

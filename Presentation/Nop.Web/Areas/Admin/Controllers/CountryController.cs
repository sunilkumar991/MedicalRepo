﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class CountryController : BaseAdminController
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly ICountryModelFactory _countryModelFactory;
        private readonly ICountryService _countryService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPermissionService _permissionService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly ICityService _cityService;
        #endregion

        #region Ctor

        public CountryController(IAddressService addressService,
            ICountryModelFactory countryModelFactory,
            ICountryService countryService,
            ICustomerActivityService customerActivityService,
            IExportManager exportManager,
            IImportManager importManager,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IPermissionService permissionService,
            IStateProvinceService stateProvinceService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            ICityService cityService)
        {
            this._addressService = addressService;
            this._countryModelFactory = countryModelFactory;
            this._countryService = countryService;
            this._customerActivityService = customerActivityService;
            this._exportManager = exportManager;
            this._importManager = importManager;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._permissionService = permissionService;
            this._stateProvinceService = stateProvinceService;
            this._storeMappingService = storeMappingService;
            this._storeService = storeService;
            this._cityService = cityService;
        }

        #endregion

        #region Utilities

        protected virtual void UpdateLocales(Country country, CountryModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(country,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual void UpdateLocales(StateProvince stateProvince, StateProvinceModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(stateProvince,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual void UpdateLocales(City stateProvince, CityModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(stateProvince,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual void SaveStoreMappings(Country country, CountryModel model)
        {
            country.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(country);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(country, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        #endregion

        #region Countries

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //prepare model
            var model = _countryModelFactory.PrepareCountrySearchModel(new CountrySearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult CountryList(CountrySearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _countryModelFactory.PrepareCountryListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //prepare model
            var model = _countryModelFactory.PrepareCountryModel(new CountryModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(CountryModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var country = model.ToEntity<Country>();
                _countryService.InsertCountry(country);

                //activity log
                _customerActivityService.InsertActivity("AddNewCountry",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewCountry"), country.Id), country);

                //locales
                UpdateLocales(country, model);

                //Stores
                SaveStoreMappings(country, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Countries.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = country.Id });
            }

            //prepare model
            model = _countryModelFactory.PrepareCountryModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a country with the specified id
            var country = _countryService.GetCountryById(id);
            if (country == null)
                return RedirectToAction("List");

            //prepare model
            var model = _countryModelFactory.PrepareCountryModel(null, country);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(CountryModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a country with the specified id
            var country = _countryService.GetCountryById(model.Id);
            if (country == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                country = model.ToEntity(country);
                _countryService.UpdateCountry(country);

                //activity log
                _customerActivityService.InsertActivity("EditCountry",
                    string.Format(_localizationService.GetResource("ActivityLog.EditCountry"), country.Id), country);

                //locales
                UpdateLocales(country, model);

                //stores
                SaveStoreMappings(country, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Countries.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = country.Id });
            }

            //prepare model
            model = _countryModelFactory.PrepareCountryModel(model, country, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a country with the specified id
            var country = _countryService.GetCountryById(id);
            if (country == null)
                return RedirectToAction("List");

            try
            {
                if (_addressService.GetAddressTotalByCountryId(country.Id) > 0)
                    throw new NopException("The country can't be deleted. It has associated addresses");

                _countryService.DeleteCountry(country);

                //activity log
                _customerActivityService.InsertActivity("DeleteCountry",
                    string.Format(_localizationService.GetResource("ActivityLog.DeleteCountry"), country.Id), country);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Countries.Deleted"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = country.Id });
            }
        }

        [HttpPost]
        public virtual IActionResult PublishSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            if (selectedIds == null)
                return Json(new { Result = true });

            var countries = _countryService.GetCountriesByIds(selectedIds.ToArray());
            foreach (var country in countries)
            {
                country.Published = true;
                _countryService.UpdateCountry(country);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult UnpublishSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            if (selectedIds == null)
                return Json(new { Result = true });

            var countries = _countryService.GetCountriesByIds(selectedIds.ToArray());
            foreach (var country in countries)
            {
                country.Published = false;
                _countryService.UpdateCountry(country);
            }

            return Json(new { Result = true });
        }

        #endregion

        #region States / provinces

        [HttpPost]
        public virtual IActionResult States(StateProvinceSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedKendoGridJson();

            //try to get a country with the specified id
            var country = _countryService.GetCountryById(searchModel.CountryId)
                ?? throw new ArgumentException("No country found with the specified id");

            //prepare model
            var model = _countryModelFactory.PrepareStateProvinceListModel(searchModel, country);

            return Json(model);
        }

        public virtual IActionResult StateCreatePopup(int countryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a country with the specified id
            var country = _countryService.GetCountryById(countryId);
            if (country == null)
                return RedirectToAction("List");

            //prepare model
            var model = _countryModelFactory.PrepareStateProvinceModel(new StateProvinceModel(), country, null);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult StateCreatePopup(StateProvinceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a country with the specified id
            var country = _countryService.GetCountryById(model.CountryId);
            if (country == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var sp = model.ToEntity<StateProvince>();

                _stateProvinceService.InsertStateProvince(sp);

                //activity log
                _customerActivityService.InsertActivity("AddNewStateProvince",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewStateProvince"), sp.Id), sp);

                UpdateLocales(sp, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = _countryModelFactory.PrepareStateProvinceModel(model, country, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult CityCreatePopup(int countryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();
            countryId = 0;

            //prepare model
            var model = _countryModelFactory.PrepareCityModel(new CityModel(), null, null, null);
            model.CountryId = 0;
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult CityCreatePopup(CityModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var country = _countryService.GetCountryById((int)model.CountryId);
            if (country == null)
                return RedirectToAction("list");

            //try to get a country with the specified id
            var state = _stateProvinceService.GetStateProvinceById((int)model.StateProvinceId);
            if (state == null)
                return RedirectToAction("list");

            if (ModelState.IsValid)
            {
                var sp = model.ToEntity<City>();
                sp.StateId = state.Id;

                _cityService.InsertCity(sp);

                //activity log
                _customerActivityService.InsertActivity("AddNewCity",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewCity"), sp.Id), sp);

                UpdateLocales(sp, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = _countryModelFactory.PrepareCityModel(model, country, state, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }


        public virtual IActionResult StateEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a state with the specified id
            var state = _stateProvinceService.GetStateProvinceById(id);
            if (state == null)
                return RedirectToAction("List");

            //try to get a country with the specified id
            var country = _countryService.GetCountryById(state.CountryId);
            if (country == null)
                return RedirectToAction("List");

            //prepare model
            var model = _countryModelFactory.PrepareStateProvinceModel(null, country, state);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult StateEditPopup(StateProvinceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a state with the specified id
            var state = _stateProvinceService.GetStateProvinceById(model.Id);
            if (state == null)
                return RedirectToAction("List");

            //try to get a country with the specified id
            var country = _countryService.GetCountryById(state.CountryId);
            if (country == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                state = model.ToEntity(state);
                _stateProvinceService.UpdateStateProvince(state);

                //activity log
                _customerActivityService.InsertActivity("EditStateProvince",
                    string.Format(_localizationService.GetResource("ActivityLog.EditStateProvince"), state.Id), state);

                UpdateLocales(state, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = _countryModelFactory.PrepareStateProvinceModel(model, country, state, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }


        public virtual IActionResult CityEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a city with the specified id
            var city = _cityService.GetCityById(id);
            if (city == null)
                return RedirectToAction("List");

            //try to get a country with the specified id
            var state = _stateProvinceService.GetStateProvinceById(city.StateId);
            if (state == null)
                return RedirectToAction("List");

            //try to get a country with the specified id
            var country = _countryService.GetCountryById(state.CountryId);
            if (country == null)
                return RedirectToAction("List");

            //prepare model
            var model = _countryModelFactory.PrepareCityModel(null, country, state, city);

            return View(model);
        }


        [HttpPost]
        public virtual IActionResult CityEditPopup(CityModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a city with the specified id
            var city = _cityService.GetCityById(model.Id);
            if (city == null)
                return RedirectToAction("List");

            //try to get a country with the specified id
            var state = _stateProvinceService.GetStateProvinceById(city.StateId);
            if (state == null)
                return RedirectToAction("List");

            //try to get a country with the specified id
            var country = _countryService.GetCountryById(state.CountryId);
            if (country == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                city = model.ToEntity(city);
                city.IsShippingAllowed = true;
                _cityService.UpdateCity(city);

                //activity log
                _customerActivityService.InsertActivity("EditCity",
                    string.Format(_localizationService.GetResource("ActivityLog.EditCity"), city.Id), city);

                UpdateLocales(city, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = _countryModelFactory.PrepareCityModel(model, country, state, city, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }


        [HttpPost]
        public virtual IActionResult StateDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a state with the specified id
            var state = _stateProvinceService.GetStateProvinceById(id)
                ?? throw new ArgumentException("No state found with the specified id");

            if (_addressService.GetAddressTotalByStateProvinceId(state.Id) > 0)
            {
                return Json(new DataSourceResult { Errors = _localizationService.GetResource("Admin.Configuration.Countries.States.CantDeleteWithAddresses") });
            }

            //int countryId = state.CountryId;
            _stateProvinceService.DeleteStateProvince(state);

            //activity log
            _customerActivityService.InsertActivity("DeleteStateProvince",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteStateProvince"), state.Id), state);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult CityDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            //try to get a city with the specified id
            var city = _cityService.GetCityById(id)
                ?? throw new ArgumentException("No City found with the specified id");

            if (_addressService.GetAddressTotalByCityId(city.Id) > 0)
            {
                return Json(new DataSourceResult { Errors = _localizationService.GetResource("Admin.Configuration.Countries.States.CantDeleteWithAddresses") });
            }

            _cityService.DeleteCity(city);

            //activity log
            _customerActivityService.InsertActivity("DeleteCity",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteCity"), city.Id), city);

            return new NullJsonResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="addSelectStateItem"></param>
        /// <param name="addAsterisk"></param>
        /// <returns></returns>
        public virtual IActionResult GetStatesByCountryId(string countryId, bool? addSelectStateItem, bool? addAsterisk)
        {
            //permission validation is not required here

            // This action method gets called via an ajax request
            if (string.IsNullOrEmpty(countryId))
                throw new ArgumentNullException(nameof(countryId));

            var country = _countryService.GetCountryById(Convert.ToInt32(countryId));
            var states = country != null ? _stateProvinceService.GetStateProvincesByCountryId(country.Id, showHidden: true).ToList() : new List<StateProvince>();
            var result = (from s in states
                          select new { id = s.Id, name = s.Name }).ToList();

            result.Insert(0, new { id = 0, name = "Select State" });
            return Json(result);
        }

        /// <summary>
        /// Get City By City Name And State Id
        /// </summary>
        /// <param name="cityName">City Name</param>
        /// <param name="stateId">State Id</param>
        /// <returns></returns>
        public virtual IActionResult GetCityByCityNameAndStateId(string cityName, string stateId)
        {
            //permission validation is not required here

            // This action method gets called via an ajax request
            if (string.IsNullOrEmpty(cityName))
                throw new ArgumentNullException(nameof(cityName));

            // This action method gets called via an ajax request
            if (string.IsNullOrEmpty(stateId))
                throw new ArgumentNullException(nameof(stateId));

            var city = _cityService.GetCityByStateIdAndCityName(Convert.ToInt32(stateId), cityName);

            bool result = false;
            if (city != null)
            {
                result = true;
            }

            return Json(result);
        }

        /// <summary>
        /// Get City By City Name And State Id For Edit
        /// </summary>
        /// <param name="cityName">City Name</param>
        /// <param name="stateId">State Id</param>
        /// <returns></returns>
        public virtual IActionResult GetCityByCityNameAndStateIdForEdit(int Id, string cityName, int stateId, decimal shippingCharge, bool published, int displayOrder, string abbreviation)
        {
            //permission validation is not required here

            // This action method gets called via an ajax request
            if ((cityName) == null)
                throw new ArgumentNullException(nameof(cityName));

            // This action method gets called via an ajax request
            if (stateId == 0)
                throw new ArgumentNullException(nameof(stateId));

            var city = _cityService.GetCityByStateIdAndCityName(Convert.ToInt32(stateId), cityName);

            int result = 0;
            if (city != null)
            {
                if ((city.Name.ToLower() == cityName.ToLower()) && Id == city.Id)
                {
                    result = 1;
                }
                else
                {
                    result = 2;

                }
            }
            return Json(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="addSelectStateItem"></param>
        /// <param name="addAsterisk"></param>
        /// <returns></returns>
        public virtual IActionResult GetCitiesByStateId(string stateId, bool? addSelectStateItem, bool? addAsterisk)
        {
            //permission validation is not required here

            // This action method gets called via an ajax request
            if (string.IsNullOrEmpty(stateId))
                throw new ArgumentNullException(nameof(stateId));

            var cities = _cityService.GetCitiesByStateId(Convert.ToInt32(stateId));
            //var states = country != null ? _stateProvinceService.GetStateProvincesByCountryId(country.Id, showHidden: true).ToList() : new List<StateProvince>();
            var result = (from s in cities
                          select new { id = s.Id, name = s.Name }).ToList();
            if (addAsterisk.HasValue && addAsterisk.Value)
            {
                //asterisk
                result.Insert(0, new { id = 0, name = "Select City" });
            }
            else
            {
                if (cities == null)
                {
                    //country is not selected ("choose country" item)
                    if (addSelectStateItem.HasValue && addSelectStateItem.Value)
                    {
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Admin.Address.SelectCity") });
                    }
                    else
                    {
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Admin.Address.SelectCity") });
                    }
                }
                else
                {
                    //some country is selected
                    if (!result.Any())
                    {
                        //country does not have states
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Admin.Address.SelectCity") });
                    }
                    else
                    {
                        //country has some states
                        if (addSelectStateItem.HasValue && addSelectStateItem.Value)
                        {
                            result.Insert(0, new { id = 0, name = _localizationService.GetResource("Admin.Address.SelectCity") });
                        }
                    }
                }
            }

            return Json(result);
        }

        #endregion

        #region City

        [HttpPost]
        public virtual IActionResult Cities(CitySearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedKendoGridJson();

            //prepare model

            var state = _countryService.GetStateById(searchModel.StateId);

            var model = _countryModelFactory.PrepareCityListModel(searchModel, state);
            return Json(model);

        }

        #endregion

        #region Export / import

        public virtual IActionResult ExportCsv()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            var fileName = $"states_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{CommonHelper.GenerateRandomDigitCode(4)}.txt";

            var states = _stateProvinceService.GetStateProvinces(true);
            var result = _exportManager.ExportStatesToTxt(states);

            return File(Encoding.UTF8.GetBytes(result), MimeTypes.TextCsv, fileName);
        }

        [HttpPost]
        public virtual IActionResult ImportCsv(IFormFile importcsvfile)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCountries))
                return AccessDeniedView();

            try
            {
                if (importcsvfile != null && importcsvfile.Length > 0)
                {
                    var count = _importManager.ImportStatesFromTxt(importcsvfile.OpenReadStream());

                    SuccessNotification(string.Format(_localizationService.GetResource("Admin.Configuration.Countries.ImportSuccess"), count));

                    return RedirectToAction("List");
                }

                ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        #endregion
    }
}
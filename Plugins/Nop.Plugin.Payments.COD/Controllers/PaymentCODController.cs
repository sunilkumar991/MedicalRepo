﻿using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.COD.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;


namespace Nop.Plugin.Payments.COD.Controllers
{
  
    public class PaymentCODController: BasePaymentController
    {
        #region Fields

        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public PaymentCODController(
            IStoreContext storeContext,
            ISettingService settingService,
            ILocalizationService localizationService,
            ILanguageService languageService,
            IPermissionService permissionService)
        {
            this._storeContext = storeContext;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._languageService = languageService;
            this._permissionService = permissionService;
        }

        #endregion

        #region Methods
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var cashOnDeliveryPaymentSettings = _settingService.LoadSetting<CODPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                DescriptionText = cashOnDeliveryPaymentSettings.DescriptionText
            };

            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.DescriptionText = _localizationService.GetLocalizedSetting(cashOnDeliveryPaymentSettings, x => x.DescriptionText, languageId, 0, false, false);
            });

            model.AdditionalFee = cashOnDeliveryPaymentSettings.AdditionalFee;
            model.AdditionalFeePercentage = cashOnDeliveryPaymentSettings.AdditionalFeePercentage;
            model.ShippableProductRequired = cashOnDeliveryPaymentSettings.ShippableProductRequired;
            model.ActiveStoreScopeConfiguration = storeScope;

            if (storeScope > 0)
            {
                model.DescriptionText_OverrideForStore = _settingService.SettingExists(cashOnDeliveryPaymentSettings, x => x.DescriptionText, storeScope);
                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(cashOnDeliveryPaymentSettings, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(cashOnDeliveryPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
                model.ShippableProductRequired_OverrideForStore = _settingService.SettingExists(cashOnDeliveryPaymentSettings, x => x.ShippableProductRequired, storeScope);
            }

            return View("~/Plugins/Payments.COD/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var cashOnDeliveryPaymentSettings = _settingService.LoadSetting<CODPaymentSettings>(storeScope);

            //save settings
            cashOnDeliveryPaymentSettings.DescriptionText = model.DescriptionText;
            cashOnDeliveryPaymentSettings.AdditionalFee = model.AdditionalFee;
            cashOnDeliveryPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            cashOnDeliveryPaymentSettings.ShippableProductRequired = model.ShippableProductRequired;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(cashOnDeliveryPaymentSettings, x => x.DescriptionText, model.DescriptionText_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(cashOnDeliveryPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(cashOnDeliveryPaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(cashOnDeliveryPaymentSettings, x => x.ShippableProductRequired, model.ShippableProductRequired_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            //localization. no multi-store support for localization yet.
            foreach (var localized in model.Locales)
            {
                _localizationService.SaveLocalizedSetting(cashOnDeliveryPaymentSettings, x => x.DescriptionText,
                    localized.LanguageId,
                    localized.DescriptionText);
            }

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        #endregion
    }
}

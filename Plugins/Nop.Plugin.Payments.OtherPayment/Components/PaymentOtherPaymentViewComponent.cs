using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.OtherPayment.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.OtherPayment.Components
{
    [ViewComponent(Name = "PaymentOtherPayment")]
    public class PaymentOtherPaymentViewComponent : NopViewComponent
    {
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;

        public PaymentOtherPaymentViewComponent(IWorkContext workContext,
            ISettingService settingService,
            ILocalizationService localizationService,
            IStoreContext storeContext)
        {
            this._workContext = workContext;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._storeContext = storeContext;
        }

        public IViewComponentResult Invoke()
        {
            var cashOnDeliveryPaymentSettings = _settingService.LoadSetting<OtherPaymentPaymentSettings>(_storeContext.CurrentStore.Id);

            var model = new PaymentInfoModel
            {
                DescriptionText = _localizationService.GetLocalizedSetting(cashOnDeliveryPaymentSettings, x => x.DescriptionText, _workContext.WorkingLanguage.Id, 0)
            };

            return View("~/Plugins/Payments.OtherPayment/Views/PaymentInfo.cshtml", model);
        }
    }
}

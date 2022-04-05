using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Common;
using Nop.Services.DeliveryBoy;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class PaymentTransactionHistoryController : BaseAdminController
    {
      
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IPaymentTransactionHistoryModelFactory _paymentTransactionHistoryModelFactory;
        private readonly IPaymentTransactionHistoryService _paymentTransactionHistoryService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;

        #endregion Fields

        #region Ctor

        public PaymentTransactionHistoryController(CatalogSettings catalogSettings,
            ICustomerActivityService customerActivityService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPermissionService permissionService,

            IPaymentTransactionHistoryModelFactory paymentTransactionHistoryModelFactory,
            IPaymentTransactionHistoryService paymentTransactionHistoryService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService
            )
        {
            this._catalogSettings = catalogSettings;
            this._customerActivityService = customerActivityService;
            this._eventPublisher = eventPublisher;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._paymentTransactionHistoryModelFactory = paymentTransactionHistoryModelFactory;
            this._paymentTransactionHistoryService = paymentTransactionHistoryService;
            this._workContext = workContext;
            this._workflowMessageService = workflowMessageService;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.PaymentTransactionHistory))
                return AccessDeniedView();

            //prepare model
            var model = _paymentTransactionHistoryModelFactory.PreparePaymentTransactionHistorySearchModel(new PaymentTransactionHistorySearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(PaymentTransactionHistorySearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.PaymentTransactionHistory))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _paymentTransactionHistoryModelFactory.PreparePaymentTransactionHistoryListModel(searchModel);
            return Json(model);
        }

       
        #endregion
    }
}
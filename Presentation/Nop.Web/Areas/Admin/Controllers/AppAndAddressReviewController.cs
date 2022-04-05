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
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class AppAndAddressReviewController : BaseAdminController
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IAppAndAddressReviewModelFactory _appAndAddressReviewModelFactory;
        private readonly IAppAndAddressReviewService _appAndAddressReviewService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
       

        #endregion Fields

        #region Ctor

        public AppAndAddressReviewController(CatalogSettings catalogSettings,
            ICustomerActivityService customerActivityService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IAppAndAddressReviewModelFactory appAndAddressReviewModelFactory,
            IAppAndAddressReviewService appAndAddressService,
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
            this._appAndAddressReviewModelFactory = appAndAddressReviewModelFactory;
            this._appAndAddressReviewService = appAndAddressService;
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAppAndAddressReviews))
                return AccessDeniedView();

            //prepare model
            var model = _appAndAddressReviewModelFactory.PrepareAppAndAddressReviewSearchModel(new AppAddressReviewSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(AppAddressReviewSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAppAndAddressReviews))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _appAndAddressReviewModelFactory.PrepareAppAndAddressReviewListModel(searchModel);
            var test = Json(model);
            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAppAndAddressReviews))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("List");

            if (selectedIds == null)
                return Json(new { Result = true });

            var appAndAddressReviews = _appAndAddressReviewService.GetAppAndAddressReviewByIds(selectedIds.ToArray());
          
            _appAndAddressReviewService.DeleteAppandAddressReviews(appAndAddressReviews);
            return Json(new { Result = true });
        }
        #endregion
    }
}

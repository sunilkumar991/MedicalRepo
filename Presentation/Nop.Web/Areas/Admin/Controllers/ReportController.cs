using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.ExportImport;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Reports;
using System;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ReportController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IReportModelFactory _reportModelFactory;
        private readonly IExportManager _exportManager;

        #endregion

        #region Ctor

        public ReportController(
            IPermissionService permissionService,
            IReportModelFactory reportModelFactory,
            IExportManager exportManager)
        {
            this._permissionService = permissionService;
            this._reportModelFactory = reportModelFactory;
            this._exportManager = exportManager;
        }

        #endregion

        #region Methods

        #region Low stock

        public virtual IActionResult LowStock()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model = _reportModelFactory.PrepareLowStockProductSearchModel(new LowStockProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult LowStockList(LowStockProductSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _reportModelFactory.PrepareLowStockProductListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Bestsellers

        public virtual IActionResult Bestsellers()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //prepare model
            var model = _reportModelFactory.PrepareBestsellerSearchModel(new BestsellerSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult BestsellersList(BestsellerSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _reportModelFactory.PrepareBestsellerListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Never Sold

        public virtual IActionResult NeverSold()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //prepare model
            var model = _reportModelFactory.PrepareNeverSoldSearchModel(new NeverSoldReportSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult NeverSoldList(NeverSoldReportSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _reportModelFactory.PrepareNeverSoldListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Country sales

        public virtual IActionResult CountrySales()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.OrderCountryReport))
                return AccessDeniedView();

            //prepare model
            var model = _reportModelFactory.PrepareCountrySalesSearchModel(new CountryReportSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult CountrySalesList(CountryReportSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.OrderCountryReport))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _reportModelFactory.PrepareCountrySalesListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Customer reports

        public virtual IActionResult Customers()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //prepare model
            var model = _reportModelFactory.PrepareCustomerReportsSearchModel(new CustomerReportsSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ReportBestCustomersByOrderTotalList(BestCustomersReportSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _reportModelFactory.PrepareBestCustomersReportListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult ReportBestCustomersByNumberOfOrdersList(BestCustomersReportSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _reportModelFactory.PrepareBestCustomersReportListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult ReportRegisteredCustomersList(RegisteredCustomersReportSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _reportModelFactory.PrepareRegisteredCustomersReportListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Export / Import Best Sellers 

        public virtual IActionResult ExportXlsx(BestsellerSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            try
            {
                var bytes = _exportManager
                    .ExportBestsellersToXlsx(_reportModelFactory.PrepareForExcelBestsellerListModel(searchModel));

                return File(bytes, MimeTypes.TextXlsx, "Bestsellers.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        #endregion

        #endregion
    }
}

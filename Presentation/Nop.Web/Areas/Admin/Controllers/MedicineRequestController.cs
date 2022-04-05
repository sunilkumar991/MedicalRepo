using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Common;
using Nop.Services.Medicine;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Controllers;
using Microsoft.AspNetCore.Http;
using System;
using Nop.Core.Domain.Medicine;
using Nop.Web.Framework.Mvc;
using Nop.Web.Areas.Admin.Models.Medicine;
using Nop.Web.Framework.Kendoui;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class MedicineRequestController : BaseAdminController
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IMedicineRequestModelFactory _medicineRequestModelFactory;
        private readonly IMedicineRequestService _medicineRequestService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;


        #endregion Fields

        #region Ctor

        public MedicineRequestController(CatalogSettings catalogSettings,
            ICustomerActivityService customerActivityService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IMedicineRequestModelFactory medicineRequestModelFactory,
            IMedicineRequestService medicineRequestService,
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
            this._medicineRequestModelFactory = medicineRequestModelFactory;
            this._medicineRequestService = medicineRequestService;
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
            if (!_permissionService.Authorize(StandardPermissionProvider.MedicineRequest))
                return AccessDeniedView();

            //prepare model
            var model = _medicineRequestModelFactory.PrepareMedicineRequestSearchModel(new MedicineRequestSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(MedicineRequestSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.MedicineRequest))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _medicineRequestModelFactory.PrepareMedicineRequestListModel(searchModel);
            return Json(model);
        }

        #region Edit, delete
        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.MedicineRequest))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _medicineRequestService.GetMedicineRequestById(id);
            if (order == null || order.IsDeleted)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("List");

            //prepare model
            var model = _medicineRequestModelFactory.MedicineRequestItemModel(null, order);
            ViewData["MedicineRequestID"] = id;
            return View(model);
        }

        [Route("MedicineRequset/AddMedicineRequsetItem/{medicineRequestId}")]
        public virtual IActionResult AddMedicineRequsetItem(int medicineRequestId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.MedicineRequest))
                return AccessDeniedView();

            //try to get an Medicine Request with the specified id
            var medicineRequest = _medicineRequestService.GetMedicineRequestById(medicineRequestId);
            if (medicineRequest == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "MedicineRequset", new { id = medicineRequestId });
            MedicineRequestItemModel searchModel = new MedicineRequestItemModel();
            searchModel.Id = medicineRequestId;
            //prepare model
            var model = _medicineRequestModelFactory.PrepareMedicineRequestItemModel(searchModel);
            var medicineRequestCollection = _medicineRequestService.GetMedicineRequestById(medicineRequestId);
            ViewData["ImageUrl"] = medicineRequestCollection.PrescriptionImageUrl;

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult MedicineRequestItemAdd(List<MedicineRequestItemListModel> alldata)
        {
            if (alldata.Count>0)
            {
                if (!_permissionService.Authorize(StandardPermissionProvider.MedicineRequest))
                    return AccessDeniedView();

                var result = alldata.FirstOrDefault(x => x.MedicineRequestID != 0);

                //try to get an Medicine Request with the specified id
                var medicineRequest = _medicineRequestService.GetMedicineRequestById(Convert.ToInt32(result.MedicineRequestID));
                if (medicineRequest == null)
                    return Json(new { Result = false });

                //a vendor does not have access to this functionality
                if (_workContext.CurrentVendor != null)
                    return Json(new { Result = false });

                foreach (var item in alldata)
                {
                    var medicineRequestItem = new MedicineRequestItem
                    {
                        Id = 0,
                        MedicineRequestID = Convert.ToInt32(item.MedicineRequestID),
                        MedicineName = item.MedicineName.Trim(),
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalAmount = (item.UnitPrice * Convert.ToDecimal(item.Quantity)),
                        ProductId = Convert.ToInt32(item.ProductID.Trim()),
                        IsAvailable = item.IsAvailable,
                        CreatedOnUtc = DateTime.UtcNow
                    };
                    medicineRequestItem.ProductId = medicineRequestItem.ProductId == 0 ? null : medicineRequestItem.ProductId;
                    medicineRequestItem.IsAvailable = medicineRequestItem.ProductId == null ? false : medicineRequestItem.IsAvailable;
                    medicineRequest.MedicineRequestItems.Add(medicineRequestItem);
                }
                _medicineRequestService.UpdateMedicineRequest(medicineRequest);
                ViewData["MedicineRequestID"] = medicineRequest.Id;
                return Json(new { Result = true });
            }
           else
            {
                return Json(new { Result = false });
            }
        }

        [HttpPost]
        public virtual IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.MedicineRequest))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("List");

            if (selectedIds == null)
                return Json(new { Result = true });

            var appAndAddressReviews = _medicineRequestService.GetMedicineRequestByIds(selectedIds.ToArray());

            _medicineRequestService.DeleteMedicineRequests(appAndAddressReviews);
            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult MedicineRequestUpdate(MedicineModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.MedicineRequest))
                return AccessDeniedView();

            if (model.RejectedReason != null)
                model.RejectedReason = model.RejectedReason.Trim();

            if (!ModelState.IsValid)
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });

            //try to get a medicineRequest with the specified id
            var medicineRequest = _medicineRequestService.GetMedicineRequestById(model.Id) ?? throw new ArgumentException("No request found with the specified id");

            medicineRequest.RejectedReason = model.RejectedReason;
            if ((RequestStatus)model.RequestStatusId == RequestStatus.Approved && !medicineRequest.MedicineRequestItems.Any())
            {
                medicineRequest.RequestStatusId = (int)RequestStatus.Pending;
            }
            else
            {
                medicineRequest.RequestStatusId = model.RequestStatusId;
            }

            _medicineRequestService.UpdateMedicineRequest(medicineRequest);
            return new NullJsonResult();
        }

        #endregion

        #region MedicineRequestItem Edit,Delete

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnSaveMedicineRequestItem")]
        public virtual IActionResult EditOrderItem(int id, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.MedicineRequest))
                return AccessDeniedView();

            //try to get an order with the specified id
            var mediRequest = _medicineRequestService.GetMedicineRequestById(id);
            if (mediRequest == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "MedicineRequest", new { id });

            //get order item identifier
            var orderItemId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("btnSaveMedicineRequestItem", StringComparison.InvariantCultureIgnoreCase))
                    orderItemId = Convert.ToInt32(formValue.Substring("btnSaveMedicineRequestItem".Length));

            foreach (var obj in mediRequest.MedicineRequestItems)
            {
                if (obj.Id == orderItemId)
                {
                    if (!int.TryParse(form["dropdown_" + orderItemId], out var productId))
                        productId = (int)obj.ProductId;
                    if (!decimal.TryParse(form["PvDiscount" + orderItemId], out var unitPrice))
                        unitPrice = obj.UnitPrice;
                    if (!int.TryParse(form["PvQuantity" + orderItemId], out var quantity))
                        quantity = obj.Quantity;
                    //if (productId == 0)
                    //{
                    //    productId = (int)obj.ProductId;
                    //}
                    var isAvailble = form["IsAvailable" + orderItemId];
                    var medicineName = string.IsNullOrEmpty(form["pvPriceInclTax" + orderItemId].ToString().Trim()) ? obj.MedicineName : form["pvPriceInclTax" + orderItemId].ToString();

                    obj.MedicineName = medicineName;
                    obj.Quantity = quantity;
                    obj.UnitPrice = unitPrice;
                    if (productId == 0)
                    {
                        obj.ProductId = null;
                    }
                    else
                    {
                        obj.ProductId = productId;
                    }
                    obj.TotalAmount = (obj.UnitPrice) * obj.Quantity;
                    if (isAvailble.ToString() == "on")
                    {
                        obj.IsAvailable = true;
                    }
                    else
                    {
                        obj.IsAvailable = false;
                    }
                    obj.IsAvailable = obj.ProductId == null ? false : obj.IsAvailable;
                }
            }

            _medicineRequestService.UpdateMedicineRequest(mediRequest);

            var orderItem = mediRequest.MedicineRequestItems.FirstOrDefault(x => x.Id == orderItemId)
                ?? throw new ArgumentException("No order item found with the specified id");

            LogEditOrder(mediRequest.Id);

            var model = _medicineRequestModelFactory.MedicineRequestItemModel(null, mediRequest);

            if (model.Count == 0)
            {
                return RedirectToAction("List");
            }
            //selected tab
            SaveSelectedTabName(persistForTheNextRequest: false);
            ViewData["MedicineRequestID"] = id;
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnDeleteMedicineRequestItem")]
        public virtual IActionResult DeleteMedicineRequestItem(int id, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.MedicineRequest))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _medicineRequestService.GetMedicineRequestById(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id });

            //get order item identifier
            var orderItemId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("btnDeleteMedicineRequestItem", StringComparison.InvariantCultureIgnoreCase))
                    orderItemId = Convert.ToInt32(formValue.Substring("btnDeleteMedicineRequestItem".Length));

            var orderItem = order.MedicineRequestItems.FirstOrDefault(x => x.Id == orderItemId)
                ?? throw new ArgumentException("No order item found with the specified id");

            //delete item

            _medicineRequestService.DeleteMedicineRequestsItem(orderItem);
            LogEditOrder(order.Id);

            var model = _medicineRequestModelFactory.MedicineRequestItemModel(null, order);

            if (model.Count == 0)
            {
                return RedirectToAction("List");
            }
            //selected tab
            SaveSelectedTabName(persistForTheNextRequest: false);
            ViewData["MedicineRequestID"] = id;
            return View(model);

        }

        protected virtual void LogEditOrder(int orderId)
        {
            var order = _medicineRequestService.GetMedicineRequestById(orderId);

            _customerActivityService.InsertActivity("EditMedicineRequest",
                string.Format(_localizationService.GetResource("ActivityLog.EditOrder"), order.Id), order);
        }

        #endregion

        #endregion
    }
}

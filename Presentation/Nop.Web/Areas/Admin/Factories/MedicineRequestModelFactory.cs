using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Medicine;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Medicine;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;
using System.Collections.Generic;
using Nop.Services.Directory;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Catalog;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the medicine request model factory implementation
    /// </summary>
    public partial class MedicineRequestModelFactory : IMedicineRequestModelFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IReviewTypeService _reviewTypeService;
        private readonly IWorkContext _workContext;
        private readonly IMedicineRequestService _medicineRequestService;
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IPriceFormatter _priceFormatter;

        #endregion

        #region Ctor

        public MedicineRequestModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IReviewTypeService reviewTypeService,
            IWorkContext workContext,
            IMedicineRequestService medicineRequestService, ICustomerService customerService, ICurrencyService currencyService, CurrencySettings currencySettings, IPriceFormatter priceFormatter, IProductService productService)
        {
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._reviewTypeService = reviewTypeService;
            this._workContext = workContext;
            this._medicineRequestService = medicineRequestService;
            this._customerService = customerService;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._priceFormatter = priceFormatter;
            this._productService = productService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare medicine request search model
        /// </summary>
        /// <param name="searchModel">medicine request search model</param>
        /// <returns>>medicine request search model</returns>
        public virtual MedicineRequestSearchModel PrepareMedicineRequestSearchModel(MedicineRequestSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            ////prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }


        /// <summary>
        /// Prepare medicine request search model
        /// </summary>
        /// <param name="searchModel">medicine request search model</param>
        /// <returns>>medicine request search model</returns>
        public virtual MedicineRequestItemModel PrepareMedicineRequestItemModel(MedicineRequestItemModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            var products = _productService.GetAllProduct();


            searchModel.AvailableMedicineProduct.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Medicine.MedicineRequestItem.Fields.MedicineProductList.Select"),
                Value = "0"
            });

            foreach (var item in products)
            {
                searchModel.AvailableMedicineProduct.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = Convert.ToString(item.Id)
                });
            }

            return searchModel;
        }

        /// <summary>
        /// Prepare paged medicine request list model
        /// </summary>
        /// <param name="searchModel">medicine request search model</param>
        /// <returns>medicine request list model</returns>
        public virtual MedicineRequestListModel PrepareMedicineRequestListModel(MedicineRequestSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get medicine requests
            var medicineRequests = _medicineRequestService.GetAllMedicineRequest(

               patientName: searchModel.PatientName,

                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            //prepare list model
            var model = new MedicineRequestListModel
            {
                Data = medicineRequests.Select(medicineReques =>
                {
                    //fill in model values from the entity
                    var medicineRequestModel = new MedicineRequestModel
                    {
                        Id = medicineReques.Id,
                        PatientName = medicineReques.PatientName,
                        Remarks = medicineReques.Remarks,
                        PrescriptionImageUrl = medicineReques.PrescriptionImageUrl,
                        RejectedReason = medicineReques.RejectedReason,
                        MobileNumber = medicineReques.MobileNumber
                    };
                    var reviewType = (RequestStatus)medicineReques.RequestStatusId;

                    if (!string.IsNullOrEmpty(medicineRequestModel.PrescriptionImageUrl))
                    {
                        medicineRequestModel.PrescriptionImageUrl = medicineRequestModel.PrescriptionImageUrl.Split(',')[0];
                    }

                    medicineRequestModel.RequestStatus = reviewType.ToString();
                    if (medicineReques.Customer != null && medicineReques.Customer.BillingAddress != null)
                    {
                        if (!string.IsNullOrEmpty(medicineReques.Customer.BillingAddress.FirstName))
                        {
                            medicineRequestModel.CustomerName = medicineReques.Customer.BillingAddress.FirstName;
                        }
                        else
                        {
                            medicineRequestModel.CustomerName = medicineReques.Customer.Username;
                        }
                    }
                    return medicineRequestModel;
                }),
                Total = medicineRequests.TotalCount
            };

            return model;
        }


        /// <summary>
        /// Prepare MedicineRequest item models
        /// </summary>
        /// <param name="models">List of MedicineRequest item models</param>
        /// <param name="medicineRequestWithId">MedicineRequestitem</param>
        public virtual List<MedicineRequestItemModel> MedicineRequestItemModel(List<MedicineRequestItemModel> models, MedicineRequest medicineRequestWithId)
        {
            if (medicineRequestWithId == null)
                throw new ArgumentNullException(nameof(medicineRequestWithId));

            var medicineRequest = _medicineRequestService.GetMedicineRequestById(medicineRequestWithId.Id);

            List<MedicineRequestItemModel> model = new List<MedicineRequestItemModel>();
            var products = _productService.GetAllProduct();

            foreach (var medicineRequestItem in medicineRequest.MedicineRequestItems)
            {
                //fill in model values from the entity
                var medicineRequestItemModel = new MedicineRequestItemModel
                {
                    Id = medicineRequestItem.Id,
                    MedicineRequestID = medicineRequestItem.MedicineRequestID,
                    MedicineName = medicineRequestItem.MedicineName,
                    Quantity = medicineRequestItem.Quantity,
                    ProductID = Convert.ToString(medicineRequestItem.ProductId),
                    TotalAmount = medicineRequestItem.TotalAmount,
                    UnitPrice = medicineRequestItem.UnitPrice,
                    IsAvailable = medicineRequestItem.IsAvailable
                };
                medicineRequestItemModel.AvailableMedicineProduct.Add(new SelectListItem
                {
                    Text = _localizationService.GetResource("Admin.Medicine.MedicineRequestItem.Fields.MedicineProductList.Select"),
                    Value = "0"
                });
               
                foreach (var item in products)
                {
                    medicineRequestItemModel.AvailableMedicineProduct.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = Convert.ToString(item.Id)
                    });
                }
                model.Add(medicineRequestItemModel);
            }
            
            return model;
        }

        #endregion
    }
}

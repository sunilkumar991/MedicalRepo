using System;
using System.Collections.Generic;
using FluentValidation.Attributes;
using Nop.Web.Areas.Admin.Validators.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial class MedicineRequestModel : BaseNopEntityModel
    {
        #region Ctor

        public MedicineRequestModel()
        {
            Items = new List<MedicineRequestItemModel>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Medicine.MedicineRequest.Fields.PatientName")]
        public string PatientName { get; set; }

        [NopResourceDisplayName("Admin.Medicine.MedicineRequest.Fields.CustomerId")]
        public int CustomerId { get; set; }

        [NopResourceDisplayName("Admin.Medicine.MedicineRequest.Fields.CustomerName")]
        public string CustomerName { get; set; }

        [NopResourceDisplayName("Admin.Medicine.MedicineRequest.Fields.Remarks")]
        public string Remarks { get; set; }

        [NopResourceDisplayName("Admin.Medicine.MedicineRequest.Fields.RejectedReason")]
        public string RejectedReason { get; set; }

        [NopResourceDisplayName("Admin.Medicine.MedicineRequest.Fields.PrescriptionImageUrl")]
        public string PrescriptionImageUrl { get; set; }

        [NopResourceDisplayName("Admin.Medicine.MedicineRequest.Fields.RequestStatusId")]
        public int RequestStatusId { get; set; }

        [NopResourceDisplayName("Admin.Medicine.MedicineRequest.Fields.RequestStatus")]
        public string RequestStatus { get; set; }

        [NopResourceDisplayName("Admin.Medicine.MedicineRequest.Fields.MobileNumber")]
        public string MobileNumber { get; set; }

        public IList<MedicineRequestItemModel> Items { get; set; }

        #endregion
    }
}

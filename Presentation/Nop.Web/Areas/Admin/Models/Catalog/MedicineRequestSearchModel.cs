using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial class MedicineRequestSearchModel : BaseSearchModel
    {
        #region Ctor
        public MedicineRequestSearchModel()
        {
            AvailableRequestStatus = new List<SelectListItem>();
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


        public IList<SelectListItem> AvailableRequestStatus { get; set; }


        #endregion
    }
}

using System;
using System.Collections.Generic;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Validators.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial class MedicineRequestItemModel : BaseNopEntityModel
    {
        #region Ctor

        public MedicineRequestItemModel()
        {
            AvailableMedicineProduct = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Medicine.MedicineRequestItem.Fields.MedicineRequestID")]
        public int MedicineRequestID { get; set; }

        [NopResourceDisplayName("Admin.Medicine.MedicineRequestItem.Fields.MedicineName")]
        public string MedicineName { get; set; }

        [NopResourceDisplayName("Admin.Medicine.MedicineRequestItem.Fields.Quantity")]
        public int Quantity { get; set; }

        [NopResourceDisplayName("Admin.Medicine.MedicineRequestItem.Fields.UnitPrice")]
        public decimal UnitPrice { get; set; }

        [NopResourceDisplayName("Admin.Medicine.MedicineRequestItem.Fields.TotalAmount")]
        public decimal TotalAmount { get; set; }

        [NopResourceDisplayName("Admin.Medicine.MedicineRequestItem.Fields.IsAvailable")]
        public bool IsAvailable { get; set; }

        [NopResourceDisplayName("Admin.Medicine.MedicineRequestItem.Fields.ProductID")]
        public string ProductID { get; set; }

        public IList<MedicineRequestItemModel> Items { get; set; }

        public IList<SelectListItem> AvailableMedicineProduct { get; set; }

        #endregion
    }
}

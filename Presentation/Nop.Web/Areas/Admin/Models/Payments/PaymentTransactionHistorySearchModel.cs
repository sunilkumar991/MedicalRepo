using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial class PaymentTransactionHistorySearchModel : BaseSearchModel
    {
        #region CtorE:\Development\OKZAY-V2\Presentation\Nop.Web\Areas\Admin\Controllers\DownloadController.cs
        public PaymentTransactionHistorySearchModel()
        {
            AvailablePaymentMethod = new List<SelectListItem>();
            AvailableTransactionStatus = new List<SelectListItem>();
        }
        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.PaymentTransactionHistory.Fields.CreatedOnFrom")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnFrom { get; set; }

        [NopResourceDisplayName("Admin.PaymentTransactionHistory.Fields.CreatedOnTo")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnTo { get; set; }

        public int CustomerId { get; set; }

        [NopResourceDisplayName("Admin.PaymentTransactionHistory.Fields.PaymentMethod")]
        public int PaymentMethodId { get; set; }

        public IList<SelectListItem> AvailablePaymentMethod { get; set; }

        [NopResourceDisplayName("Admin.PaymentTransactionHistory.Fields.TransactionStatus")]
        public int TransactionStatusId { get; set; }

        public IList<SelectListItem> AvailableTransactionStatus { get; set; }

        [NopResourceDisplayName("Admin.PaymentTransactionHistory.Fields.TransactionId")]
        public string TransactionId { get; set; }

        public string TransactionStatus { get; set; }

        public decimal TransactionAmount { get; set; }

        public int IsTransactionSuccess { get; set; }
        
        [UIHint("DateNullable")]
        public DateTime? CreatedOnUtc { get; set; }
        
        #endregion
    }
}

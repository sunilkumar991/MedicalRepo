using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial class PaymentTransactionHistoryModel : BaseNopEntityModel
    {
        #region Ctor

        public PaymentTransactionHistoryModel()
        {

        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.PaymentTransactionHistory.Fields.CustomerName")]
        public string CustomerName { get; set; }

        [NopResourceDisplayName("Admin.PaymentTransactionHistory.Fields.PaymentMethod")]
        public string PaymentMethod { get; set; }

        [NopResourceDisplayName("Admin.PaymentTransactionHistory.Fields.TransactionId")]
        public string TransactionId { get; set; }

        [NopResourceDisplayName("Admin.PaymentTransactionHistory.Fields.TransactionMessage")]
        public string TransactionMessage { get; set; }

        [NopResourceDisplayName("Admin.PaymentTransactionHistory.Fields.TransactionAmount")]
        public string TransactionAmount { get; set; }

        [NopResourceDisplayName("Admin.PaymentTransactionHistory.Fields.IsTransactionSuccess")]
        public string IsTransactionSuccess { get; set; }

        [NopResourceDisplayName("Admin.PaymentTransactionHistory.Fields.IssueStatus")]
        public string IssueStatus { get; set; }

        [NopResourceDisplayName("Admin.PaymentTransactionHistory.Fields.UpdatedBy")]
        public string UpdatedBy { get; set; }

        [NopResourceDisplayName("Admin.PaymentTransactionHistory.Fields.Comments")]
        public string Comments { get; set; }

        [NopResourceDisplayName("Admin.PaymentTransactionHistory.Fields.CreatedOnUtc")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnUtc { get; set; }

        [NopResourceDisplayName("Admin.PaymentTransactionHistory.Fields.UpdatedOnUtc")]
        [UIHint("DateNullable")]
        public DateTime? UpdatedOnUtc { get; set; }

        #endregion
    }
}

using Nop.Core.Domain.Customers;
using System;

namespace Nop.Core.Domain.Payments
{
    /// <summary>
    /// Created By : Alexandar Rajavel on 05-Nov-2018
    /// Represents a payment transaction history
    /// </summary>
    public class PaymentTransactionHistory : BaseEntity
    {
        #region Properties
        public int CustomerId { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }

        //public string TransactionStatus { get; set; }
        public string TransactionDescription { get; set; }

        public decimal TransactionAmount { get; set; }

        //public bool IsTransactionSuccess { get; set; }
        public int TransactionStatus { get; set; }

        public int? IssueStatus { get; set; }
        public string Comments { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsNew { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        #endregion

        #region Navigation properties
        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public virtual Customer Customer { get; set; }
        #endregion
    }
}

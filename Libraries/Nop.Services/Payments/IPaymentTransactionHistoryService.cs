using Nop.Core;
using Nop.Core.Domain.Payments;
using System;
using System.Collections.Generic;

namespace Nop.Services.Payments
{
    /// <summary>
    /// Created By : Alexandar Rajavel on 05-Nov-2018
    /// Payment transaction histrory service interface
    /// </summary>
    public interface IPaymentTransactionHistoryService
    {
        /// <summary>
        ///Get All Payment Transaction History  Created By Sunil Kumar on 02-Apr-2019
        /// </summary>
        /// <param name="customerId">Customer identifier (who wrote a review); 0 to load all records</param>
        /// <param name="TransactionId">TransactionId</param> 
        /// <param name="TransactionStatus">TransactionStatus</param>
        /// <param name="PaymentMethod">PaymentMethod</param>
        /// <param name="fromUtc">fromUtc</param>
        /// <param name="toUtc">toUtc</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Payment Transaction History</returns>
        IPagedList<PaymentTransactionHistory> GetAllPaymentTransactionHistory(
          int CustomerId = 0, string TransactionId = null, int TransactionStatus = 0,
           string PaymentMethod = null, DateTime? fromUtc = null, DateTime? toUtc = null,
           int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Created By Alexandar Rajavel on 01-Apr-2019
        /// Get paymentTransactionHistory by id
        /// </summary>
        PaymentTransactionHistory GetTransactionHistoryById(int transactionId);

        /// <summary>
        /// Created By Alexandar Rajavel on 01-Apr-2019
        /// Get failed TransactionHistory details
        /// </summary>
        IList<PaymentTransactionHistory> GetTransactionFailedCustoemrList();

        /// <summary>
        /// Inserts paymentTransactionHistory
        /// </summary>
        /// <param name="paymentTransactionHistory">paymentTransactionHistory</param>
        void InsertTransactionHistory(PaymentTransactionHistory paymentTransactionHistory);

        /// <summary>
        /// Updates the paymentTransactionHistory
        /// </summary>
        /// <param name="paymentTransactionHistory">paymentTransactionHistory</param>
        void UpdateTransactionHistory(PaymentTransactionHistory paymentTransactionHistory);
    }
}

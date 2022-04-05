using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Payments;
using Nop.Services.Events;

namespace Nop.Services.Payments
{
    /// <summary>
    /// Payment transaction history service
    /// Created By : Alexandar Rajavel on 05-Nov-2018
    /// </summary>
    public class PaymentTransactionHistoryService : IPaymentTransactionHistoryService
    {
        private readonly IRepository<PaymentTransactionHistory> _paymentTransactionHistory;
        private readonly IEventPublisher _eventPublisher;
        private readonly CatalogSettings _catalogSettings;

        public PaymentTransactionHistoryService(IRepository<PaymentTransactionHistory> paymentTransactionHistory, IEventPublisher eventPublisher, CatalogSettings catalogSettings)
        {
            _paymentTransactionHistory = paymentTransactionHistory;
            _eventPublisher = eventPublisher;
            _catalogSettings = catalogSettings;
        }

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
        public virtual IPagedList<PaymentTransactionHistory> GetAllPaymentTransactionHistory(
           int CustomerId = 0, string TransactionId = null, int TransactionStatus = 0,
            string PaymentMethod = null, DateTime? fromUtc = null, DateTime? toUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _paymentTransactionHistory.Table;

            if (!string.IsNullOrEmpty(PaymentMethod) && (PaymentMethod != "All"))
            {
                if (PaymentMethod == "TWOCTWOP")
                {
                    PaymentMethod = "2C2P";
                }
                query = query.Where(pr => pr.PaymentMethod.Contains(PaymentMethod));
            }
            if (!string.IsNullOrEmpty(TransactionId))
                query = query.Where(pr => pr.TransactionId.Contains(TransactionId));
            if (TransactionStatus != 0)
                query = query.Where(pr => pr.TransactionStatus == TransactionStatus);
            if (fromUtc.HasValue)
                query = query.Where(pr => fromUtc.Value <= pr.CreatedOnUtc);
            if (toUtc.HasValue)
                query = query.Where(pr => toUtc.Value >= pr.CreatedOnUtc);

            query = _catalogSettings.PaymentTransactionHistorySortByCreatedDateAscending
                ? query.OrderBy(pr => pr.CreatedOnUtc).ThenBy(pr => pr.Id)
                : query.OrderByDescending(pr => pr.CreatedOnUtc).ThenBy(pr => pr.Id);

            var appAndAddressReviews = new PagedList<PaymentTransactionHistory>(query, pageIndex, pageSize);

            return appAndAddressReviews;
        }


        /// <summary>
        /// Created By Alexandar Rajavel on 01-Apr-2019
        /// </summary>
        public virtual IList<PaymentTransactionHistory> GetTransactionFailedCustoemrList()
        {
            var query = from c in _paymentTransactionHistory.Table
                        orderby c.Id
                        where c.IssueStatus == (int)IssueStatusType.Open || c.IssueStatus == (int)IssueStatusType.Inprogress
                        select c;
            return query.ToList();
        }

        /// <summary>
        /// Created By Alexandar Rajavel on 01-Apr-2019
        /// </summary>
        public virtual PaymentTransactionHistory GetTransactionHistoryById(int transactionId)
        {
            if (transactionId == 0)
                return null;
            return _paymentTransactionHistory.GetById(transactionId);
        }

        /// <summary>
        /// Inserts an paymentTransactionHistory
        /// </summary>
        /// <param name="paymentTransactionHistory">paymentTransactionHistory</param>
        public virtual void InsertTransactionHistory(PaymentTransactionHistory paymentTransactionHistory)
        {
            if (paymentTransactionHistory == null)
                throw new ArgumentNullException(nameof(paymentTransactionHistory));

            paymentTransactionHistory.CreatedOnUtc = DateTime.Now;
            paymentTransactionHistory.UpdatedOnUtc = DateTime.Now;

            _paymentTransactionHistory.Insert(paymentTransactionHistory);

            //event notification
            _eventPublisher.EntityInserted(paymentTransactionHistory);
        }

        /// <summary>
        /// Updates the paymentTransactionHistory
        /// </summary>
        /// <param name="paymentTransactionHistory">paymentTransactionHistory</param>
        public virtual void UpdateTransactionHistory(PaymentTransactionHistory paymentTransactionHistory)
        {
            if (paymentTransactionHistory == null)
                throw new ArgumentNullException(nameof(paymentTransactionHistory));

            _paymentTransactionHistory.Update(paymentTransactionHistory);

            //event notification
            _eventPublisher.EntityUpdated(paymentTransactionHistory);
        }
    }
}

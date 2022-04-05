using Nop.Core.Domain.Catalog;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Nop.Web.Areas.Admin.Factories
{
    public partial interface IPaymentTransactionHistoryModelFactory
    {

        /// <summary>
        /// Prepare Payment Transaction History SearchModel
        /// </summary>
        /// <param name="searchModel">Payment Transaction History model</param>
        /// <returns>Payment Transaction History search model</returns>
        PaymentTransactionHistorySearchModel PreparePaymentTransactionHistorySearchModel(PaymentTransactionHistorySearchModel searchModel);

        /// <summary>
        /// Prepare Payment Transaction History list model
        /// </summary>
        /// <param name="searchModel">Payment Transaction History search model</param>
        /// <returns>Payment Transaction History list model</returns>
        PaymentTransactionHistoryListModel PreparePaymentTransactionHistoryListModel(PaymentTransactionHistorySearchModel searchModel);
    }
}

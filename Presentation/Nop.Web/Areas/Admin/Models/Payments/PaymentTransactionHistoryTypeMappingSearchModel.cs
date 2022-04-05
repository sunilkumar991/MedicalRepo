using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Payments
{
    /// <summary>
    /// Represents a Payment Transaction History Type Mapping Search Model
    /// </summary>
    public class PaymentTransactionHistoryTypeMappingSearchModel : BaseSearchModel
    {
        #region Properties

        public int PaymentTransactionHistoryId { get; set; }

        #endregion
    }
}

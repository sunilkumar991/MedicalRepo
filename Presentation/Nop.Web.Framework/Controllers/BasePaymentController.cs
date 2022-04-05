
namespace Nop.Web.Framework.Controllers
{
    /// <summary>
    /// Base controller for payment plugins
    /// </summary>
    public abstract class BasePaymentController : BasePluginController
    {
        public const string PAYMENT_STATUS_SUCCESS = "000";
        public const string PAYMENT_STATUS_PENDING = "001";
        public const string COMPLETED = "completed";
        public const string PENDING = "pending";
    }
}

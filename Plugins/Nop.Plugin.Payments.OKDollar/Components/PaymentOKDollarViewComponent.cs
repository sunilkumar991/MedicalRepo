using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.OKDollar.Components
{
    [ViewComponent(Name = "PaymentOKDollar")]
    public class PaymentOKDollarViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Payments.OKDollar/Views/PaymentInfo.cshtml");
        }

    }
}

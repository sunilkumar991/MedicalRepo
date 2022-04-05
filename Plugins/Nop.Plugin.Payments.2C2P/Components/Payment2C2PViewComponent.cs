using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments._2C2P.Components
{
    // Created by Alexandar Rajavel on 14-Mar-2019
    [ViewComponent(Name = "Payment2C2P")]
    public class Payment2C2PViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Payments.2C2P/Views/PaymentInfo.cshtml");
        }
    }
}

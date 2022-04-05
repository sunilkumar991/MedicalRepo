using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.OKDollar
{
    public class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            //PDT
            routeBuilder.MapRoute("Plugin.Payments.OKDollar.PDTHandler", "Plugins/PaymentOKDollar/PDTHandler",
                 new { controller = "PaymentOKDollar", action = "PDTHandler" });

            //IPN
            routeBuilder.MapRoute("Plugin.Payments.OKDollar.IPNHandler", "Plugins/PaymentOKDollar/IPNHandler",
                 new { controller = "PaymentOKDollar", action = "IPNHandler" });

            //Cancel
            routeBuilder.MapRoute("Plugin.Payments.OKDollar.CancelOrder", "Plugins/PaymentOKDollar/CancelOrder",
                 new { controller = "PaymentOKDollar", action = "CancelOrder" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority
        {
            get { return -1; }
        }
    }
}

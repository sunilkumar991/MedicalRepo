using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments._2C2P
{
    // Created by Alexandar Rajavel on 14-Mar-2019
    public partial class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            //PDT
            routeBuilder.MapRoute("Plugin.Payments.2C2P.PDTHandler", "Plugins/Payment2C2P/PDTHandler",
                 new { controller = "Payment2C2P", action = "PDTHandler" });

            //2C2P Web Backend response
            routeBuilder.MapRoute("Plugin.Payments.2C2P.BackendResponseFrom2C2P", "Plugins/Payment2C2P/BackendResponseFrom2C2P",
                 new { controller = "Payment2C2P", action = "BackendResponseFrom2C2P" });

            //2C2P SDK Backend response
            routeBuilder.MapRoute("Plugin.Payments.2C2P.SDKBackendResponseFrom2C2P", "Plugins/Payment2C2P/SDKBackendResponseFrom2C2P",
                 new { controller = "Payment2C2P", action = "SDKBackendResponseFrom2C2P" });

            //IPN
            routeBuilder.MapRoute("Plugin.Payments.2C2P.IPNHandler", "Plugins/Payment2C2P/IPNHandler",
                 new { controller = "Payment2C2P", action = "IPNHandler" });

            //Cancel
            routeBuilder.MapRoute("Plugin.Payments.2C2P.CancelOrder", "Plugins/Payment2C2P/CancelOrder",
                 new { controller = "Payment2C2P", action = "CancelOrder" });
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

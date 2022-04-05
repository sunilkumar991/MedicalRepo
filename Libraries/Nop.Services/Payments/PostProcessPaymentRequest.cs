using Nop.Core.Domain.Orders;
using System.Collections.Generic;

namespace Nop.Services.Payments
{
    /// <summary>
    /// Represents a PostProcessPaymentRequest
    /// </summary>
    public partial class PostProcessPaymentRequest
    {
        /// <summary>
        /// Gets or sets an order. Used when order is already saved (payment gateways that redirect a customer to a third-party URL)
        /// </summary>
        public Order Order { get; set; }

        /// <summary>
        /// Gets or sets an order. Used when order is already saved (payment gateways that redirect a customer to a third-party URL)
        /// Added by Ankur for EC-151
        /// </summary>
        public List<Order> Orders { get; set; }

        /// <summary>
        /// Gets or sets an Product List String from the Shopping Cart(payment gateways that redirect a customer to a third-party URL)
        /// </summary>
        public string ProductListString { get; set; }
    }
}

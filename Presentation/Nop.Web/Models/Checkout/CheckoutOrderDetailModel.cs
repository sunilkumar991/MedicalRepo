using Nop.Web.Framework.Models;
using System;

namespace Nop.Web.Models.Checkout
{
    public class CheckoutOrderDetailJsonModel 
    {
       public int OrderId { get; set; }
       public string CustomOrderNumber { get; set; }
       public string ExpectedDeliveryDate { get; set; }
    }


}

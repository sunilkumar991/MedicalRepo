using Nop.Web.Framework.Models;
using System.Collections.Generic;
using System;



namespace Nop.Web.Models.Checkout
{
    public partial class CheckoutCompletedModel : BaseNopModel
    {
        //Changed By Ankur For EC-151
        //public int OrderId { get; set; }
        //public List<int> OrderIds { get; set; }
        //public string CustomOrderNumber { get; set; }
        //public List<string> CustomOrderNumbers { get; set; }
        public bool OnePageCheckoutEnabled { get; set; }
        public int OrderGroupNumber { get; set; }
        //public List<DateTime?> ExpectedDeliveryDays { get; set; }
        public List<CheckoutOrderDetailJsonModel> Orders { get; set; }
        public bool IsScuccess { get; set; }
        public string ErrorMessage { get; set; }

    }
}
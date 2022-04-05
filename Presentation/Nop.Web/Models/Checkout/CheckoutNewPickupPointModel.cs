using System.Collections.Generic;

namespace Nop.Web.Models.Checkout
{
    // Created by : Alexandar Rajavel on 29-Oct-2018
    public class CheckoutNewPickupPointModel
    {
        public CheckoutNewPickupPointModel()
        {
            PickupPointMethods = new List<NewPickupPointModel>();
        }

        public IList<NewPickupPointModel> PickupPointMethods { get; set; }
        public bool IsDeliveryAllowed { get; set; }
    }
    #region Nested classes

    public partial class NewPickupPointModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Fee { get; set; }
        public bool Selected { get; set; }
        public string LogoUrl { get; set; }
        public string OpeningHours { get; set; }
    }

    #endregion
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Checkout
{
    public class CheckoutSavePaymentMethodQueryModel
    {
        public string PaymentMethod { get; set; }
        public int VersionCode { get; set; }

        public string SubPaymetnType { get; set; }
    }
}

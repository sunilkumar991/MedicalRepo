using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.OKDollar
{
    public class OKDollarPaymentSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to use sandbox (testing environment)
        /// </summary>
        public bool UseSandbox { get; set; }

        public string ApiKey { get; set; }

        public string Destination { get; set; }

        public string MerchantName { get; set; }

        //public string Source { get; set; }
        //public decimal Amount { get; set; }
        //public string RefNumber { get; set; }

        public string SecretKey { get; set; }

        public string IVValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to pass info about purchased items to PayPal
        /// </summary>
        public bool PassProductNamesAndTotals { get; set; }

        /// <summary>
        /// Gets or sets an additional fee
        /// </summary>
        public decimal AdditionalFee { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }

        /// <summary>
        /// Gets or sets a url for redirect and post data for OK$ payment gateway
        /// </summary>
        public string RedirectAndPostDataURL { get; set; }
    }
}

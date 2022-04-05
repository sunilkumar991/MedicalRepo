using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Configuration;

namespace Nop.Plugin.Payments._2C2P
{
    // Created by Alexandar Rajavel on 14-Mar-2019
    public class _2C2PPaymentSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to use sandbox (testing environment)
        /// </summary>
        public bool UseSandbox { get; set; }

        public string Version { get; set; }

        public string MerchantId { get; set; }

        public string SecretKey { get; set; }

        public string ResultUrl { get; set; }

        public string Currency { get; set; }

        public string PrivatePfxFilePath { get; set; }

        public string PrivatePfxFilePassword { get; set; }

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
        /// Gets or sets a url for redirect and post data for 2C2P payment gateway
        /// </summary>
        public string RedirectAndPostDataURL { get; set; }
    }
}

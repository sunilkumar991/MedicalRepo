﻿using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.COD
{
   public class CODPaymentSettings : ISettings
    {
        public decimal AdditionalFee { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }

        public string DescriptionText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether shippable products are required in order to display this payment method during checkout
        /// </summary>
        public bool ShippableProductRequired { get; set; }

        public bool SkipPaymentInfo { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;
using BS.Plugin.NopStation.MobileWebApi.Extensions.Authorize.Net;

namespace BS.Plugin.NopStation.MobileWebApi.PluginSettings
{
    public class AuthorizeNetPaymentSettings : ISettings
    {

        public bool UseSandbox { get; set; }
        public TransactMode TransactMode { get; set; }
        public string TransactionKey { get; set; }
        public string LoginId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }
        /// <summary>
        /// Additional fee
        /// </summary>
        public decimal AdditionalFee { get; set; }

    }
}

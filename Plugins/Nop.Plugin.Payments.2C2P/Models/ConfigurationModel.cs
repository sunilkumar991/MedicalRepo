using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments._2C2P.Models
{
    // Created by Alexandar Rajavel on 14-Mar-2019
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.2C2P.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.2C2P.Fields.Version")]
        public string Version { get; set; }
        public bool Version_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.2C2P.Fields.MerchantId")]
        public string MerchantId { get; set; }
        public bool MerchantId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.2C2P.Fields.SecretKey")]
        public string SecretKey { get; set; }
        public bool SecretKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.2C2P.Fields.ResultUrl")]
        public string ResultUrl { get; set; }
        public bool ResultUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.2C2P.Fields.Currency")]
        public string Currency { get; set; }
        public bool Currency_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.2C2P.Fields.PrivatePfxFilePath")]
        public string PrivatePfxFilePath { get; set; }
        public bool PrivatePfxFilePath_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.2C2P.Fields.PrivatePfxFilePassword")]
        public string PrivatePfxFilePassword { get; set; }
        public bool PrivatePfxFilePassword_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.2C2P.Fields.PassProductNamesAndTotals")]
        public bool PassProductNamesAndTotals { get; set; }
        public bool PassProductNamesAndTotals_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.2C2P.Fields.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.2C2P.Fields.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.2C2P.Fields.RedirectAndPostDataURL")]
        public string RedirectAndPostDataURL { get; set; }
        public bool RedirectAndPostDataURL_OverrideForStore { get; set; }

    }
}

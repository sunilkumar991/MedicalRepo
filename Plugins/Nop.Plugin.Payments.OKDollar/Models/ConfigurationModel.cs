using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.OKDollar.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.OKDollar.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.OKDollar.Fields.ApiKey")]
        public string ApiKey { get; set; }
        public bool ApiKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.OKDollar.Fields.Destination")]
        public string Destination { get; set; }
        public bool Destination_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.OKDollar.Fields.MerchantName")]
        public string MerchantName { get; set; }
        public bool MerchantName_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.OKDollar.Fields.SecretKey")]
        public string SecretKey { get; set; }
        public bool SecretKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.OKDollar.Fields.IVValue")]
        public string IVValue { get; set; }
        public bool IVValue_OverrideForStore { get; set; }

        //[NopResourceDisplayName("Plugins.Payments.OKDollar.Fields.RefNumber")]
        //public string RefNumber { get; set; }
        //public bool RefNumber_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.OKDollar.Fields.PassProductNamesAndTotals")]
        public bool PassProductNamesAndTotals { get; set; }
        public bool PassProductNamesAndTotals_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.OKDollar.Fields.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.OKDollar.Fields.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.OKDollar.Fields.RedirectAndPostDataURL")]
        public string RedirectAndPostDataURL { get; set; }
        public bool RedirectAndPostDataURL_OverrideForStore { get; set; }
    }
}

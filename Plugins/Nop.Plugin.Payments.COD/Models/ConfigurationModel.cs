using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
namespace Nop.Plugin.Payments.COD.Models
{
    public class ConfigurationModel : BaseNopModel, ILocalizedModel<ConfigurationModel.ConfigurationLocalizedModel>
    {
        public ConfigurationModel()
        {
            Locales = new List<ConfigurationLocalizedModel>();
        }

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payment.COD.AdditionalFee")]
        public decimal AdditionalFee { get; set; }

        public bool AdditionalFee_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payment.COD.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }

        public bool AdditionalFeePercentage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payment.COD.DescriptionText")]
        public string DescriptionText { get; set; }

        public bool DescriptionText_OverrideForStore { get; set; }

        public IList<ConfigurationLocalizedModel> Locales { get; set; }

        [NopResourceDisplayName("Plugins.Payment.COD.ShippableProductRequired")]
        public bool ShippableProductRequired { get; set; }

        public bool ShippableProductRequired_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payment.COD.SkipPaymentInfo")]
        public bool SkipPaymentInfo { get; set; }

        public bool SkipPaymentInfo_OverrideForStore { get; set; }

        #region Nested class

        public partial class ConfigurationLocalizedModel : ILocalizedLocaleModel
        {
            [NopResourceDisplayName("Plugins.Payment.COD.DescriptionText")]
            public string DescriptionText { get; set; }

            public int LanguageId { get; set; }
        }

        #endregion
    }
}
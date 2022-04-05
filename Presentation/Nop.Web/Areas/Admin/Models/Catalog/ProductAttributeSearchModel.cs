using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product attribute search model
    /// </summary>
    public partial class ProductAttributeSearchModel : BaseSearchModel
    {
        [NopResourceDisplayName("Admin.Catalog.Categories.List.SearchProductAttribute")]
        public string SearchProductAttribute { get; set; }
    }
}
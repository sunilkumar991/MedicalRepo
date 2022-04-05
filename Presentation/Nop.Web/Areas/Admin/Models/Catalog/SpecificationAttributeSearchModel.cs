using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a specification attribute search model
    /// </summary>
    public partial class SpecificationAttributeSearchModel : BaseSearchModel
    {
        [NopResourceDisplayName("Admin.Catalog.Categories.List.SearchSpecificationAttribute")]
        public string SearchSpecificationAttribute { get; set; }
    }
}
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Catalog
{
    public class ProductOverViewWithFilterModelApi : GeneralResponseModel<IList<ProductOverViewModelApi>>
    {
        public PriceRange PriceRange { get; set; }
        public dynamic AvailableSortOptions { get; set; }
        public List<SpecificationFilterItem> NotFilteredItems { get; set; }
    }

    public partial class SpecificationFilterItem
    {
        public string SpecificationAttributeName { get; set; }
        public string SpecificationAttributeOptionName { get; set; }
        public int FilterId { get; set; }
        public int ProductId { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Catalog
{
    public class SearchProductResponseModel : BaseResponse
    {
        public SearchProductResponseModel()
        {
            Products = new List<ProductOverViewModelApi>();
            PriceRange = new PriceRange();
            AvailableSortOptions = new List<SelectListItem>();
            NotFilteredItems = new List<SpecificationFilterItem>();
        }

        public PriceRange PriceRange { get; set; }
        public IList<ProductOverViewModelApi> Products { get; set; }
        public int TotalPages { get; set; }
        public IList<SelectListItem> AvailableSortOptions { get; set; }
        public IList<SpecificationFilterItem> NotFilteredItems { get; set; }
    }
}

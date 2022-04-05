﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Catalog
{
    public class CategoryNavigationModelApi : CategoryBaseModelApi
    {
        public CategoryNavigationModelApi()
        {
            Children = new List<CategoryNavigationModelApi>();
        }
        public int ParentCategoryId { get; set; }
        public int DisplayOrder { get; set; }
        public string IconPath { get; set; }
        public string Extension { get; set; }

        public IList<CategoryNavigationModelApi> Children { get; set; }
    }
}

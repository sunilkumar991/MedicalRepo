using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using System;
using System.Collections.Generic;
using System.Text;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Vendor
{
    public class AllCategoryResponseModel : BaseResponse
    {
        public IList<CategoryNavigationModelApi> Data { get; set; }
        public int Count { get; set; }
    
    }
}

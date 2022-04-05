using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.ShoppingCart
{
    public class DiscountsResponseModel : BaseResponse
    {
        public DiscountsResponseModel()
        {
            DiscountModels = new List<DiscountModel>();
        }

        public IList<DiscountModel> DiscountModels { get; set; }
    }

    #region Nested Class
    public partial class DiscountModel : BaseNopEntityModel
    {
        public string CouponCode { get; set; } 
    }
    #endregion
}

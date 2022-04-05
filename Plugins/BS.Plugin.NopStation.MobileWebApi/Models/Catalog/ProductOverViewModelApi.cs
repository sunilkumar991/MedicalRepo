using BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel;
using BS.Plugin.NopStation.MobileWebApi.Models.Product;
using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using PictureModel = BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel.PictureModel;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Catalog
{
    public class ProductOverViewModelApi : BaseNopEntityModel
    {
        public ProductOverViewModelApi()
        {
            ProductPrice = new ProductDetailsModelApi.ProductPriceModel();
            DefaultPictureModel = new PictureModel();
            ReviewOverviewModel = new ProductReviewOverviewModel();
            //ProductAttributes = new List<ProductAttributesModel>();
        }

        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Sku { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedOn { get; set; }

        public bool InStock { get; set; }
        //price
        public ProductDetailsModelApi.ProductPriceModel ProductPrice { get; set; }
        //picture
        public PictureModel DefaultPictureModel { get; set; }
        //price
        public ProductReviewOverviewModel ReviewOverviewModel { get; set; }

        //productattributes
        //public IList<ProductAttributesModel> ProductAttributes { get; set; }

        //IsWishList adeed by Sunil Kumar at 21-12-18
        public bool IsWishList { get; set; }
        //IsWishList adeed by Sunil Kumar at 27-02-19
        public bool DisableWishlistButton { get; set; }

        //MarkAsNew added by Sunil Kumar at 01-04-19
        public bool MarkAsNew { get; set; }

        #region Nested Classes

        //public partial class ProductPriceModel 
        //{
        //    public string OldPrice { get; set; }
        //    public string Price {get;set;}
        //}

        public partial class ProductReviewOverviewModel
        {
            public int ProductId { get; set; }
            public double RatingSum { get; set; }
            public bool AllowCustomerReviews { get; set; }
            public int TotalReviews { get; set; }
            //Added by Sunil Kumar 18-2-19
            public string RatingUrlFromGSMArena { get; set; }
        }
        //Added by Sunil Kumar 05-03-19
        //public partial class ProductAttributesModel
        //{
        //    public string SpecificationAttributeName { get; set; }
        //    public string SpecificationAttributeOptionName { get; set; }
        //    public int FilterId { get; set; }
        //    public int ProductId { get; set; }
        //}
        #endregion
    }
}

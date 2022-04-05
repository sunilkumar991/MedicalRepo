using System;
using FluentValidation.Attributes;
using Nop.Web.Areas.Admin.Validators.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial class AppAndAddressReviewModel : BaseNopEntityModel
    {
        #region Ctor

        public AppAndAddressReviewModel()
        {
           
        }

        #endregion

        #region Properties

       
        //[NopResourceDisplayName("Admin.Catalog.ProductReviews.Fields.Customer")]
        //public int CustomerId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductReviews.Fields.Customer")]
        public string CustomerName { get; set; }

        //[NopResourceDisplayName("Admin.Catalog.ProductReviews.Fields.ReviewTypeId")]
        //public int ReviewTypeId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductReviews.Fields.ReviewType")]
        public string ReviewType { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductReviews.Fields.OrderNo")]
        public string OrderNo { get; set; }


        [NopResourceDisplayName("Admin.Catalog.ProductReviews.Fields.AddressId")]
        public int AddressId { get; set; }


        [NopResourceDisplayName("Admin.Catalog.ProductReviews.Fields.ReviewText")]
        public string ReviewText { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductReviews.Fields.Rating")]
        public int Rating { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductReviews.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

       
        #endregion
    }
}

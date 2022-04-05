using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial class AppAddressReviewSearchModel : BaseSearchModel
    {
        #region Ctor
        public AppAddressReviewSearchModel()
        {
            AvailableReviewType = new List<SelectListItem>();
        }
        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Catalog.ProductReviews.List.CreatedOnFrom")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnFrom { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductReviews.List.CreatedOnTo")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnTo { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductReviews.List.SearchText")]
        public string ReviewText { get; set; }

        [NopResourceDisplayName("Admin.Catalog.AppAndAddressReviews.Fields.CustomerId")]
        public int CustomerId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.AppAndAddressReviews.Fields.Rating")]
        public int Rating { get; set; }

        [NopResourceDisplayName("Admin.Catalog.AppAndAddressReviews.Fields.OrderNo")]
        public int OrderNo { get; set; }

        [NopResourceDisplayName("Admin.Catalog.AppAndAddressReviews.Fields.ReviewType")]
        public int ReviewTypeId { get; set; }

        public IList<SelectListItem> AvailableReviewType { get; set; }


        #endregion
    }
}

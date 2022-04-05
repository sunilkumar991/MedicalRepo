using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Review;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.DeliveryBoy;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the app and address review model factory implementation
    /// </summary>
    public partial class AppAndAddressReviewModelFactory:IAppAndAddressReviewModelFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IReviewTypeService _reviewTypeService;
        private readonly IWorkContext _workContext;
        private readonly IAppAndAddressReviewService _appAndAddressReviewService;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public AppAndAddressReviewModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IReviewTypeService reviewTypeService,
            IWorkContext workContext,
            IAppAndAddressReviewService appAndAddressReviewService, ICustomerService customerService)
        {
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._reviewTypeService = reviewTypeService;
            this._workContext = workContext;
            this._appAndAddressReviewService = appAndAddressReviewService;
            this._customerService = customerService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare App and Address review search model
        /// </summary>
        /// <param name="searchModel">App And Address review search model</param>
        /// <returns>>App And Address review search model</returns>
        public virtual AppAddressReviewSearchModel PrepareAppAndAddressReviewSearchModel(AppAddressReviewSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare "approved" property(0 - all; 1 - app only; 2 - address only)
            searchModel.AvailableReviewType.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Catalog.AppAndAddressReviews.list.SearchAppAndAddressReviewType.All"),
                Value = "0"
            });
            searchModel.AvailableReviewType.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Catalog.AppAndAddressReviews.list.SearchAppAndAddressReviewType.AppReviewonly"),
                Value = "1"
            });
            searchModel.AvailableReviewType.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Catalog.AppAndAddressReviews.list.SearchAppAndAddressReviewType.AddressReviewonly"),
                Value = "2"
            });

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }


        /// <summary>
        /// Prepare paged App And Address review list model
        /// </summary>
        /// <param name="searchModel">App and Address review search model</param>
        /// <returns>App And Address review list model</returns>
        public virtual AppAndAddressReviewListModel PrepareAppAndAddressReviewListModel(AppAddressReviewSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter reviews
            var createdOnFromValue = !searchModel.CreatedOnFrom.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone);
            var createdToFromValue = !searchModel.CreatedOnTo.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);
          
            //get App And Address reviews
            var appAndAddressReviews = _appAndAddressReviewService.GetAllAppAndAddressReviews(
                fromUtc: createdOnFromValue,
                toUtc: createdToFromValue,
                reviewTypeId: searchModel.ReviewTypeId,
                reviewtext: searchModel.ReviewText,
                pageIndex: searchModel.Page - 1, 
                pageSize: searchModel.PageSize);

            //prepare list model
            var model = new AppAndAddressReviewListModel
            {
                Data = appAndAddressReviews.Select(appAndAddressReview =>
                {
                    //fill in model values from the entity
                    var appAndAddressReviewModel = new AppAndAddressReviewModel
                    {
                        Id = appAndAddressReview.Id,
                        OrderNo = appAndAddressReview.OrderNo,
                        AddressId= appAndAddressReview.AddressId,
                        ReviewText = appAndAddressReview.ReviewText,
                        Rating = appAndAddressReview.Rating,
                        CreatedOn= appAndAddressReview.CreatedOnUtc
                    };
                    var reviewType = (ReviewTypeEnum)appAndAddressReview.ReviewTypeId;
                    appAndAddressReviewModel.ReviewType = reviewType.ToString();

                    //try to get a customer with the specified id
                    var customer = _customerService.GetCustomerById(appAndAddressReview.CustomerId);
                    if (customer!=null)
                    {
                        if (customer.BillingAddress!=null && customer.BillingAddress.FirstName!=null)
                        {
                            appAndAddressReviewModel.CustomerName = customer.BillingAddress.FirstName;
                        }
                        else
                        {
                            appAndAddressReviewModel.CustomerName = customer.Username;
                        }

                    }
                    return appAndAddressReviewModel;
                }),
                Total = appAndAddressReviews.TotalCount
            };

            return model;
        }

        #endregion
    }
}

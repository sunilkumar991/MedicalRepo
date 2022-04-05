using Nop.Core;
using Nop.Core.Domain.DeliveryBoy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.DeliveryBoy
{
    /// <summary>
    /// App And Address Review service interface
    /// Created By : Sunil Kumar S
    /// Created On : 24-Jan-2019
    /// </summary>
    public partial interface IAppAndAddressReviewService
    {
        /// <summary>
        /// GetAppAndAddressReviewById
        /// </summary>
        /// <param name="orderNo">App and Address Review identifier</param>
        /// <param name="customerId">App and Address Review identifier</param>
        /// <param name="ReviewTypeId">App and Address Review identifier</param>
        /// <returns>App and Address Review</returns>
        AppNAddressReview GetAppAndAddressReview(string orderNo, int customerId, int ReviewTypeId);

        IPagedList<AppNAddressReview> GetAllAppAndAddressReviews(
            DateTime? fromUtc = null, DateTime? toUtc = null, int reviewTypeId = 0,
            string reviewtext = null,
            int pageIndex = 0, int pageSize = int.MaxValue);
        
        /// <summary>
        /// Inserts AppAndAddressReview
        /// </summary>
        /// <param name="AppAndAddressReview">AppAndAddressReview</param>
        void InsertAppAndAddressReview(AppNAddressReview appAndAddressReview);

        /// <summary>
        /// Updates the AppAndAddressReview
        /// </summary>
        /// <param name="AppAndAddressReview">AppAndAddressReview</param>
        void UpdateAppAndAddressReview(AppNAddressReview appAndAddressReview);

        /// <summary>
        /// Gets App And Address review
        /// </summary>
        /// <param name="AppAndAddressReviewId">App And Address review identifier</param>
        /// <returns>App And Address review</returns>
        AppNAddressReview GetAppAndAddressReviewById(int appAndAddressReviewId);

        /// <summary>
        /// Get App And Address reviews by identifiers
        /// </summary>
        /// <param name="appAndAddressIds">App And Address review identifiers</param>
        /// <returns>App and Address reviews</returns>
        IList<AppNAddressReview> GetAppAndAddressReviewByIds(int[] appAndAddressIds);

        /// <summary>
        /// Deletes App And Address reviews
        /// </summary>
        /// <param name="appAndAddressReviews">App And Address reviews</param>
        void DeleteAppandAddressReviews(IList<AppNAddressReview> appAndAddressReviews);
    }
}

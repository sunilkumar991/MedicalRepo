using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.DeliveryBoy;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Events;

namespace Nop.Services.DeliveryBoy
{
    /// <summary>
    /// App And Address Review service
    /// Created By : Sunil Kumar S
    /// Created On : 24-Jan-2019
    /// </summary>
    public partial class AppAndAddressReviewService : IAppAndAddressReviewService
    {
        private readonly IRepository<AppNAddressReview> _AppAndAddressReviewRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly CatalogSettings _catalogSettings;
        private readonly ICacheManager _cacheManager;

        public AppAndAddressReviewService(IRepository<AppNAddressReview> AppAndAddressReviewRepository, IEventPublisher eventPublisher, CatalogSettings catalogSettings, ICacheManager cacheManager)
        {
            _AppAndAddressReviewRepository = AppAndAddressReviewRepository;
            _eventPublisher = eventPublisher;
            _catalogSettings = catalogSettings;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Gets all App and address Reviews
        /// </summary>
        /// <returns>AppAndAddressReview</returns>
        public virtual IList<AppNAddressReview> GetAllAppAndAddressReview()
        {
            var query = from p in _AppAndAddressReviewRepository.Table
                        orderby p.Id
                        select p;
            var appAndAddressReview = query.ToList();
            return appAndAddressReview;
        }

        /// <summary>
        /// GetAppAndAddressReviewById
        /// </summary>
        /// <param name="orderNo">AppandAddress identifier</param>
        /// <param name="customerId">AppandAddress identifier</param>
        /// <param name="ReviewTypeId">AppandAddress identifier</param>
        /// <returns>AppandAddress</returns>
        public virtual AppNAddressReview GetAppAndAddressReview(string orderNo, int customerId, int ReviewTypeId)
        {
            if (orderNo == "" || customerId == 0 || ReviewTypeId == 0)
                return null;
            var query = from p in _AppAndAddressReviewRepository.Table where p.OrderNo == orderNo.Trim() && p.CustomerId == customerId && p.ReviewTypeId == ReviewTypeId select p;
            var review = query.ToList().FirstOrDefault();
            return review;
        }

        /// <summary>
        /// Inserts an AppAndAddressReview
        /// </summary>
        /// <param name="AppAndAddressReview">AppAndAddressReview</param>
        public virtual void InsertAppAndAddressReview(AppNAddressReview appAndAddressReview)
        {
            if (appAndAddressReview == null)
                throw new ArgumentNullException(nameof(appAndAddressReview));

            appAndAddressReview.CreatedOnUtc = DateTime.UtcNow;

            _AppAndAddressReviewRepository.Insert(appAndAddressReview);

            //event notification
            _eventPublisher.EntityInserted(appAndAddressReview);
        }

        /// <summary>
        /// Updates the AppAndAddressReview
        /// </summary>
        /// <param name="AppAndAddressReview">AppAndAddressReview</param>
        public virtual void UpdateAppAndAddressReview(AppNAddressReview appAndAddressReview)
        {
            if (appAndAddressReview == null)
                throw new ArgumentNullException(nameof(appAndAddressReview));

            _AppAndAddressReviewRepository.Update(appAndAddressReview);

            //event notification
            _eventPublisher.EntityUpdated(appAndAddressReview);
        }


        /// <summary>
        /// Gets all App and Address reviews
        /// </summary>
        /// <param name="customerId">Customer identifier (who wrote a review); 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
        /// <param name="fromUtc">Item creation from; null to load all records</param>
        /// <param name="toUtc">Item item creation to; null to load all records</param>
        /// <param name="message">Search title or review text; null to load all records</param>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="AppandAddressReviewId">The App and Address identifier; pass 0 to load all records</param>
        /// <param name="vendorId">The vendor identifier (limit to AppandAddress of this vendor); pass 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Reviews</returns>
        public virtual IPagedList<AppNAddressReview> GetAllAppAndAddressReviews(
            DateTime? fromUtc = null, DateTime? toUtc = null, int reviewTypeId = 0,
            string reviewtext = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _AppAndAddressReviewRepository.Table;

            if (reviewTypeId>0)
                query = query.Where(pr => pr.ReviewTypeId == reviewTypeId);
            if (fromUtc.HasValue)
                query = query.Where(pr => fromUtc.Value <= pr.CreatedOnUtc);
            if (toUtc.HasValue)
                query = query.Where(pr => toUtc.Value >= pr.CreatedOnUtc);
            if (!string.IsNullOrEmpty(reviewtext))
                query = query.Where(pr => pr.ReviewText.Contains(reviewtext));
           
            query = _catalogSettings.AppAndAddressReviewsSortByCreatedDateAscending
                ? query.OrderBy(pr => pr.CreatedOnUtc).ThenBy(pr => pr.Id)
                : query.OrderByDescending(pr => pr.CreatedOnUtc).ThenBy(pr => pr.Id);

            var appAndAddressReviews = new PagedList<AppNAddressReview>(query, pageIndex, pageSize);

            return appAndAddressReviews;
        }


        /// <summary>
        /// Gets App And Address review
        /// </summary>
        /// <param name="appAndAddressReviewId">App And Address review identifier</param>
        /// <returns>App And Address review</returns>
        public virtual AppNAddressReview GetAppAndAddressReviewById(int appAndAddressReviewId)
        {
            if (appAndAddressReviewId == 0)
                return null;

            return _AppAndAddressReviewRepository.GetById(appAndAddressReviewId);
        }

        /// <summary>
        /// Get App And Address reviews by identifiers
        /// </summary>
        /// <param name="appAndAddressIds">App And Address review identifiers</param>
        /// <returns>App and Address reviews</returns>
        public virtual IList<AppNAddressReview> GetAppAndAddressReviewByIds(int[] appAndAddressIds)
        {
            if (appAndAddressIds == null || appAndAddressIds.Length == 0)
                return new List<AppNAddressReview>();

            var query = from pr in _AppAndAddressReviewRepository.Table
                        where appAndAddressIds.Contains(pr.Id)
                        select pr;
            var appAndAddressReviews = query.ToList();
            //sort by passed identifiers
            var sortedAppAndAddressReviews = new List<AppNAddressReview>();
            foreach (var id in appAndAddressIds)
            {
                var appAndAddressReview = appAndAddressReviews.Find(x => x.Id == id);
                if (appAndAddressReview != null)
                    sortedAppAndAddressReviews.Add(appAndAddressReview);
            }

            return sortedAppAndAddressReviews;
        }


        /// <summary>
        /// Deletes App and Address reviews
        /// </summary>
        /// <param name="appAndAddressReviews">App and Address reviews</param>
        public virtual void DeleteAppandAddressReviews(IList<AppNAddressReview> appAndAddressReviews)
        {
            if (appAndAddressReviews == null)
                throw new ArgumentNullException(nameof(appAndAddressReviews));

            _AppAndAddressReviewRepository.Delete(appAndAddressReviews);

            _cacheManager.RemoveByPattern(NopCatalogDefaults.AppAndAddressReviewPatternCacheKey);
            //event notification
            foreach (var appAndAddressReview in appAndAddressReviews)
            {
                _eventPublisher.EntityDeleted(appAndAddressReview);
            }
        }

    }
}

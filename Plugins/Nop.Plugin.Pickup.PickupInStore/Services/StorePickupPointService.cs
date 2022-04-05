using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Plugin.Pickup.PickupInStore.Domain;

namespace Nop.Plugin.Pickup.PickupInStore.Services
{
    /// <summary>
    /// Store pickup point service
    /// </summary>
    public partial class StorePickupPointService : IStorePickupPointService
    {
        #region Constants

        /// <summary>
        /// Cache key for pickup points
        /// </summary>
        /// <remarks>
        /// {0} : page index
        /// {1} : page size
        /// {2} : current store ID
        /// </remarks>
        private const string PICKUP_POINT_ALL_KEY = "Nop.pickuppoint.all-{0}-{1}-{2}";
        private const string PICKUP_POINT_PATTERN_KEY = "Nop.pickuppoint.";

        #endregion

        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IRepository<StorePickupPoint> _storePickupPointRepository;
        private readonly IRepository<StorePickupPointCityMapping> _storePickupPointCityMappingRepository;
        private readonly IStorePickupPointCityMappingService _storePickupPointCityMappingService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="storePickupPointRepository">Store pickup point repository</param>
        public StorePickupPointService(ICacheManager cacheManager,
            IRepository<StorePickupPoint> storePickupPointRepository,
            IRepository<StorePickupPointCityMapping> storePickupPointCityMappingRepository,
            IStorePickupPointCityMappingService storePickupPointCityMappingService)
        {
            this._cacheManager = cacheManager;
            this._storePickupPointRepository = storePickupPointRepository;
            this._storePickupPointCityMappingRepository = storePickupPointCityMappingRepository;
            _storePickupPointCityMappingService = storePickupPointCityMappingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all pickup points
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Pickup points</returns>
        public virtual IPagedList<StorePickupPoint> GetAllStorePickupPoints(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = string.Format(PICKUP_POINT_ALL_KEY, pageIndex, pageSize, storeId);
            return _cacheManager.Get(key, () =>
            {
                var query = _storePickupPointRepository.Table.Where(p => p.IsDeleted == false);
                if (storeId > 0)
                    query = query.Where(point => point.StoreId == storeId || point.StoreId == 0);
                query = query.OrderBy(point => point.DisplayOrder).ThenBy(point => point.Name);

                return new PagedList<StorePickupPoint>(query, pageIndex, pageSize);
            });
        }

        /// <summary>
        /// Gets all pickup points by CityID
        /// Created By : Ankur Shrivastava
        /// Date : 11/Oct/2018
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Pickup points</returns>
        public virtual List<StorePickupPoint> GetAllStorePickupPointsByCityId(int cityId)
        {
            List<StorePickupPoint> list = new List<StorePickupPoint>();
            var mapping = _storePickupPointCityMappingRepository.Table.Where(o => o.CityId == cityId).ToList();

            foreach (StorePickupPointCityMapping m in mapping)
            {
                list.Add(_storePickupPointRepository.GetById(m.StorePickupPointId));
            }
            return list.Where(p => p.IsDeleted == false).ToList();
        }

        /// <summary>
        /// Gets a pickup point
        /// </summary>
        /// <param name="pickupPointId">Pickup point identifier</param>
        /// <returns>Pickup point</returns>
        public virtual StorePickupPoint GetStorePickupPointById(int pickupPointId)
        {
            if (pickupPointId == 0)
                return null;

            return _storePickupPointRepository.GetById(pickupPointId);
        }

        /// <summary>
        /// Inserts a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        public virtual void InsertStorePickupPoint(StorePickupPoint pickupPoint, int cityId)
        {
            if (pickupPoint == null)
                throw new ArgumentNullException(nameof(pickupPoint));

            // _storePickupPointRepository.Insert(pickupPoint);
            // Added by Alexandar Rajavel on 14-Dec-2014
            var result = _storePickupPointRepository.InsertAndReturn(pickupPoint).ToList();
            var pickupPointCityMapping = new StorePickupPointCityMapping() { StorePickupPointId = result.LastOrDefault().Id, CityId = cityId };
            _storePickupPointCityMappingService.InsertStorePickupPointCityMapping(pickupPointCityMapping);

            _cacheManager.RemoveByPattern(PICKUP_POINT_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        public virtual void UpdateStorePickupPoint(StorePickupPoint pickupPoint)
        {
            if (pickupPoint == null)
                throw new ArgumentNullException(nameof(pickupPoint));

            _storePickupPointRepository.Update(pickupPoint);
            _cacheManager.RemoveByPattern(PICKUP_POINT_PATTERN_KEY);
        }

        /// <summary>
        /// Deletes a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        public virtual void DeleteStorePickupPoint(StorePickupPoint pickupPoint)
        {
            if (pickupPoint == null)
                throw new ArgumentNullException(nameof(pickupPoint));

            //_storePickupPointRepository.Delete(pickupPoint);
            pickupPoint.IsDeleted = true;
            _storePickupPointRepository.Update(pickupPoint);
            _cacheManager.RemoveByPattern(PICKUP_POINT_PATTERN_KEY);
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Plugin.Pickup.PickupInStore.Domain;

namespace Nop.Plugin.Pickup.PickupInStore.Services
{
    public partial class StorePickupPointCityMappingService : IStorePickupPointCityMappingService
    {
        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IRepository<StorePickupPointCityMapping> _storePickupPointCityMappingRepository;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="storePickupPointRepository">Store pickup point repository</param>
        public StorePickupPointCityMappingService(ICacheManager cacheManager,
            IRepository<StorePickupPointCityMapping> storePickupPointCityMappingRepository)
        {
            this._cacheManager = cacheManager;
            this._storePickupPointCityMappingRepository = storePickupPointCityMappingRepository;
        }

        #endregion

        public void DeleteStorePickupPointCityMapping(StorePickupPointCityMapping storePickupPointCityMapping)
        {
            if (storePickupPointCityMapping == null)
                throw new ArgumentNullException(nameof(storePickupPointCityMapping));

            _storePickupPointCityMappingRepository.Delete(storePickupPointCityMapping);
        }

        public List<StorePickupPointCityMapping> GetAllStorePickupPointCityMappings(int cityId)
        {
            var query = _storePickupPointCityMappingRepository.Table;
            if (cityId > 0)
                query = query.Where(point => point.CityId == cityId);
            return query.ToList();
            //query = query.OrderBy(point => point.DisplayOrder).ThenBy(point => point.Name);
        }

        public StorePickupPointCityMapping GetStorePickupPointCityMappingById(int Id)
        {
            if (Id == 0)
                return null;

            return _storePickupPointCityMappingRepository.GetById(Id);
        }

        public void InsertStorePickupPointCityMapping(StorePickupPointCityMapping storePickupPointCityMapping)
        {
            if (storePickupPointCityMapping == null)
                throw new ArgumentNullException(nameof(storePickupPointCityMapping));

            _storePickupPointCityMappingRepository.Insert(storePickupPointCityMapping);
        }

        public void UpdateStorePickupPointCityMapping(StorePickupPointCityMapping storePickupPointCityMapping)
        {
            if (storePickupPointCityMapping == null)
                throw new ArgumentNullException(nameof(storePickupPointCityMapping));

            _storePickupPointCityMappingRepository.Update(storePickupPointCityMapping);
        }
    }
}

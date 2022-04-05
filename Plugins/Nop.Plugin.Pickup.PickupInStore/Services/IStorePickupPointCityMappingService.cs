using Nop.Core;
using Nop.Plugin.Pickup.PickupInStore.Domain;
using System.Collections.Generic;

namespace Nop.Plugin.Pickup.PickupInStore.Services
{
    public partial interface IStorePickupPointCityMappingService
    {
        /// <summary>
        /// Gets all pickup points
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns>Pickup points</returns>
        List<StorePickupPointCityMapping> GetAllStorePickupPointCityMappings(int cityId);

        /// <summary>
        /// Gets a pickup point
        /// </summary>
        /// <param name="pickupPointId">Pickup point identifier</param>
        /// <returns>Pickup point</returns>
        StorePickupPointCityMapping GetStorePickupPointCityMappingById(int Id);

        /// <summary>
        /// Inserts a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        void InsertStorePickupPointCityMapping(StorePickupPointCityMapping storePickupPointCityMapping);

        /// <summary>
        /// Updates a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        void UpdateStorePickupPointCityMapping(StorePickupPointCityMapping storePickupPointCityMapping);

        /// <summary>
        /// Deletes a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        void DeleteStorePickupPointCityMapping(StorePickupPointCityMapping storePickupPointCityMapping);
    }
}

using Nop.Core;
using Nop.Plugin.Pickup.PickupInStore.Domain;
using System.Collections.Generic;

namespace Nop.Plugin.Pickup.PickupInStore.Services
{
    /// <summary>
    /// Store pickup point service interface
    /// </summary>
    public partial interface IStorePickupPointService
    {
        /// <summary>
        /// Gets all pickup points
        /// </summary>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Pickup points</returns>
        IPagedList<StorePickupPoint> GetAllStorePickupPoints(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Added By Ankur on : 11-OCT-2018
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        List<StorePickupPoint> GetAllStorePickupPointsByCityId(int cityId);

        /// <summary>
        /// Gets a pickup point
        /// </summary>
        /// <param name="pickupPointId">Pickup point identifier</param>
        /// <returns>Pickup point</returns>
        StorePickupPoint GetStorePickupPointById(int pickupPointId);

        /// <summary>
        /// Inserts a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        void InsertStorePickupPoint(StorePickupPoint pickupPoint, int cityId);

        /// <summary>
        /// Updates a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        void UpdateStorePickupPoint(StorePickupPoint pickupPoint);

        /// <summary>
        /// Deletes a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        void DeleteStorePickupPoint(StorePickupPoint pickupPoint);
    }
}
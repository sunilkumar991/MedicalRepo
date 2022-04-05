using Nop.Core;

namespace Nop.Plugin.Pickup.PickupInStore.Domain
{
    public partial class StorePickupPointCityMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets a store identifier
        /// </summary>
        public int StorePickupPointId { get; set; }
        /// <summary>
        /// Gets or sets a store identifier
        /// </summary>
        public int CityId { get; set; }
    }
}

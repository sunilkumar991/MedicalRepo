using Nop.Core;

namespace Nop.Plugin.Pickup.PickupInStore.Domain
{
    /// <summary>
    /// Represents a pickup point of store
    /// </summary>
    public partial class StorePickupPoint : BaseEntity
    {
        /// <summary>
        /// Gets or sets a name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets an address identifier
        /// </summary>
        public int AddressId { get; set; }

        /// <summary>
        /// Gets or sets a fee for the pickup
        /// </summary>
        public decimal PickupFee { get; set; }

        /// <summary>
        /// Gets or sets an opening hours
        /// </summary>
        public string OpeningHours { get; set; }

        /// <summary>
        /// Gets or sets a display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a store identifier
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Added by Alexandar Rajavel on 14-Dec-2018
        /// Gets or sets a value indicating whether the entity is deleted
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
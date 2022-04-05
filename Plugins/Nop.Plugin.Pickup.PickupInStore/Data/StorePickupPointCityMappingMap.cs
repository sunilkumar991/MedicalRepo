using Nop.Data.Mapping;
using Nop.Plugin.Pickup.PickupInStore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nop.Plugin.Pickup.PickupInStore.Data
{
    public partial class StorePickupPointCityMappingMap : NopEntityTypeConfiguration<StorePickupPointCityMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<StorePickupPointCityMapping> builder)
        {
            builder.ToTable(nameof(StorePickupPointCityMapping));
            builder.HasKey(point => point.Id);
        }

        #endregion
    }
}

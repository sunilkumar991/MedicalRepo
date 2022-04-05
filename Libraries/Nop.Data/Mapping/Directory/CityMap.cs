using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Directory;

namespace Nop.Data.Mapping.Directory
{
    /// <summary>
    /// Represents a City mapping configuration
    /// </summary>
    public partial class CityMap : NopEntityTypeConfiguration<City>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<City> builder)
        {
            builder.ToTable(nameof(City));
            builder.HasKey(city => city.Id);
            builder.Property(state => state.Name).HasMaxLength(200).IsRequired();
            builder.Property(state => state.Abbreviation).HasMaxLength(5);
            builder.HasOne(city => city.StateProvince)
                .WithMany(state => state.City)
                .HasForeignKey(city => city.StateId)
                .IsRequired();
            base.Configure(builder);
        }

        #endregion

    }
}

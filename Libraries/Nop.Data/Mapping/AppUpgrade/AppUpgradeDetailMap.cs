using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.AppUpgrade;
using Nop.Core.Domain.DeliveryBoy;

namespace Nop.Data.Mapping.AppUpgrade
{
    /// <summary>
    /// Represents a App Upgrade Detail
    /// </summary>
    public partial class AppUpgradeDetailMap : NopEntityTypeConfiguration<AppUpgradeDetails>
    {
        #region Methods
        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<AppUpgradeDetails> builder)
        {
            builder.ToTable(nameof(AppUpgradeDetails));
            builder.HasKey(Review => Review.Id);

            builder.Property(Review => Review.AppLatestVersionName).HasColumnName("AppLatestVersionName");
            builder.Property(Review => Review.AppLatestVersionCode).HasColumnName("AppLatestVersionCode");

            builder.Property(Review => Review.AppURL).HasColumnName("AppURL");
            builder.Property(Review => Review.IsUpdateRequired).HasColumnName("IsUpdateRequired");
            builder.Property(Review => Review.IsBackgroundDownload).HasColumnName("IsBackgroundDownload");
            builder.Property(Review => Review.message).HasColumnName("message");
            builder.Property(Review => Review.IsProduction).HasColumnName("IsProduction");
            builder.Property(Review => Review.playStoreUrl).HasColumnName("playStoreUrl");
            builder.Property(Review => Review.apkName).HasColumnName("apkName");
            builder.Property(Review => Review.IsUpdateForceRequired).HasColumnName("IsUpdateForceRequired");
            builder.Property(Review => Review.DeviceId).HasColumnName("DeviceId");
            base.Configure(builder);
        }
        #endregion
    }
}

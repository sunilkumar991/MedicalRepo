using Nop.Data.Mapping;
using BS.Plugin.NopStation.MobileApp.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BS.Plugin.NopStation.MobileApp.Data
{
    public partial class MessageTemplateMap : NopEntityTypeConfiguration<NotificationMessageTemplate>
    {
        public override void Configure(EntityTypeBuilder<NotificationMessageTemplate> builder)
        {
            builder.ToTable("Bs_NotificationMessageTemplate");
            builder.HasKey(mt => mt.Id);

            builder.Property(mt => mt.Name).IsRequired().HasMaxLength(200);
            builder.Property(mt => mt.Subject).HasMaxLength(1000);

            //added by Sunil Kumar at 27-04-2020
            builder.Property(mt => mt.Body).IsRequired().HasMaxLength(1000);
            builder.Property(mt => mt.IsActive);

            builder.Property(mt => mt.AttachedDownloadId);
            builder.Property(mt => mt.LimitedToStores);
        }
    }
}
using BS.Plugin.NopStation.MobileApp.Domain;
using Nop.Data.Mapping;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BS.Plugin.NopStation.MobileApp.Data
{
    public class QueuedNotificationMap : NopEntityTypeConfiguration<QueuedNotification>
    {
        public override void Configure(EntityTypeBuilder<QueuedNotification> builder)
        {
            builder.ToTable("Bs_QueuedNotification");
            builder.HasKey(x => x.Id);
            builder.Ignore(x => x.DeviceType);
            builder.Ignore(x => x.NotificationType);
        }
    }
}

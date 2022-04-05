using BS.Plugin.NopStation.MobileApp.Domain;
using Nop.Data.Mapping;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BS.Plugin.NopStation.MobileApp.Data
{
    public class ScheduledNotificationMap : NopEntityTypeConfiguration<ScheduledNotification>
    {
        public override void Configure(EntityTypeBuilder<ScheduledNotification> builder)
        {
            builder.ToTable("Bs_ScheduledNotification");
            builder.HasKey(x => x.Id);
            builder.Ignore(x => x.NotificationType);
        }
    }
}

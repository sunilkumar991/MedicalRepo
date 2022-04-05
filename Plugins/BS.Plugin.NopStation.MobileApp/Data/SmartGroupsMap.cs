using Nop.Data.Mapping;
using BS.Plugin.NopStation.MobileApp.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BS.Plugin.NopStation.MobileApp.Data
{
    public partial class SmartGroupsMap : NopEntityTypeConfiguration<SmartGroup>
    {
        public override void Configure(EntityTypeBuilder<SmartGroup> builder)
        {
            builder.ToTable("Bs_SmartGroups");
            builder.HasKey(x => x.Id);

            base.Configure(builder);
        }
    }
}
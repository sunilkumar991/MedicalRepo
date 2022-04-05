using BS.Plugin.NopStation.MobileWebApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;

namespace BS.Plugin.NopStation.MobileWebApi.Data
{
    public partial class BS_ContentManagementMap : NopEntityTypeConfiguration<BS_ContentManagement>
    {
        public override void Configure(EntityTypeBuilder<BS_ContentManagement> builder)
        {
            builder.ToTable("BS_ContentManagement");
            builder.HasKey(t => t.Id);
        }
    }
}

using BS.Plugin.NopStation.MobileWebApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;

namespace BS.Plugin.NopStation.MobileWebApi.Data
{
    public partial class BS_ContentManagementTemplateMap : NopEntityTypeConfiguration<BS_ContentManagementTemplate>
    {
        public override void Configure(EntityTypeBuilder<BS_ContentManagementTemplate> builder)
        {
            builder.ToTable("BS_ContentManagementTemplate");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(400);
            builder.Property(t => t.ViewPath).IsRequired().HasMaxLength(400);
        }
    }
}

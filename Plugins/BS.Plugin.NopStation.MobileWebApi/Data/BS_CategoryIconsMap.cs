using BS.Plugin.NopStation.MobileWebApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;

namespace BS.Plugin.NopStation.MobileWebApi.Data
{
    public partial class BS_CategoryIconsMap : NopEntityTypeConfiguration<BS_CategoryIcons>
    {
        public override void Configure(EntityTypeBuilder<BS_CategoryIcons> builder)
        {
            builder.ToTable("BS_CategoryIcons");
            builder.HasKey(x => x.Id);
        }
    }
}

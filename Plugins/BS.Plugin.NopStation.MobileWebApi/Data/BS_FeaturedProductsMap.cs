using BS.Plugin.NopStation.MobileWebApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;

namespace BS.Plugin.NopStation.MobileWebApi.Data
{
    public partial class BS_FeaturedProductsMap : NopEntityTypeConfiguration<BS_FeaturedProducts>
    {
        public override void Configure(EntityTypeBuilder<BS_FeaturedProducts> builder)
        {
            builder.ToTable("BS_FeaturedProducts");
            builder.HasKey(x => x.Id);
        }
    }
}

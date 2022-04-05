using BS.Plugin.NopStation.MobileWebApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;

namespace BS.Plugin.NopStation.MobileWebApi.Data
{
    public partial class BS_SliderMap : NopEntityTypeConfiguration<BS_Slider>
    {
        public override void Configure(EntityTypeBuilder<BS_Slider> builder)
        {
            builder.ToTable("BS_Slider");
            builder.HasKey(x => x.Id);
        }
    }
}

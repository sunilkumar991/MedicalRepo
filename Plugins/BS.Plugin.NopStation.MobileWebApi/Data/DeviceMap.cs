using Nop.Data.Mapping;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BS.Plugin.NopStation.MobileWebApi.Data
{
    public partial class DeviceMap : NopEntityTypeConfiguration<Device>
    {
        public override void Configure(EntityTypeBuilder<Device> builder)
        {
            builder.ToTable("BS_WebApi_Device");
            builder.HasKey(x => x.Id);
        }
    }
}
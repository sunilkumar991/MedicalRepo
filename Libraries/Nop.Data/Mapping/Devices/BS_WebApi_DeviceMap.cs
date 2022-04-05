using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Devices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.Devices
{
    public partial class BS_WebApi_DeviceMap : NopEntityTypeConfiguration<BS_WebApi_Device>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<BS_WebApi_Device> builder)
        {
            builder.ToTable(nameof(BS_WebApi_Device));
            builder.HasKey(device => device.Id);
            base.Configure(builder);
        }

        #endregion

    }
}

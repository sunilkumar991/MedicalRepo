using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.HelpNSupport;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.HelpNSupport
{
    // Created by Alexandar Rajavel on 08-Feb-2019
    public class HelpandSupportMap : NopEntityTypeConfiguration<HelpandSupport>
    {
        #region Methods
        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<HelpandSupport> builder)
        {
            builder.ToTable(nameof(HelpandSupport));
            builder.HasKey(support => support.Id);
            base.Configure(builder);
        }

        #endregion
    }
}

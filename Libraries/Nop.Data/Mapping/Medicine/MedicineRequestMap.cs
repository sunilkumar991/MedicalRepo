﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Medicine;

namespace Nop.Data.Mapping.Medicine
{
    public partial class MedicineRequestMap : NopEntityTypeConfiguration<MedicineRequest>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<MedicineRequest> builder)
        {
            builder.ToTable(nameof(MedicineRequest));
            builder.HasKey(medicineRequest => medicineRequest.Id);
            base.Configure(builder);
        }

        #endregion

    }
}

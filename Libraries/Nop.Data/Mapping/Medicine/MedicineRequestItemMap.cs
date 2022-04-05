using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Medicine;

namespace Nop.Data.Mapping.Medicine
{
    public partial class MedicineRequestItemMap : NopEntityTypeConfiguration<MedicineRequestItem>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<MedicineRequestItem> builder)
        {
            builder.ToTable(nameof(MedicineRequestItem));
            builder.HasKey(medicineRequestItem => medicineRequestItem.Id);
            base.Configure(builder);
        }

        #endregion

    }
}

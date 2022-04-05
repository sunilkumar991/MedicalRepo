using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    public partial class CustomerRemarkMap : NopEntityTypeConfiguration<CustomerRemark>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<CustomerRemark> builder)
        {
            builder.ToTable(nameof(CustomerRemark));
            builder.HasKey(remark => remark.Id);

            builder.HasOne(remark => remark.Customer)
                .WithMany()
                .HasForeignKey(remark => remark.CustomerId)
                .IsRequired();
            builder.Ignore(remark => remark.NetworkRemark);
            builder.Property(remark => remark.NetworkRemark).HasMaxLength(4000);
            base.Configure(builder);
        }

        #endregion
    }
}

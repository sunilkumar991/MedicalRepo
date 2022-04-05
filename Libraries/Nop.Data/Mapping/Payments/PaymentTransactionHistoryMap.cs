using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Payments;

namespace Nop.Data.Mapping.Payments
{
    /// <summary>
    /// Created By : Alexandar Rajavel on 05-Nov-2018
    /// Represents a payment transaction history mapping configuration
    /// </summary>
    public class PaymentTransactionHistoryMap : NopEntityTypeConfiguration<PaymentTransactionHistory>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<PaymentTransactionHistory> builder)
        {
            builder.ToTable(nameof(PaymentTransactionHistory));
            builder.HasKey(payment => payment.Id);
            base.Configure(builder);
        }

        #endregion
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.DeliveryBoy;

namespace Nop.Data.Mapping.DeliveryBoy
{
    /// <summary>
    /// Represents a App And Address Review mapping configuration
    /// </summary>
    public partial class AppNAddressReviewMap : NopEntityTypeConfiguration<AppNAddressReview>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<AppNAddressReview> builder)
        {
            builder.ToTable(nameof(AppNAddressReview));
            builder.HasKey(Review => Review.Id);
            
            builder.Property(Review => Review.CustomerId).HasColumnName("CustomerId");
            builder.Property(Review => Review.ReviewTypeId).HasColumnName("ReviewTypeId");

            builder.Property(Review => Review.OrderNo).HasMaxLength(1000);
            builder.Property(Review => Review.ReviewText).HasMaxLength(1000);

            builder.Property(Review => Review.AddressId).HasColumnName("AddressId");
            builder.Property(Review => Review.ReviewTypeId).HasColumnName("ReviewTypeId");
            builder.Property(Review => Review.Rating).HasColumnName("Rating");
            builder.Property(Review => Review.CreatedOnUtc).HasColumnName("CreatedOnUtc");
           
            base.Configure(builder);
        }
        #endregion
    }
}


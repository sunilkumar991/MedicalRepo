using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Common;
namespace Nop.Data.Mapping.Common
{
    public partial class ContactDetailMap : NopEntityTypeConfiguration<ContactDetail>
    {

        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ContactDetail> builder)
        {
            builder.ToTable(nameof(ContactDetail));
            builder.HasKey(contact => contact.Id);
            base.Configure(builder);
        }

        #endregion
    }
}

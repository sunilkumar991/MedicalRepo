using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Common
{
    public partial class ContactDetail : BaseEntity
    {
        public string MobileNumber { get; set; }
        public string ContactDetails { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}

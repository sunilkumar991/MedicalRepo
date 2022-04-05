using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.DeliveryBoy
{
    public partial class AppNAddressReview : BaseEntity
    {
        public int CustomerId { get; set; }
        public int ReviewTypeId { get; set; }
        public string OrderNo { get; set; }
        public int AddressId { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}

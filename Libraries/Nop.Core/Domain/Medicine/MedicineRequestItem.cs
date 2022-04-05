using System;

namespace Nop.Core.Domain.Medicine
{
    public partial class MedicineRequestItem : BaseEntity
    {
        #region Properties
        public int MedicineRequestID { get; set; }
        public string MedicineName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
        public int? ProductId { get; set; }
        #endregion

        #region Navigation properties
        /// <summary>
        /// Gets or sets the MedicineRequest
        /// </summary>
        public virtual MedicineRequest MedicineRequest { get; set; }

        #endregion
    }
}

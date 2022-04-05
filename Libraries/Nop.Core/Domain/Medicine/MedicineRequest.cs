using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;

namespace Nop.Core.Domain.Medicine
{
    public partial class MedicineRequest : BaseEntity
    {
        private ICollection<MedicineRequestItem> _medicineRequestItems;
        #region Properties
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string HospitalName { get; set; }
        public string MobileNumber { get; set; }
        public int CustomerId { get; set; }
        public string PrescriptionImageUrl { get; set; }
        public string Remarks { get; set; }
        public string RejectedReason { get; set; }
        public int RequestStatusId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
        #endregion

        #region Navigation properties
        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public virtual Customer Customer { get; set; }


        /// <summary>
        /// Gets or sets order items
        /// </summary>
        public virtual ICollection<MedicineRequestItem> MedicineRequestItems
        {
            get => _medicineRequestItems ?? (_medicineRequestItems = new List<MedicineRequestItem>());
            protected set => _medicineRequestItems = value;
        }
        #endregion
    }
}

using Nop.Core;
using Nop.Core.Domain.Medicine;
using System.Collections.Generic;

namespace Nop.Services.Medicine
{
    public partial interface IMedicineRequestService
    {
        /// <summary>
        /// Gets a MedicineRequest
        /// </summary>
        /// <param name="requestId">The order identifier</param>
        /// <returns>Order</returns>
        MedicineRequest GetMedicineRequestById(int requestId);

        /// <summary>
        /// Get All Medicine Request details
        /// </summary>
        IPagedList<MedicineRequest> GetAllMedicineRequest(string patientName, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Get failed TransactionHistory details
        /// </summary>
        IList<MedicineRequest> GetMedicineRequestByCustomerId(int customerId);

        /// <summary>
        /// Inserts MedicineRequest
        /// </summary>
        /// <param name="medicineRequest">medicineRequest</param>
        void InsertMedicineRequest(MedicineRequest medicineRequest);

        /// <summary>
        /// Updates the MedicineRequest
        /// </summary>
        /// <param name="medicineRequest">medicineRequest</param>
        void UpdateMedicineRequest(MedicineRequest medicineRequest);

        /// <summary>
        /// Get MedicineRequest by identifiers
        /// </summary>
        /// <param name="Medicine Request">Medicine Request identifiers</param>
        /// <returns>App and Address reviews</returns>
        IList<MedicineRequest> GetMedicineRequestByIds(int[] medicineRequestIds);

        /// <summary>
        /// Deletes Medicine Request
        /// </summary>
        /// <param name="appAndAddressReviews">App and Address reviews</param>
        void DeleteMedicineRequests(IList<MedicineRequest> medicineRequests);

        /// <summary>
        /// Delete an medicine Request Item
        /// </summary>
        /// <param name="orderItem">The order item</param>
        void DeleteMedicineRequestsItem(MedicineRequestItem medicineItem);
    }
}

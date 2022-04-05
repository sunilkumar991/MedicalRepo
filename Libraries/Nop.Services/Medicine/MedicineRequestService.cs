using System;
using System.Collections.Generic;
using Nop.Core.Data;
using Nop.Core.Domain.Medicine;
using Nop.Services.Events;
using Nop.Core.Domain.Catalog;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Catalog;

namespace Nop.Services.Medicine
{
    public partial class MedicineRequestService : IMedicineRequestService
    {
        private readonly IRepository<MedicineRequest> _medicineRequestRepository;
        private readonly IRepository<MedicineRequestItem> _medicineRequestItemRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly CatalogSettings _catalogSettings;
        private readonly ICacheManager _cacheManager;

        public MedicineRequestService(IRepository<MedicineRequest> medicineRequestRepository, IEventPublisher eventPublisher, CatalogSettings catalogSettings, ICacheManager cacheManager, IRepository<MedicineRequestItem> medicineRequestItemRepository)
        {
            _medicineRequestRepository = medicineRequestRepository;
            _eventPublisher = eventPublisher;
            _catalogSettings = catalogSettings;
            _cacheManager = cacheManager;
            _medicineRequestItemRepository = medicineRequestItemRepository;
        }

        public IList<MedicineRequest> GetMedicineRequestByCustomerId(int customerId)
        {
            if (customerId == 0)
                return null;
            var query = from m in _medicineRequestRepository.Table
                        where m.CustomerId == customerId && !m.IsDeleted
                        select m;
            var medicineRequests = query.ToList();
            return medicineRequests;
        }

        public IPagedList<MedicineRequest> GetAllMedicineRequest(string patientName, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _medicineRequestRepository.Table;

            if (!string.IsNullOrEmpty(patientName))
                query = query.Where(pr => pr.PatientName == patientName);

            query = _catalogSettings.AppAndAddressReviewsSortByCreatedDateAscending
                ? query.OrderBy(pr => pr.CreatedOnUtc).ThenBy(pr => pr.Id)
                : query.OrderByDescending(pr => pr.CreatedOnUtc).ThenBy(pr => pr.Id);

            var medicineRequests = new PagedList<MedicineRequest>(query, pageIndex, pageSize);
            return medicineRequests;
        }

        public MedicineRequest GetMedicineRequestById(int requestId)
        {
            if (requestId == 0)
                return null;

            return _medicineRequestRepository.GetById(requestId);
        }

        public void InsertMedicineRequest(MedicineRequest medicineRequest)
        {
            if (medicineRequest == null)
                throw new ArgumentNullException(nameof(medicineRequest));

            medicineRequest.CreatedOnUtc = DateTime.UtcNow;
            _medicineRequestRepository.Insert(medicineRequest);

            //event notification
            _eventPublisher.EntityInserted(medicineRequest);
        }

        public void UpdateMedicineRequest(MedicineRequest medicineRequest)
        {
            if (medicineRequest == null)
                throw new ArgumentNullException(nameof(medicineRequest));

            medicineRequest.UpdatedOnUtc = DateTime.UtcNow;
            _medicineRequestRepository.Update(medicineRequest);

            //event notification
            _eventPublisher.EntityInserted(medicineRequest);
        }


        /// <summary>
        /// Get Medicine Request by identifiers
        /// </summary>
        /// <param name="Medicine Request">Medicine Request identifiers</param>
        /// <returns>App and Address reviews</returns>
        public virtual IList<MedicineRequest> GetMedicineRequestByIds(int[] medicineRequestIds)
        {
            if (medicineRequestIds == null || medicineRequestIds.Length == 0)
                return new List<MedicineRequest>();

            var query = from pr in _medicineRequestRepository.Table
                        where medicineRequestIds.Contains(pr.Id)
                        select pr;
            var medicineRequests = query.ToList();
            //sort by passed identifiers
            var sortedAppAndAddressReviews = new List<MedicineRequest>();
            foreach (var id in medicineRequestIds)
            {
                var medicineRequest = medicineRequests.Find(x => x.Id == id);
                if (medicineRequest != null)
                    sortedAppAndAddressReviews.Add(medicineRequest);
            }

            return medicineRequests;
        }


        /// <summary>
        /// Deletes Medicine Request
        /// </summary>
        /// <param name="appAndAddressReviews">App and Address reviews</param>
        public virtual void DeleteMedicineRequests(IList<MedicineRequest> medicineRequests)
        {
            if (medicineRequests == null)
                throw new ArgumentNullException(nameof(medicineRequests));

            _medicineRequestRepository.Delete(medicineRequests);

            _cacheManager.RemoveByPattern(NopCatalogDefaults.MedicineRequestPatternCacheKey);
            //event notification
            foreach (var appAndAddressReview in medicineRequests)
            {
                _eventPublisher.EntityDeleted(appAndAddressReview);
            }
        }


        /// <summary>
        /// Delete an Medicine Request Item
        /// </summary>
        /// <param name="MedicineRequestItem">Medicine Request Item</param>
        public virtual void DeleteMedicineRequestsItem(MedicineRequestItem medicineItem)
        {
            if (medicineItem == null)
                throw new ArgumentNullException(nameof(medicineItem));

            _medicineRequestItemRepository.Delete(medicineItem);

            //event notification
            _eventPublisher.EntityDeleted(medicineItem);
        }
    }
}

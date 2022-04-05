using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Services.Directory;
using Nop.Services.Events;

namespace Nop.Services.Common
{
    /// <summary>
    /// ContactDetail service
    /// Created By : Alexandar Rajavel
    /// Created On : 09-Oct-2018
    /// </summary>
    public partial class ContactDetailService : IContactDetailService
    {
        private readonly IRepository<ContactDetail> _contactDetailRepository;
        private readonly IEventPublisher _eventPublisher;

        public ContactDetailService(IRepository<ContactDetail> contactDetailRepository, IEventPublisher eventPublisher)
        {
            _contactDetailRepository = contactDetailRepository;
            _eventPublisher = eventPublisher;
        }

        public virtual ContactDetail GetContactDetailByMobileNumber(string mobileNumber)
        {
            if (string.IsNullOrWhiteSpace(mobileNumber))
                return null;

            var query = from a in _contactDetailRepository.Table
                        orderby a.Id
                        where a.MobileNumber == mobileNumber
                        select a;

            var contactDetail = query.FirstOrDefault();
            return contactDetail;
        }

        /// <summary>
        /// Inserts an contactDetail
        /// </summary>
        /// <param name="contactDetail">contactDetail</param>
        public virtual void InsertContactDetail(ContactDetail contactDetail)
        {
            if (contactDetail == null)
                throw new ArgumentNullException(nameof(contactDetail));

            contactDetail.CreatedOnUtc = DateTime.UtcNow;

            _contactDetailRepository.Insert(contactDetail);

            //event notification
            _eventPublisher.EntityInserted(contactDetail);
        }

        /// <summary>
        /// Updates the contactDetail
        /// </summary>
        /// <param name="contactDetail">contactDetail</param>
        public virtual void UpdateContactDetail(ContactDetail contactDetail)
        {
            if (contactDetail == null)
                throw new ArgumentNullException(nameof(contactDetail));

            _contactDetailRepository.Update(contactDetail);

            //event notification
            _eventPublisher.EntityUpdated(contactDetail);
        }
    }
}

using Nop.Core.Domain.Common;

namespace Nop.Services.Common
{
    /// <summary>
    /// ContactDetail service interface
    /// Created By : Alexandar Rajavel
    /// Created On : 09-Oct-2018
    /// </summary>
    public partial interface IContactDetailService
    {
        /// <summary>
        /// Get contact details by mobile number
        /// </summary>
        /// <param name="mobileNumber">ContactDetail identifier</param>
        /// <returns>ContactDetail</returns>
        ContactDetail GetContactDetailByMobileNumber(string mobileNumber);

        /// <summary>
        /// Inserts ContactDetail
        /// </summary>
        /// <param name="contactDetail">contactDetail</param>
        void InsertContactDetail(ContactDetail contactDetail);

        /// <summary>
        /// Updates the contactDetail
        /// </summary>
        /// <param name="contactDetail">contactDetail</param>
        void UpdateContactDetail(ContactDetail contactDetail);
    }
}

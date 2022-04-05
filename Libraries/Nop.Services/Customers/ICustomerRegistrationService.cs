using Nop.Core.Domain.Customers;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Customer registration interface
    /// </summary>
    public partial interface ICustomerRegistrationService
    {
        /// <summary>
        /// Validate customer
        /// </summary>
        /// <param name="usernameOrEmail">Username or email</param>
        /// <param name="password">Password</param>
        /// <returns>Result</returns>
        CustomerLoginResults ValidateCustomer(string usernameOrEmail, string password, string simId = "");

        /// <summary>
        /// Register customer
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        CustomerRegistrationResult RegisterCustomer(CustomerRegistrationRequest request);

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        ChangePasswordResult ChangePassword(ChangePasswordRequest request);

        //added by Sunil Kumar At 22-01-19
        /// <summary>
        /// Sets a user email
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="newEmail">New email</param>
        /// <param name="requireValidation">Require validation of new email address</param>
        void SetEmail(Customer customer, string newEmail, bool requireValidation);

        /// <summary>
        /// Sets a IsDisplayEmail
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="IsDisplayEmail">IsDisplayEmail</param>
        void SetIsDisplayEmail(Customer customer, bool IsDisplayEmail);

        /// <summary>
        /// Sets a customer username
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="newUsername">New Username</param>
        void SetUsername(Customer customer, string newUsername);

        /// </summary>
        /// While login if sim id is changed then need to update password
        /// Created By: Alexandar Rajavel
        /// Created On: 25-Sep-2018
        /// <param name="customer">customer</param>
        /// <param name="password">Password</param>
        /// <returns>Result</returns>
        void UpdateNewPassword(Customer customer, string password);
    }
}
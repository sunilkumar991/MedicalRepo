using BS.Plugin.NopStation.MobileWebApi.Models;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Customer;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Customer;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    // Created by Alexandar Rajavel
    public interface IQueuedNotificationApiService
    {
        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Test Sends the IOS and Android notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        void SendNotication(NotificationRequest notification);

        VerifyMobileNumberResponse SendOTP(VerifyMobileNumberRequest model);

        bool SendSMS(SMSRequest model);
    }
}

using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Messages
{
    // Added by Alexandar Rajavel on 25-June-2019
    public interface INotificationService
    {
        /// <summary>
        /// Sends the IOS and Android notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        void SendNotication(QueuedNotification notification);

        VerifyMobileNumberResponse SendOTP(VerifyMobileNumberRequest model);

        VerifyMobileNumberResponse SendOTPHuwai(VerifyMobileNumberRequest model);

        bool SendSMS(SMSRequest model);

       

        //commented by Sunil Kumar at 26-03-2020 no need this method for onestopkitchen
        /// <summary>
        /// Sends the notification to Warehouse management with order id
        /// </summary>
        /// <param name="orderId">The order id.</param>
        //void SendNoticationToWHM(Order orderId);
    }
}

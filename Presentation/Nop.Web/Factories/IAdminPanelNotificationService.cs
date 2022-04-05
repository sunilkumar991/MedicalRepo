using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Web.Models.AdminNotification;
namespace Nop.Web.Areas.Admin.Factories
{
    public interface IAdminPanelNotificationService
    {
        /// <summary>
        /// Sends the IOS and Android notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        void SendNotication(NotificationRequest notification);
    }
}

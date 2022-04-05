using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.AdminNotification
{
    public enum NotificationType
    {
        SimpleText = 1,
        //OrderDetail=2,
        //PromotionVendor=3,
        PromotionProduct = 4,
        //Announcement=5,
        //ActionURL=6,
        PromotionCategory = 7
    }
}

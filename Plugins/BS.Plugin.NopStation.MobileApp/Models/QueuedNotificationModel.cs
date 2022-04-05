using System;
using FluentValidation.Attributes;
using BS.Plugin.NopStation.MobileApp.Validators;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace BS.Plugin.NopStation.MobileApp.Models
{
    [Validator(typeof(QueuedNotificationValidator))]
    public class QueuedNotificationModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.MobileApp.QueuedNotificationModel.Priority")]
        public int Priority { get; set; }
        
        [NopResourceDisplayName("Admin.MobileApp.QueuedNotificationModel.ToCustomerId")]
        public int ToCustomerId { get; set; }

        [NopResourceDisplayName("Admin.MobileApp.QueuedNotificationModel.DeviceTypeId")]
        public int DeviceTypeId { get; set; }

        [NopResourceDisplayName("Admin.MobileApp.QueuedNotificationModel.DeviceType")]
        public string DeviceType { get; set; }

        [NopResourceDisplayName("Admin.MobileApp.QueuedNotificationModel.SubscriptionId")]
        public string SubscriptionId { get; set; }

        [NopResourceDisplayName("Admin.MobileApp.QueuedNotificationModel.ToCustomerName")]
        public string ToCustomerName { get; set; }
        
        [NopResourceDisplayName("Admin.MobileApp.QueuedNotificationModel.GroupId")]
        public int GroupId { get; set; }

        [NopResourceDisplayName("Admin.MobileApp.QueuedNotificationModel.GroupName")]
        public string GroupName { get; set; }
        
        [NopResourceDisplayName("Admin.MobileApp.QueuedNotificationModel.Subject")]
        public string Subject { get; set; }
        
        [NopResourceDisplayName("Admin.MobileApp.QueuedNotificationModel.Message")]
        public string Message { get; set; }
        
        [NopResourceDisplayName("Admin.MobileApp.QueuedNotificationModel.CreatedOnUtc")]
        public DateTime CreatedOnUtc { get; set; }
        
        [NopResourceDisplayName("Admin.MobileApp.QueuedNotificationModel.SentTries")]
        public int SentTries { get; set; }
        
        [NopResourceDisplayName("Admin.MobileApp.QueuedNotificationModel.SentOnUtc")]
        public DateTime? SentOnUtc { get; set; }

        [NopResourceDisplayName("Admin.MobileApp.QueuedNotificationModel.IsSent")]
        public bool IsSent { get; set; }
        
        [NopResourceDisplayName("Admin.MobileApp.QueuedNotificationModel.ErrorLog")]
        public string ErrorLog { get; set; }

        [NopResourceDisplayName("Admin.MobileApp.QueuedNotificationModel.NotificationTypeId")]
        public int NotificationTypeId { get; set; }

        [NopResourceDisplayName("Admin.MobileApp.QueuedNotificationModel.NotificationType")]
        public string NotificationType { get; set; }

        [NopResourceDisplayName("Admin.MobileApp.QueuedNotificationModel.ItemId")]
        public int ItemId { get; set; }

    }
}

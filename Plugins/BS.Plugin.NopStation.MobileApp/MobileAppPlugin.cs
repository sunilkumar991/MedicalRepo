using System.Collections.Generic;
using System.IO;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Plugins;
using BS.Plugin.NopStation.MobileApp.Data;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Common;
using Nop.Services.Vendors;
using Nop.Web.Framework.Menu;
using Nop.Core.Domain.Tasks;
using Nop.Core.Data;
using System.Linq;
using System;
using Microsoft.AspNetCore.Routing;
using Nop.Services.Security;

namespace BS.Plugin.NopStation.MobileApp
{
    /// <summary>
    /// PLugin
    /// </summary>
    public class MobileAppPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly MobileAppObjectContext _objectContext;
        private readonly IWorkContext _workContext;
        private readonly IRepository<ScheduleTask> _scheduleRepository;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        public MobileAppPlugin(IPictureService pictureService,
            ISettingService settingService, IWebHelper webHelper,
            MobileAppObjectContext objectContext, IWorkContext workContext,
            IVendorService vendorService,
            IRepository<ScheduleTask> scheduleRepository,
            ILocalizationService localizationService,
            IPermissionService permissionService)
        {
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._webHelper = webHelper;
            this._objectContext = objectContext;
            this._workContext = workContext;
            this._scheduleRepository = scheduleRepository;
            this._localizationService = localizationService;
            _permissionService = permissionService;
        }

        #region utilites
        private void InstallScheduleTask()
        {
            ScheduleTask data = new ScheduleTask
            {
                Enabled = true,
                Name = "Queued Notification Send",
                Seconds = 60,
                StopOnError = false,
                Type = "BS.Plugin.NopStation.MobileApp.Services.QueuedNotificationSendTask, BS.Plugin.NopStation.MobileApp"
            };
            _scheduleRepository.Insert(data);

            data = new ScheduleTask
            {

                Enabled = true,
                Name = "Insert Queued Notifications",
                Seconds = 60,
                StopOnError = false,
                Type = "BS.Plugin.NopStation.MobileApp.Services.InsertQueuedNotificationsTask, BS.Plugin.NopStation.MobileApp"
            };
            _scheduleRepository.Insert(data);

        }

        private void UninstallScheduleTask()
        {
            ScheduleTask task1 = _scheduleRepository.Table.Where(p => p.Type == "BS.Plugin.NopStation.MobileApp.Services.QueuedNotificationSendTask, BS.Plugin.NopStation.MobileApp").FirstOrDefault();
            _scheduleRepository.Delete(task1);
            ScheduleTask task2 = _scheduleRepository.Table.Where(p => p.Type == "BS.Plugin.NopStation.MobileApp.Services.InsertQueuedNotificationsTask, BS.Plugin.NopStation.MobileApp").FirstOrDefault();
            _scheduleRepository.Delete(task2);
        }
        #endregion

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "BsNotificationAdmin";
            routeValues = new RouteValueDictionary { { "Namespaces", "BS.Plugin.NopStation.MobileApp.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for displaying widget
        /// </summary>
        /// <param name="widgetZone">Widget zone where it's displayed</param>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            #region Local Resources

            _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Test.Push", "Test Push");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Group.List", "Groups");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Group.List.Header", "Group List");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Groups.Header.EditGroup", "Edit Group");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Groups.Title.AddNew", "Create Group");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Groups.Header.AddNew", "Create Group");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Schedules.Manage", "Manage Schedules");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Schedules.Header", "Schedule List");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Schedule.Title.EditSchedule", "Edit Schedule");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Schedule.Header.EditSchedule", "Edit Schedule");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Schedule.Title.CreateSchedule", "Create Schedule");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Schedule.Header.CreateSchedule", "Create Schedule");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Queued.Manage", "Queue");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Queued.Header", "Queue List");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Device.Manage", "Manage Device");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Device.Header", "Device List");

            _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Common.BackToList", "Back to List");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.BsNotification.Groups.Fields.Name", "Group Name");

            _localizationService.AddOrUpdatePluginLocaleResource("BS.Plugin.NopStation.MobileApp.AppleCertFileNameWithPath", "Apple Cert File Name With Path");
            _localizationService.AddOrUpdatePluginLocaleResource("BS.Plugin.NopStation.MobileApp.ApplePassword", "Apple Password");
            _localizationService.AddOrUpdatePluginLocaleResource("BS.Plugin.NopStation.MobileApp.IsAppleProductionMode", "Is Apple Production Mode");
            _localizationService.AddOrUpdatePluginLocaleResource("BS.Plugin.NopStation.MobileApp.GoogleProject_Number", "Google Project Number");
            _localizationService.AddOrUpdatePluginLocaleResource("BS.Plugin.NopStation.MobileApp.GoogleConsoleAPIAccess_KEY", "Google Console API Access KEY");

            _localizationService.AddOrUpdatePluginLocaleResource("BS.Plugin.NopStation.MobileApp.AppleCertFileNameWithPath.Hint", "Apple Cert File Name With Path");
            _localizationService.AddOrUpdatePluginLocaleResource("BS.Plugin.NopStation.MobileApp.ApplePassword.Hint", "Apple Password");
            _localizationService.AddOrUpdatePluginLocaleResource("BS.Plugin.NopStation.MobileApp.IsAppleProductionMode.Hint", "Is Apple Production Mode");
            _localizationService.AddOrUpdatePluginLocaleResource("BS.Plugin.NopStation.MobileApp.GoogleProject_Number.Hint", "Google Project Number");
            _localizationService.AddOrUpdatePluginLocaleResource("BS.Plugin.NopStation.MobileApp.GoogleConsoleAPIAccess_KEY.Hint", "Google Console API Access KEY");

            _localizationService.AddOrUpdatePluginLocaleResource("Admin.ContentManagement.MessageTemplates.Fields.AttachedDownload", "Attached static file");
            _localizationService.AddOrUpdatePluginLocaleResource("admin.contentmanagement.messagetemplates.fields.attacheddownload.exists", "Has attached file");


            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.Priority", "Priority");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.ToCustomerId", "To Customer Id");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.DeviceTypeId", "Device Type Id");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.DeviceType", "Device Type");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.SubscriptionId", "Subscription Id");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.ToCustomerName", "To Customer Name");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.GroupId", "Group Id");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.GroupName", "Group Name");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.Subject", "Subject");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.Message", "Message");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.CreatedOnUtc", "Created On Utc");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.SentTries", "SentTries");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.SentOnUtc", "Sent On Utc");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.IsSent", "Is Sent");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.ErrorLog", "Error Log");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.NotificationTypeId", "Notification Type Id");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.NotificationType", "Notification Type");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.ItemId", "Item Id");

            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.Priority.Hint.Hint", "Priority");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.ToCustomerId.Hint", "To Customer Id");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.DeviceTypeId.Hint", "Device Type Id");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.DeviceType.Hint", "Device Type");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.SubscriptionId.Hint", "Subscription Id");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.ToCustomerName.Hint", "To Customer Name");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.GroupId.Hint", "Group Id");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.GroupName.Hint", "Group Name");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.Subject.Hint", "Subject");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.Message.Hint", "Message");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.CreatedOnUtc.Hint", "Created On Utc");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.SentTries.Hint", "SentTries");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.SentOnUtc.Hint", "Sent On Utc");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.IsSent.Hint", "Is Sent");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.ErrorLog.Hint", "Error Log");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.NotificationTypeId.Hint", "Notification Type Id");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.NotificationType.Hint", "Notification Type");
            _localizationService.AddOrUpdatePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.ItemId.Hint", "Item Id");

            _localizationService.AddOrUpdatePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Configure", "Configure");
            #endregion

            _objectContext.Install();

            InstallScheduleTask();
            base.Install();

        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            #region Local Resources

            _localizationService.DeletePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Test.Push");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Group.List");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Group.List.Header");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Groups.Header.EditGroup");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Groups.Title.AddNew");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Groups.Header.AddNew");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Schedules.Manage");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Schedules.Header");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Schedule.Title.EditSchedule");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Schedule.Header.EditSchedule");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Schedule.Title.CreateSchedule");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Schedule.Header.CreateSchedule");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Queued.Manage");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Queued.Header");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Device.Manage");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Device.Header");

            _localizationService.DeletePluginLocaleResource("BS.Plugin.NopStation.MobileApp.AppleCertFileNameWithPath.Hint");
            _localizationService.DeletePluginLocaleResource("BS.Plugin.NopStation.MobileApp.ApplePassword.Hint");
            _localizationService.DeletePluginLocaleResource("BS.Plugin.NopStation.MobileApp.IsAppleProductionMode.Hint");
            _localizationService.DeletePluginLocaleResource("BS.Plugin.NopStation.MobileApp.GoogleProject_Number.Hint");
            _localizationService.DeletePluginLocaleResource("BS.Plugin.NopStation.MobileApp.GoogleConsoleAPIAccess_KEY.Hint");

            _localizationService.DeletePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Common.BackToList");
            _localizationService.DeletePluginLocaleResource("Admin.Plugin.BsNotification.Groups.Fields.Name");

            _localizationService.DeletePluginLocaleResource("BS.Plugin.NopStation.MobileApp.AppleCertFileNameWithPath");
            _localizationService.DeletePluginLocaleResource("BS.Plugin.NopStation.MobileApp.ApplePassword");
            _localizationService.DeletePluginLocaleResource("BS.Plugin.NopStation.MobileApp.IsAppleProductionMode");
            _localizationService.DeletePluginLocaleResource("BS.Plugin.NopStation.MobileApp.GoogleProject_Number");
            _localizationService.DeletePluginLocaleResource("BS.Plugin.NopStation.MobileApp.GoogleConsoleAPIAccess_KEY");

            _localizationService.DeletePluginLocaleResource("Admin.ContentManagement.MessageTemplates.Fields.AttachedDownload");
            _localizationService.DeletePluginLocaleResource("admin.contentmanagement.messagetemplates.fields.attacheddownload.exists");

            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.Priority");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.ToCustomerId");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.DeviceTypeId");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.DeviceType");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.SubscriptionId");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.ToCustomerName");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.GroupId");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.GroupName");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.Subject");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.Message");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.CreatedOnUtc");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.SentTries");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.SentOnUtc");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.IsSent");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.ErrorLog");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.NotificationTypeId");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.NotificationType");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.ItemId");

            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.Priority.Hint");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.ToCustomerId.Hint");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.DeviceTypeId.Hint");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.DeviceType.Hint");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.SubscriptionId.Hint");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.ToCustomerName.Hint");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.GroupId.Hint");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.GroupName.Hint");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.Subject.Hint");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.Message.Hint");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.CreatedOnUtc.Hint");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.SentTries.Hint");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.SentOnUtc.Hint");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.IsSent.Hint");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.ErrorLog.Hint");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.NotificationTypeId.Hint");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.NotificationType.Hint");
            _localizationService.DeletePluginLocaleResource("Admin.MobileApp.QueuedNotificationModel.ItemId.Hint");

            _localizationService.DeletePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Configure");

            #endregion

            //_objectContext.Uninstall();
            _objectContext.Uninstall();
            UninstallScheduleTask();
            base.Uninstall();
        }

        public bool Authenticate()
        {
            return true;
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menuItemBuilder = new SiteMapNode();
            //{
            //    Title = "Mobile Notification App",
            //    Visible = true,
            //    SystemName = "Mobile Notification App",
            //    IconClass = "fa-bell"
            //};

            if (_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
            {

                menuItemBuilder.Title = "Mobile Notification App";
                menuItemBuilder.Visible = true;
                menuItemBuilder.SystemName = "Mobile Notification App";
                menuItemBuilder.IconClass = "fa-bell";

                menuItemBuilder.ChildNodes.Add(new SiteMapNode
                {
                    Visible = true,
                    Title = "Configure",
                    Url = "/Admin/BsNotificationAdmin/Configure",
                    SystemName = "Mobile Notification App Configure",
                    IconClass = "fa-circle-o"
                });

                #region notification message template
                var messageTemplateNode = new SiteMapNode()
                {
                    Visible = true,
                    Title = "Message Template",
                    IconClass = "fa-circle-o"
                };
                messageTemplateNode.ChildNodes.Add(new SiteMapNode
                {
                    Visible = true,
                    Title = "Templates",
                    Url = "/Admin/BsNotificationMessageTemplate/List",
                    SystemName = "Mobile Notification App Message Templates",
                    IconClass = "fa-genderless"
                });
                messageTemplateNode.ChildNodes.Add(new SiteMapNode
                {
                    Visible = true,
                    Title = "Create Message Template",
                    Url = "/Admin/BsNotificationMessageTemplate/Create",
                    SystemName = "Mobile Notification App Create Message Template",
                    IconClass = "fa-genderless"
                });
                menuItemBuilder.ChildNodes.Add(messageTemplateNode);
                #endregion

                #region device
                var deviceNode = new SiteMapNode()
                {
                    Visible = true,
                    Title = "Customer Devices",
                    SystemName = "Mobile Notification App Customer Devices",
                    IconClass = "fa-circle-o"

                };
                deviceNode.ChildNodes.Add(new SiteMapNode
                {
                    Visible = true,
                    Title = "Device List",
                    Url = "/Admin/BsNotificationAdmin/Device",
                    SystemName = "Mobile Notification App Device List",
                    IconClass = "fa-genderless"
                });
                //deviceNode.ChildNodes.Add(new SiteMapNode
                //{
                //    Visible = true,
                //    Title = "Create Sample Device",
                //    Url = "/Admin/BsNotificationAdmin/CreateDevice",
                //    RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
                //});
                menuItemBuilder.ChildNodes.Add(deviceNode);
                #endregion

                #region queued
                var queuedNode = new SiteMapNode()
                {
                    Visible = true,
                    Title = "Queued Notification",
                    SystemName = "Mobile Notification App Queued Notification",
                    IconClass = "fa-circle-o"
                };
                queuedNode.ChildNodes.Add(new SiteMapNode
                {
                    Visible = true,
                    Title = "Queued List",
                    Url = "/Admin/BsNotificationAdmin/Queued",
                    SystemName = "Mobile Notification App Queued List",
                    IconClass = "fa-genderless"
                });

                menuItemBuilder.ChildNodes.Add(queuedNode);
                #endregion

                #region group
                var groupNode = new SiteMapNode()
                {
                    Visible = true,
                    Title = "Group",
                    SystemName = "Mobile Notification App Group",
                    IconClass = "fa-circle-o"
                };
                groupNode.ChildNodes.Add(new SiteMapNode
                {
                    Visible = true,
                    Title = "Customer Groups",
                    Url = "/Admin/BsNotificationAdmin/Group",
                    SystemName = "Mobile Notification App Customer Groups",
                    IconClass = "fa-genderless"
                });
                groupNode.ChildNodes.Add(new SiteMapNode
                {
                    Visible = true,
                    Title = "Create Group",
                    Url = "/Admin/BsNotificationAdmin/CreateGroup",
                    SystemName = "Mobile Notification App Create Group",
                    IconClass = "fa-genderless"
                });
                menuItemBuilder.ChildNodes.Add(groupNode);
                #endregion

                #region schedule
                var scheduleNode = new SiteMapNode()
                {
                    Visible = true,
                    SystemName = "Mobile Notification App Schedule",
                    Title = "Schedule",
                    IconClass = "fa-circle-o"
                };
                scheduleNode.ChildNodes.Add(new SiteMapNode
                {
                    Visible = true,
                    Title = "Schedules",
                    Url = "/Admin/BsNotificationAdmin/Schedule",
                    SystemName = "Mobile Notification App Schedules",
                    IconClass = "fa-genderless"
                });
                scheduleNode.ChildNodes.Add(new SiteMapNode
                {
                    Visible = true,
                    Title = "Create Schedule",
                    Url = "/Admin/BsNotificationAdmin/CreateSchedule",
                    SystemName = "Mobile Notification App Create Schedule",
                    IconClass = "fa-genderless"
                });
                menuItemBuilder.ChildNodes.Add(scheduleNode);
                #endregion

                #region test push

                menuItemBuilder.ChildNodes.Add(new SiteMapNode
                {
                    Visible = true,
                    Title = "Test Push",
                    Url = "/Admin/BsNotificationAdmin/TestPushNotification",
                    SystemName = "Mobile Notification App Test Push",
                    IconClass = "fa-circle-o"
                });
                #endregion
            }

            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Nop Station");
            if (pluginNode != null)
            {
                pluginNode.ChildNodes.Add(menuItemBuilder);
            }
            else
            {
                var nopStation = new SiteMapNode()
                {
                    Visible = true,
                    Title = "Nop Station",
                    Url = "",
                    SystemName = "Nop Station",
                    IconClass = "fa-folder-o "
                };
                rootNode.ChildNodes.Add(nopStation);
                nopStation.ChildNodes.Add(menuItemBuilder);
            }
        }
    }
}

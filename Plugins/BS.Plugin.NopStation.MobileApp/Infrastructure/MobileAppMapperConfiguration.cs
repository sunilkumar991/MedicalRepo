using AutoMapper;
using BS.Plugin.NopStation.MobileApp.Domain;
using BS.Plugin.NopStation.MobileApp.Models;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using Nop.Core.Infrastructure.Mapper;
using System;

namespace BS.Plugin.NopStation.MobileApp.Infrastructure
{
    public class MobileAppMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public MobileAppMapperConfiguration()
        {
            #region AutoMapperStartupTask
            //message template
            CreateMap<NotificationMessageTemplate, NotificationMessageTemplateModel>()
                .ForMember(dest => dest.AllowedTokens, mo => mo.Ignore())
                .ForMember(dest => dest.HasAttachedDownload, mo => mo.Ignore())
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                .ForMember(dest => dest.ListOfStores, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            CreateMap<NotificationMessageTemplateModel, NotificationMessageTemplate>();
            //schedule
            CreateMap<ScheduledNotification, ScheduledNotificationModel>()
                .ForMember(dest => dest.GroupName, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableGroups, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableMessageTemplates, mo => mo.Ignore())
                .ForMember(dest => dest.NotificationType, mo => mo.Ignore());

            CreateMap<ScheduledNotificationModel, ScheduledNotification>()
                .ForMember(dest => dest.NotificationType, mo => mo.Ignore());

            //queued notification
            CreateMap<QueuedNotification, QueuedNotificationModel>()
                .ForMember(dest => dest.GroupName, mo => mo.Ignore())
                .ForMember(dest => dest.ToCustomerName, mo => mo.Ignore())
                .ForMember(dest => dest.DeviceType, mo => mo.Ignore())
                .ForMember(dest => dest.NotificationType, mo => mo.Ignore());

            CreateMap<QueuedNotificationModel, QueuedNotification>()
                .ForMember(dest => dest.DeviceType, mo => mo.Ignore())
                .ForMember(dest => dest.NotificationType, mo => mo.Ignore());
            //device
            CreateMap<Device, DeviceModel>()
                .ForMember(dest => dest.DeviceType, mo => mo.Ignore())
                .ForMember(dest => dest.CustomerName, mo => mo.Ignore());

            CreateMap<DeviceModel, Device>();
            #endregion
        }

        /// <summary>
        /// Order of this mapper implementation
        /// </summary>
        public int Order => 1;
    }
}

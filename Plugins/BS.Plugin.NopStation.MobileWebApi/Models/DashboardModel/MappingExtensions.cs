﻿using System;
using AutoMapper;
using Nop.Core;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Services.Seo;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel
{
    public static class MappingExtensions
    {
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }

        #region Topics

        public static TopicModel ToModel(this BS_ContentManagement entity)
        {
            return entity.MapTo<BS_ContentManagement, TopicModel>();
        }

        public static BS_ContentManagement ToEntity(this TopicModel model)
        {
            return model.MapTo<TopicModel, BS_ContentManagement>();
        }

        public static BS_ContentManagement ToEntity(this TopicModel model, BS_ContentManagement destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Stores

        public static StoreModel ToModel(this Store entity)
        {
            return entity.MapTo<Store, StoreModel>();
        }

        public static Store ToEntity(this StoreModel model)
        {
            return model.MapTo<StoreModel, Store>();
        }

        public static Store ToEntity(this StoreModel model, Store destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Templates

        public static ContentManagementModel ToModel(this BS_ContentManagementTemplate entity)
        {
            return entity.MapTo<BS_ContentManagementTemplate, ContentManagementModel>();
        }

        public static BS_ContentManagementTemplate ToEntity(this ContentManagementModel model)
        {
            return model.MapTo<ContentManagementModel, BS_ContentManagementTemplate>();
        }

        public static BS_ContentManagementTemplate ToEntity(this ContentManagementModel model, BS_ContentManagementTemplate destination)
        {
            return model.MapTo(destination);
        }

        public static BS_CategoryIcons ToEntity(this CategoryIconsModel model)
        {
            return model.MapTo<CategoryIconsModel, BS_CategoryIcons>();
        }

        #endregion

        #region Topics

        public static ProductModel ToModel(this Nop.Core.Domain.Catalog.Product entity)
        {
            return entity.MapTo<Nop.Core.Domain.Catalog.Product, ProductModel>();
        }
        #endregion

    }
}
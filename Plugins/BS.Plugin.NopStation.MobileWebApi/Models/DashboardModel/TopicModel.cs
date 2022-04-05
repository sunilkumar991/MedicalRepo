﻿using System.Collections.Generic;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using BS.Plugin.NopStation.MobileWebApi.Validators.Stores;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel
{
    [Validator(typeof(TopicValidator))]
    public partial class TopicModel : BaseNopEntityModel, ILocalizedModel<TopicLocalizedModel>
    {
        public TopicModel()
        {
            AvailableTopicTemplates = new List<SelectListItem>();
            Locales = new List<TopicLocalizedModel>();
            AvailableStores = new List<StoreModel>();
        }

        //Store mapping
        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.LimitedToStores")]
        public bool LimitedToStores { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.AvailableStores")]
        public List<StoreModel> AvailableStores { get; set; }
        public int[] SelectedStoreIds { get; set; }


        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.SystemName")]
      
        public string SystemName { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInSitemap")]
        public bool IncludeInSitemap { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInTopMenu")]
        public bool IncludeInTopMenu { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn1")]
        public bool IncludeInFooterColumn1 { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn2")]
        public bool IncludeInFooterColumn2 { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn3")]
        public bool IncludeInFooterColumn3 { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.AccessibleWhenStoreClosed")]
        public bool AccessibleWhenStoreClosed { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.IsPasswordProtected")]
        public bool IsPasswordProtected { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.Password")]
        public string Password { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.URL")]
       
        public string Url { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.Title")]
       
        public string Title { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.Body")]
      
        public string Body { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.TopicTemplate")]
        public int TopicTemplateId { get; set; }
        public IList<SelectListItem> AvailableTopicTemplates { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaKeywords")]
       
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaDescription")]
      
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaTitle")]
        
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.SeName")]
       
        public string SeName { get; set; }
        
        public IList<TopicLocalizedModel> Locales { get; set; }
    }

    public partial class TopicLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.Title")]
       
        public string Title { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.Body")]
  
        public string Body { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaKeywords")]
  
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaDescription")]
      
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaTitle")]
      
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Topics.Fields.SeName")]
  
        public string SeName { get; set; }
    }
}
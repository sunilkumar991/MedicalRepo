using System;
using System.Linq;
using System.Text;
using Nop.Core.Domain.Messages;
using BS.Plugin.NopStation.MobileApp.Domain;
using BS.Plugin.NopStation.MobileApp.Extensions;
using BS.Plugin.NopStation.MobileApp.Models;
using BS.Plugin.NopStation.MobileApp.Services;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Messages;
using Nop.Core;
using Nop.Web.Areas.Admin.Models.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;

namespace BS.Plugin.NopStation.MobileApp.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public partial class BsNotificationMessageTemplateController : BasePluginController
    {
        #region Fields

        private readonly INotificationMessageTemplateService _notificationMessageTemplateService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IStoreContext _storeContext;

        #endregion Fields

        #region Constructors

        public BsNotificationMessageTemplateController(INotificationMessageTemplateService messageTemplateService, 
            IEmailAccountService emailAccountService,
            ILanguageService languageService, 
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService, 
            IMessageTokenProvider messageTokenProvider, 
            IPermissionService permissionService,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IWorkflowMessageService workflowMessageService,
            EmailAccountSettings emailAccountSettings,
            IStoreContext storeContext)
        {
            this._notificationMessageTemplateService = messageTemplateService;
            this._emailAccountService = emailAccountService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._messageTokenProvider = messageTokenProvider;
            this._permissionService = permissionService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._workflowMessageService = workflowMessageService;
            this._emailAccountSettings = emailAccountSettings;
            this._storeContext = storeContext;
        }

        #endregion
        
        #region Utilities

        /// <summary>
        /// Save selected TAB index
        /// </summary>
        /// <param name="index">Idnex to save; null to automatically detect it</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        protected void SaveSelectedTabIndex(int? index = null, bool persistForTheNextRequest = true)
        {
            //keep this method synchronized with
            //"GetSelectedTabIndex" method of \Nop.Web.Framework\ViewEngines\Razor\WebViewPage.cs
            if (!index.HasValue)
            {
                int tmp;
                if (int.TryParse(this.Request.Form["selected-tab-index"], out tmp))
                {
                    index = tmp;
                }
            }
            if (index.HasValue)
            {
                string dataKey = "nop.selected-tab-index";
                if (persistForTheNextRequest)
                {
                    TempData[dataKey] = index;
                }
                else
                {
                    ViewData[dataKey] = index;
                }
            }
        }


        private string FormatTokens(string[] tokens)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];
                sb.Append(token);
                if (i != tokens.Length - 1)
                    sb.Append(", ");
            }

            return sb.ToString();
        }

        [NonAction]
        protected virtual void UpdateLocales(NotificationMessageTemplate mt, NotificationMessageTemplateModel model)
        {
            foreach (var localized in model.Locales)
            {
             
                _localizedEntityService.SaveLocalizedValue(mt,
                                                           x => x.Subject,
                                                           localized.Subject,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(mt,
                                                           x => x.Body,
                                                           localized.Body,
                                                           localized.LanguageId);

                }
        }


        [NonAction]
        protected virtual void PrepareStoresMappingModel(NotificationMessageTemplateModel model, NotificationMessageTemplate messageTemplate, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableStores = _storeService
                .GetAllStores()
                .Select(s => s.ToModel<StoreModel>())
                .ToList();
            if (!excludeProperties)
            {
                if (messageTemplate != null)
                {
                    model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(messageTemplate);
                }
            }
        }

        [NonAction]
        protected virtual void SaveStoreMappings(NotificationMessageTemplate messageTemplate, NotificationMessageTemplateModel model)
        {
            var existingStoreMappings = _storeMappingService.GetStoreMappings(messageTemplate);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds != null && model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(messageTemplate, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        #endregion
        
        #region Methods

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return Challenge();

            var model = new NotificationMessageTemplateListModel();
            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            
            return View("~/Plugins/NopStation.MobileApp/Views/BsNotificationMessageTemplate/List.cshtml", model);
        }

        [HttpPost]
        public IActionResult List(DataSourceRequest command, MessageTemplateListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return Challenge();

            var messageTemplates = _notificationMessageTemplateService.GetAllNotificationMessageTemplates(_storeContext.ActiveStoreScopeConfiguration);
            //var gridModel = new DataSourceResult
            //{
            //    Data = messageTemplates.Select(x =>
            //    {
            //        var templateModel = x.ToModel();
            //        PrepareStoresMappingModel(templateModel, x, false);
            //        var stores = _storeService
            //                .GetAllStores()
            //                .Where(s => !x.LimitedToStores || templateModel.SelectedStoreIds.Contains(s.Id))
            //                .ToList();
            //        for (int i = 0; i < stores.Count; i++)
            //        {
            //            templateModel.ListOfStores += stores[i].Name;
            //            if (i != stores.Count - 1)
            //                templateModel.ListOfStores += ", ";
            //        }
            //        return templateModel;
            //    }),
            //    Total = messageTemplates.Count
            //};

            // Added by Sunil Kumar S 28-04-2020
            var gridModel = new DataSourceResult
            {
                Data = messageTemplates.Select(d => new MessageTemplateModel()
                {
                    Id=d.Id,
                    Name = d.Name,
                    Subject = d.Subject,
                    Body = d.Body,
                    IsActive = d.IsActive,
                    AttachedDownloadId = d.AttachedDownloadId
                  
                }),
                Total = messageTemplates.Count
            };


            return Json(gridModel);
        }


        public IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return Challenge();



            var model = new NotificationMessageTemplateModel();
            model.HasAttachedDownload = model.AttachedDownloadId > 0;
            model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfAllowedTokens().ToArray());

            //Store
            PrepareStoresMappingModel(model, null, false);
           

            return View("~/Plugins/NopStation.MobileApp/Views/BsNotificationMessageTemplate/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public IActionResult Create(NotificationMessageTemplateModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return Challenge();

            if (ModelState.IsValid)
            {
               //var  messageTemplate = model.ToEntity();
                var notimessageTemplate = new NotificationMessageTemplate();

                notimessageTemplate.Name = model.Name;
                notimessageTemplate.Subject = model.Subject;
                notimessageTemplate.Body = model.Body;
                notimessageTemplate.IsActive = model.IsActive;
                notimessageTemplate.LimitedToStores = model.LimitedToStores;

                //attached file
                if (!model.HasAttachedDownload)
                    notimessageTemplate.AttachedDownloadId = 0;
                else
                    notimessageTemplate.AttachedDownloadId = model.AttachedDownloadId;
                _notificationMessageTemplateService.InsertNotificationMessageTemplate(notimessageTemplate);
               //Stores
               SaveStoreMappings(notimessageTemplate, model);
               //locales
               UpdateLocales(notimessageTemplate, model);

               SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Updated"));

               if (continueEditing)
               {
                   //selected tab
                   SaveSelectedTabIndex();

                   return RedirectToAction("Edit", new { id = notimessageTemplate.Id, area = "Admin" });
               }
                return RedirectToAction("List",new {area="Admin"});
            }


            //If we got this far, something failed, redisplay form
            model.HasAttachedDownload = model.AttachedDownloadId > 0;
            model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfAllowedTokens().ToArray());
            //Store
            PrepareStoresMappingModel(model, null, true);
            return View(model);
        }

     

        public IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return Challenge();

            var messageTemplate = _notificationMessageTemplateService.GetNotificationMessageTemplateById(id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("List",new {area= "Admin" });

            //var model = messageTemplate.ToModel();

            var model = messageTemplate.ToNotificationMessageTemplateModel();

            //var messageTemplateModel = new NotificationMessageTemplateModel();

            //messageTemplateModel.Name = messageTemplate.Name ;
            //messageTemplateModel.Subject = messageTemplate.Subject;
            //messageTemplateModel.Body = messageTemplate.Body ;

            //messageTemplateModel.IsActive = messageTemplate.IsActive;

            //messageTemplateModel.AttachedDownloadId = messageTemplateModel.AttachedDownloadId;
            //messageTemplateModel.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfAllowedTokens().ToArray());


            model.HasAttachedDownload = model.AttachedDownloadId > 0;
            model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfAllowedTokens().ToArray());

            //Store
            PrepareStoresMappingModel(model, messageTemplate, false);
        ////locales
        //AddLocales(_languageService, model.Locales, (locale, languageId) =>
        //{

        //    locale.Subject = messageTemplate.GetLocalized(x => x.Subject, languageId, false, false);
        //    locale.Body = messageTemplate.GetLocalized(x => x.Body, languageId, false, false);
        //});


            return View("~/Plugins/NopStation.MobileApp/Views/BsNotificationMessageTemplate/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public IActionResult Edit(NotificationMessageTemplateModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return Challenge();

            var messageTemplate = _notificationMessageTemplateService.GetNotificationMessageTemplateById(model.Id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("List", new { area = "Admin" });
            
            if (ModelState.IsValid)
            {
                
                     messageTemplate = model.ToNotificationMessageEntity();
               // messageTemplate = model.ToEntity(messageTemplate);
                //attached file
                if (!model.HasAttachedDownload)
                    messageTemplate.AttachedDownloadId = 0;
                _notificationMessageTemplateService.UpdateNotificationMessageTemplate(messageTemplate);
                //Stores
                SaveStoreMappings(messageTemplate, model);
                //locales
                UpdateLocales(messageTemplate, model);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Updated"));
                
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabIndex();

                    return RedirectToAction("Edit", new { id = messageTemplate.Id, area = "Admin" });
                }
                return RedirectToAction("List", new { area = "Admin" });
            }


            //If we got this far, something failed, redisplay form
            model.HasAttachedDownload = model.AttachedDownloadId > 0;
            model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfAllowedTokens().ToArray());
          
            //Store
            PrepareStoresMappingModel(model, messageTemplate, true);
            return View("~/Plugins/NopStation.MobileApp/Views/BsNotificationMessageTemplate/Edit.cshtml", model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("message-template-delete")]
        public IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return Challenge();

            var messageTemplate = _notificationMessageTemplateService.GetNotificationMessageTemplateById(id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("List", new { area = "Admin" });

            _notificationMessageTemplateService.DeleteNotificationMessageTemplate(messageTemplate);
            
            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Deleted"));
            return RedirectToAction("List", new { area = "Admin" });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("message-template-copy")]
        public IActionResult CopyTemplate(NotificationMessageTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return Challenge();

            var messageTemplate = _notificationMessageTemplateService.GetNotificationMessageTemplateById(model.Id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("List", new { area = "Admin" });

            try
            {
                var newMessageTemplate = _notificationMessageTemplateService.CopyNotificationMessageTemplate(messageTemplate);
                SuccessNotification("The message template has been copied successfully");
                return RedirectToAction("Edit", new { id = newMessageTemplate.Id, area = "Admin" });
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = model.Id, area = "Admin" });
            }
        }

        #endregion
    }
}

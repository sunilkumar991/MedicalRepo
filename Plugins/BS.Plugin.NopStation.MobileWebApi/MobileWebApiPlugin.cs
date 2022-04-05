using System.Collections.Generic;
using System.IO;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Plugins;
using BS.Plugin.NopStation.MobileWebApi.Data;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Common;
using Nop.Services.Vendors;
using Nop.Web.Framework.Menu;
using BS.Plugin.NopStation.MobileWebApi.Services;
using System.Linq;
using BS.Plugin.NopStation.MobileWebApi.Models.NstSettingsModel;
using Microsoft.AspNetCore.Routing;
using Nop.Services.Security;

namespace BS.Plugin.NopStation.MobileWebApi
{
    /// <summary>
    /// PLugin
    /// </summary>
    public class MobileWebApiPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin //, IWidgetPlugin
    {
        #region Fields

        private readonly IBsNopMobilePluginService _BsPluginService;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        //private readonly MobileWebApiObjectContext _objectContext;
        private readonly IContentManagementTemplateService _contentmanagementTemplate;
        private readonly MobileWebApiObjectContext _context;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor
        public MobileWebApiPlugin(IBsNopMobilePluginService BsPluginService,
            ISettingService settingService,
            IWorkContext workContext,
            MobileWebApiSettings webapiSettings,
             IContentManagementTemplateService contentmanagementTemplate,
             MobileWebApiObjectContext context,
             ILocalizationService localizationService,
             IPermissionService permissionService)
        {
            this._BsPluginService = BsPluginService;
            this._settingService = settingService;
            this._workContext = workContext;
            this._contentmanagementTemplate = contentmanagementTemplate;
            this._context = context;
            _localizationService = localizationService;
            _permissionService = permissionService;
        }

        #endregion

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode()
            {
                Title = "Mobile Web Api",
                Visible = true,
                IconClass = "fa-dot-circle-o",
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };

            var categoryIconItem = new SiteMapNode()
            {
                Title = "Category Icons",
                ControllerName = "MobileWebApiConfiguration",
                ActionName = "CategoryIcons",
                Visible = true,
                IconClass = "fa-genderless",
                SystemName = "CategoryIcons",
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };
            menuItem.ChildNodes.Add(categoryIconItem);

            var sliderInfoItem = new SiteMapNode()
            {
                Title = "Slider",
                ControllerName = "MobileWebApiConfiguration",
                ActionName = "SliderImage",
                Visible = true,
                IconClass = "fa-genderless",
                SystemName = "SliderImage",
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };
            menuItem.ChildNodes.Add(sliderInfoItem);

            var nstSettings = new SiteMapNode()
            {
                Title = "NST Settings",
                ControllerName = "MobileWebApiConfiguration",
                ActionName = "NopStationSecrateToken",
                SystemName = "NopStationSecrateToken",
                Visible = true,
                IconClass = "fa-genderless",

                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };
            menuItem.ChildNodes.Add(nstSettings);

            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "nopStation");
            if (pluginNode != null)
                pluginNode.ChildNodes.Add(menuItem);

            else
            {
                if (_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                {
                    var nopStation = new SiteMapNode()
                    {
                        Visible = true,
                        Title = "nopStation",
                        Url = "",
                        SystemName = "nopStation",
                        IconClass = "fa-folder"
                    };
                    rootNode.ChildNodes.Add(nopStation);
                    nopStation.ChildNodes.Add(menuItem);
                }
            }
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "GeneralSetting";
            controllerName = "MobileWebApiConfiguration";
            routeValues = new RouteValueDictionary() { { "Namespaces", "BS.Plugin.NopStation.MobileWebApi.Controllers" }, { "area", null } };
        }

        public IList<string> GetWidgetZones()
        {
            return new List<string>() { "admin_webapi_bottom" };

        }
        public bool Authenticate()
        {
            return true;
        }


        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Dashboard";
            controllerName = "MobileWebApiConfiguration";
            routeValues = new RouteValueDictionary()
            {
                {"Namespaces", "BS.Plugin.NopStation.MobileWebApi.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }

        public override void Install()
        {
            //settings
            var settings = new MobileWebApiSettings()
            {
                //ProductPictureSize = 125,
                //PassShippingInfo = false,
                //StaticFileName = string.Format("froogle_{0}.xml", CommonHelper.GenerateRandomDigitCode(10)),
            };
            _settingService.SaveSetting(settings);

            //_settingService.SaveSetting(settings);
            var nstsettings = new NstSettingsModel()
            {
                NST_KEY = "bm9wU3RhdGlvblRva2Vu",
                NST_SECRET = "bm9wS2V5"
            };
            _settingService.SaveSetting(nstsettings);

            #region Local Resources

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.NopMobilemenuMainTitle", "NopMobileWebApiConfigration");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Willusedefaultnopcategory", "Will use default nopcategory?");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.DefaultNopFlowSameAs", "Will use default nopflow same as?");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.SubCategory", "Select a Sub Category");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.SubCategory.Hint", "Select a Sub Category");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopMobile.WebApi.NST", "NST Settings");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.SubCategory.Hint", "Select a Sub Category");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.CategoryIcon", "Upload Category Icon");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.CategoryIcon.Hint", "Upload Category Icon");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.ActivatePushNotification", "Activate push notification");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.SandboxMode", "Sandbox mode");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.GoogleApiProjectNumber", "Google api project number");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.UploudeIOSPEMFile", "Uploude IOS PEMFile");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.PEMPassword", "PEM password");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppNameOnGooglePlayStore", "App name on google playstore");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppUrlOnGooglePlayStore", "App URL on google playstore");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppNameOnAppleStore", "App name on Applestore");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppUrlonAppleStore", "App URL on Applestore");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppDescription", "App description");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppImage", "App image");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppLogo", "App Logo");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppLogoAltText", "App Logo Alternate Text");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.GcmApiKey", "Gcm Apikey");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppKey", "AppKey");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppName", "App name");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.LicenceType", "Licence type");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.CreatedDate", "Created date");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AndroidAppStatus", "Android app status");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.DownloadUrl", "Download url");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.iOsAPPUDID", "Ios APPUDID");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.MobilWebsiteURL", "Mobil website URL");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.PushNotificationHeading", "Push notification heading");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.PushNotificationMessage", "Push notification message");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.HeaderBackgroundColor", "Header background color");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.HeaderFontandIconColor", "Header font and icon color");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.HighlightedTextColor", "High lighted text color");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.PrimaryTextColor", "Primary text color");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.SecondaryTextColor", "Secondary text color");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.BackgroundColorofPrimaryButton", "Background color of primary button");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.TextColorofPrimaryButton", "Text color of primary Button");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.BackgroundColorofSecondaryButton", "Background color of secondary button");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.menu", "NopMobile Menu");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.overview", "Overview");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.generalsetting", "General Settings");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.themepersonalization", "Theme Personalization");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.bannericon", "Banner Slide");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.contantmanegement", "Content Management");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.pushnotification", "Push Notification");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.categoryicon", "Category Icon");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.mobilewebsitesetting", "Mobile Website Settings");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.HeaderBackgroundColorHint", "Configure background color for heaer of the app");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.HeaderFontandIconColorHint", "Configure font and icon color for heaer of the app");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.HighlightedTextColorHint", "Configure font color for highlighted text message such warning,product,price etc.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.PrimaryTextColorHint", "Configure font color for primary text labels such as product name,category name etc.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.SecondaryTextColorHint", "Configure font color for secondary text labels such as option name,no. of rating etc.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.BackgroundColorofPrimaryButtonHint", "Configure background color for all primary button such as Add to Cart etc");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.TextColorofPrimaryButtonHint", "Configure font color for all primary button such as Add to Cart etc");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.BackgroundColorofSecondaryButtonHint", "Configure background color for all secondary button");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.persionalizewebsiteTitle", "Personalization [Website]");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.categoryiconTitle", "Configure Category Icon");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.configureTitle", "Configure Plugin");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.contentmanagementTitle", "ConfigureContent Management");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.bannerconfigureTitle", "Configure Content Banner");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.mobilewebsitesettingTitle", "Configure Mobile Settings");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.pushnotificationTitle", "Configure Push Notification");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture1", "Picture 1");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture2", "Picture 2");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture3", "Picture 3");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture4", "Picture 4");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture5", "Picture 5");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture", "Picture");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture.Hint", "Upload picture.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Text", "Comment");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Text.Hint", "Enter comment for picture. Leave empty if you don't want to display any text.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Link", "URL");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Link.Hint", "Enter URL. Leave empty if you don't want this picture to be clickable.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.IsProduct", "Is Product");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.IsProduct.Hint", "Checked, if you enter Product Id or Unchecked for Category Id.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.ProdOrCat", "Product or Category Id");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.ProdOrCat.Hint", "Enter your product or category id");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.EnableBestseller", "Enable Bestseller Home Page");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.EnableFeaturedProducts", "Enable Featured Products Home Page");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.EnableNewProducts", "Enable New Products Home Page");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AddNew", "Add New FeturedProduct");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.sliderImage", "Slider Image");
            _localizationService.AddOrUpdatePluginLocaleResource("Slider.Products.Fields.StartDate", "Start Date");
            _localizationService.AddOrUpdatePluginLocaleResource("Slider.Products.Fields.StartDate.Hint", "Start Date");
            _localizationService.AddOrUpdatePluginLocaleResource("Slider.Products.Fields.EndDate", "End Date");
            _localizationService.AddOrUpdatePluginLocaleResource("Slider.Products.Fields.EndDate.Hint", "End Date");
            _localizationService.AddOrUpdatePluginLocaleResource("Slider.Products.Fields.IsProduct", "Is Product");
            _localizationService.AddOrUpdatePluginLocaleResource("Slider.Products.Fields.IsProduct.Hint", "Is Product");
            _localizationService.AddOrUpdatePluginLocaleResource("Slider.Products.Fields.ProductOrCategory", "Product or Category");
            _localizationService.AddOrUpdatePluginLocaleResource("Slider.Products.Fields.ProductOrCategory.Hint", "Product or Category");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopMobile.WebApi.Slider", "Configure Slider Image");
            _localizationService.AddOrUpdatePluginLocaleResource("admin.bsslider.fields.slideractivestartdate", "Slider Active Start Date");
            _localizationService.AddOrUpdatePluginLocaleResource("admin.bsslider.fields.slideractiveenddate", "Slider Active End Date");
            _localizationService.AddOrUpdatePluginLocaleResource("admin.bsslider.fields.isproduct", "Is Product");
            _localizationService.AddOrUpdatePluginLocaleResource("admin.bsslider.fields.productorcategory", "Product Or Category");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.NstSettingsModel.NST_KEY", "NST Key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.NstSettingsModel.NST_KEY.Hint", "NST Key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.NstSettingsModel.NST_SECRET", "NST Secret");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.NstSettingsModel.NST_SECRET.Hint", "NST Secret");
            #endregion

            //install db
            _context.Install();

            //base install
            base.Install();
        }
        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            #region Local Resources

            //settings

            _settingService.DeleteSetting<MobileWebApiSettings>();

            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.NopMobilemenuMainTitle");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Willusedefaultnopcategory");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.SubCategory");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.CategoryIcon");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.DefaultNopFlowSameAs");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.ActivatePushNotification");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.SandboxMode");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.GoogleApiProjectNumber");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.UploudeIOSPEMFile");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.PEMPassword");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppNameOnGooglePlayStore");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppUrlOnGooglePlayStore");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppNameOnAppleStore");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppUrlonAppleStore");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppDescription");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppImage");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppLogo");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.GcmApiKey");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppKey");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppName");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.LicenceType");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.CreatedDate");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AndroidAppStatus");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.DownloadUrl");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.iOsAPPUDID");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.MobilWebsiteURL");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.PushNotificationHeading");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.PushNotificationMessage");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.HeaderBackgroundColor");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.HeaderFontandIconColor");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.HighlightedTextColor");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.PrimaryTextColor");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.SecondaryTextColor");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.BackgroundColorofPrimaryButton");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.TextColorofPrimaryButton");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.BackgroundColorofSecondaryButton");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.menu");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.overview");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.generalsetting");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.bannericon");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.contantmanegement");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.pushnotification");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.categoryicon");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.mobilewebsitesetting");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.HeaderBackgroundColorHint");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.HeaderFontandIconColorHint");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.HighlightedTextColorHint");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.PrimaryTextColorHint");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.SecondaryTextColorHint");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.BackgroundColorofPrimaryButtonHint");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.TextColorofPrimaryButtonHint");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.BackgroundColorofSecondaryButtonHint");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.persionalizewebsiteTitle");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.categoryiconTitle");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.configureTitle");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.contentmanagementTitle");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.mobilewebsitesettingTitle");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.pushnotificationTitle");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture1");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture2");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture3");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture4");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture5");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Text");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Text.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Link");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Link.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.IsProduct");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.IsProduct.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.ProdOrCat");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.ProdOrCat.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.bannerconfigureTitle");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.EnableBestseller");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.EnableFeaturedProducts");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.EnableNewProducts");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AddNew");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppLogoAltText");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.sliderImage");
            _localizationService.DeletePluginLocaleResource("Slider.Products.Fields.StartDate");
            _localizationService.DeletePluginLocaleResource("Slider.Products.Fields.EndDate");
            _localizationService.DeletePluginLocaleResource("Slider.Products.Fields.IsProduct");
            _localizationService.DeletePluginLocaleResource("Slider.Products.Fields.ProductOrCategory");
            _localizationService.DeletePluginLocaleResource("Plugins.NopMobile.WebApi.Slider");
            _localizationService.DeletePluginLocaleResource("admin.bsslider.fields.slideractivestartdate");
            _localizationService.DeletePluginLocaleResource("admin.bsslider.fields.slideractiveenddate");
            _localizationService.DeletePluginLocaleResource("admin.bsslider.fields.isproduct");
            _localizationService.DeletePluginLocaleResource("admin.bsslider.fields.productorcategory");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.NstSettingsModel.NST_KEY");
            _localizationService.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.NstSettingsModel.NST_SECRET");
            #endregion

            _context.Uninstall();
            base.Uninstall();
        }
    }
}

using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Banner;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Services.Plugins;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    //[Route("api/[controller]")]
    public class BannerController : BaseApiController
    {

        #region Field
        private readonly IPluginFinder _pluginFinder;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly IPictureService _pictureService;
        private readonly IBS_SliderService _bsSliderService;
        private readonly ICategoryService _categoryService;

        #endregion

        #region Ctor
        public BannerController(IPluginFinder pluginFinder,
            IStoreService storeService,
            IWorkContext workContext,
            ISettingService settingService,
            IPictureService pictureService,
            IBS_SliderService bsSliderService,
            ICategoryService categoryService)
        {
            _pluginFinder = pluginFinder;
            _storeService = storeService;
            _workContext = workContext;
            _settingService = settingService;
            _pictureService = pictureService;
            _bsSliderService = bsSliderService;
            _categoryService = categoryService;

        }
        #endregion

        #region Utility

        private HomePageBannerResponseModel.BannerModel GetPictureUrl(int pictureId)
        {
            string imageUrl = _pictureService.GetPictureUrl(pictureId, 300, showDefaultPicture: false);
            HomePageBannerResponseModel.BannerModel picture = new HomePageBannerResponseModel.BannerModel()
            {
                ImageUrl = imageUrl == string.Empty ? null : imageUrl
            };
            return picture;
        }
        #endregion

        #region Action Method
        [Route("api/homepagebanner")]
        [HttpGet]
        public IActionResult HomePageBanner()
        {
            HomePageBannerResponseModel result = new HomePageBannerResponseModel();
            //var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            //var nivoSliderSettings = _settingService.LoadSetting<BsNopMobileSettings>(storeScope);
            //int campaignType = type;
            List<Domain.BS_Slider> sliderDomainList = _bsSliderService.GetBSSliderImagesByDate();

            List<HomePageBannerResponseModel.BannerModel> pictureList = (from sliderDomain in sliderDomainList
                                                                         let picture = _pictureService.GetPictureById(sliderDomain.PictureId)
                                                                         select new HomePageBannerResponseModel.BannerModel
                                                                         {
                                                                             ImageUrl = string.IsNullOrEmpty(_pictureService.GetPictureUrl(picture)) ? "https://onestop-kitchen.com/images/thumbs/002/0025868.jpeg" : _pictureService.GetPictureUrl(picture),
                                                                             Text = "",
                                                                             Link = "",
                                                                             IsProduct = Convert.ToInt32(sliderDomain.IsProduct),
                                                                             ProdOrCatId = Convert.ToString(sliderDomain.ProdOrCatId),
                                                                             //Added by Sunil Kumar at 4/1/19
                                                                             CategoryName = Convert.ToString(_categoryService.GetCategoryNameById(sliderDomain.ProdOrCatId))
                                                                         }).ToList();
            result.IsEnabled = pictureList.Count > 0;

            result.Data = pictureList;

            return Ok(result);
        }
        #endregion
    }
}

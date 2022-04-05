﻿using FluentValidation.Attributes;
using Nop.Web.Framework.Mvc;
using BS.Plugin.NopStation.MobileWebApi.Validators.Stores;
using BS.Plugin.NopStation.MobileWebApi.Validators.Catalog;
using Nop.Web.Framework.Models;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel
{
     [Validator(typeof(BannerSliderValidator))]
    public class SliderModel : BaseNopModel
    {
        public string Picture1Url { get; set; }
        public string Text1 { get; set; }
        public string Link1 { get; set; }

        public string Picture2Url { get; set; }
        public string Text2 { get; set; }
        public string Link2 { get; set; }

        public string Picture3Url { get; set; }
        public string Text3 { get; set; }
        public string Link3 { get; set; }

        public string Picture4Url { get; set; }
        public string Text4 { get; set; }
        public string Link4 { get; set; }

        public string Picture5Url { get; set; }
        public string Text5 { get; set; }
        public string Link5 { get; set; }
    }
}
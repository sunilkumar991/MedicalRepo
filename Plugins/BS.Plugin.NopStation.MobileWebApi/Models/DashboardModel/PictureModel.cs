using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Models;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel
{
    public partial class PictureModel : BaseNopModel
    {
        //added by Sunil Kumar At 23-04-19
        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public string FullSizeImageUrl { get; set; }

        public string Title { get; set; }

        public string AlternateText { get; set; }
    }
}
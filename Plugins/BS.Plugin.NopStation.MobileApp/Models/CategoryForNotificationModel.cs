using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace BS.Plugin.NopStation.MobileApp.Models
{
    public class CategoryForNotificationModel
    {
        [NopResourceDisplayName("Admin.Catalog.Categories.List.SearchCategoryName")]

        public string SearchCategoryName { get; set; }

        public int ScheduleIdId { get; set; }

        public int[] SelectedCategoryIds { get; set; }
    }
}

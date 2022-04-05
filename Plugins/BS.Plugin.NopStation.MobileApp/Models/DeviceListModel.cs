using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace BS.Plugin.NopStation.MobileApp.Models
{
    public class DeviceListModel : BaseNopModel
    {
        public DeviceListModel()
        {
            AvailableDeviceTypes=new List<SelectListItem>();    
        }

        [NopResourceDisplayName("Admin.MobileApp.DeviceList.CustomerId")]
        public int CustomerId { get; set; }

        [NopResourceDisplayName("Admin.MobileApp.DeviceList.DeviceToken")]
        public string DeviceToken { get; set; }

        [NopResourceDisplayName("Admin.MobileApp.DeviceList.DeviceType")]
        public int DeviceType { get; set; }

        public IList<SelectListItem> AvailableDeviceTypes { get; set; }
    }
}
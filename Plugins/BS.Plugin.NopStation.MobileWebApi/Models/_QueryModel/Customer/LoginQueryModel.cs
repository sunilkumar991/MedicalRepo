using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Customer
{
    public class LoginQueryModel
    {
        public LoginQueryModel()
        {
            this.DeviceInfo = new DeviceInfo();
        }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string SimId { get; set; }
        public DeviceInfo DeviceInfo { get; set; }
    }
}

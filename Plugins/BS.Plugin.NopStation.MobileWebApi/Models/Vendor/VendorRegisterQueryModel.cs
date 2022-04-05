using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Vendor
{
    public partial class VendorRegisterQueryModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public int CityId { get; set; }
        public int StateId { get; set; }

        public string MobileNumber { get; set; }
        public string Password { get; set; }

        public string HouseNo { get; set; }
        public string FloorNo { get; set; }
        public string RoomNo { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public string ShopName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;

namespace BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Customer
{
    public partial class RegisterFromOKDollarQueryModel
    {
        public RegisterFromOKDollarQueryModel()
        {
            this.DeviceInfo = new DeviceInfo();
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Gender { get; set; }
        public int? DateOfBirthDay { get; set; }
        public int? DateofBirthMonth { get; set; }
        public int? DateOfBirthYear { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string OtherEmail { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public string DeviceID { get; set; }
        public string Simid { get; set; }
        public string Token { get; set; }
        public string Mode { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string ViberNumber { get; set; }
        public int MaritalStatus { get; set; }
        public bool DisplayAvatar { get; set; }
        public string HouseNo { get; set; }
        public string FloorNo { get; set; }
        public string RoomNo { get; set; }
        public string CountryCode { get; set; }
        public DeviceInfo DeviceInfo { get; set; }
        //added by Sunil Kumar At 29-04-19
        public int? VersionCode { get; set; }
        public DateTime? ParseDateOfBirth()
        {
            if (!DateOfBirthYear.HasValue || !DateofBirthMonth.HasValue || !DateOfBirthDay.HasValue)
                return null;

            DateTime? dateOfBirth = null;
            try
            {
                dateOfBirth = new DateTime(DateOfBirthYear.Value, DateofBirthMonth.Value, DateOfBirthDay.Value);
            }
            catch { }
            return dateOfBirth;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Customer
{
    public class ContactDetailsQueryModel
    {
        public string AgentNumber { get; set; }
        public Login Login { get; set; }
        public Contactdetails[] ContactDetailses { get; set; }
    }

    public class Login
    {
        public string MobileNumber { get; set; }
        public string Simid { get; set; }
        public string Msid { get; set; }
        public int Ostype { get; set; }
        public string Otp { get; set; }
    }

    public class Contactdetails
    {
        public string EmailID { get; set; }
        public string IsOkDollarAcc { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string PhotoUri { get; set; }
        public string VcardUri { get; set; }
        public int _id { get; set; }
        public bool expand { get; set; }
        public string isContactUpload { get; set; }
        public string isVerified { get; set; }
        public object[] mContactModel { get; set; }
        public int type { get; set; }
    }
}

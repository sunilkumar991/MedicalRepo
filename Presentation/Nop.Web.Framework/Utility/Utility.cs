using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Web.Framework.Utility
{
    public class UtilityJsonRequest
    {
        [JsonIgnore]
        private readonly Utility _utility;

        public UtilityJsonRequest(Utility utility)
        {
            _utility = utility;
        }

        [JsonProperty(PropertyName = "UtilityJsonRequest")]
        public Utility Utility
        {
            get { return _utility; }
        }
    }

    public class MerchantPayment
    {
        public List<Utility> Data { get; set; }   
    }

    public class Utility
    {

        /// <summary>
        /// Added Field
        /// </summary>
        /// 

        [JsonIgnore]
        private readonly string _firstname;
        [JsonIgnore]
        private readonly string _lastname;
        [JsonIgnore]
        private readonly string _email;
        [JsonIgnore]
        private readonly string _username;
        [JsonIgnore]
        private readonly string _gender;
        [JsonIgnore]
        private readonly string _dateofbirthday;
        [JsonIgnore]
        private readonly string _dateofbirthmonth;
        [JsonIgnore]
        private readonly string _dateofbirthyear;
        [JsonIgnore]
        private readonly string _address1;
        [JsonIgnore]
        private readonly string _address2;
        [JsonIgnore]
        private readonly string _city;
        [JsonIgnore]
        private readonly string _state;
        [JsonIgnore]
        private readonly string _country;
        [JsonIgnore]
        private readonly string _otheremail;
       
        //--------------------------------------------
        [JsonIgnore]
        private readonly string _mobileNumber;
        [JsonIgnore]
        private readonly string _password;
        [JsonIgnore]
        private readonly string _appId;
        [JsonIgnore]
        private readonly string _simId;
        [JsonIgnore]
        private readonly string _msId;
        [JsonIgnore]
        private readonly string _latitude;
        [JsonIgnore]
        private readonly string _longitude;
        [JsonIgnore]
        private readonly string _deviceId;
        [JsonIgnore]
        private readonly string _language;
        [JsonIgnore]
        private bool _isUnicode;
        [JsonIgnore]
        private string _appVersion;
        [JsonIgnore]
        private bool _isNative;
        [JsonIgnore]
        private bool _showHeaderTop;
        [JsonIgnore]
        private bool _isIphone;
        public Utility(string username,string firstname, string lastname, string email, string gender, string dateofbirthday, string dateofbirthmonth, string dateofbirthyear,
            string mobileNumber, string password, string appId, string simId, string msId, string latitude,
            string address1, string address2, string city, string state, string country, string otheremail,
            string longitude, string deviceId, string language, bool isUnicode, bool showHeaderTop, string appVersion, bool isNative, bool isIphone)
        {
            _username = username;
            _firstname = firstname;
            _lastname = lastname;
            _email = email;
            _gender = gender;
            _dateofbirthday = dateofbirthday;
            _dateofbirthmonth = dateofbirthmonth;
            _dateofbirthyear = dateofbirthyear;
            _address1 = address1;
            _address2 = address2;
            _city = city;
            _state = state;
            _country = country;
            _otheremail = otheremail;
            //-----------------------------------------//
            _mobileNumber = mobileNumber;
            _password = password;
            _appId = appId;
            _simId = simId;
            _msId = msId;
            _latitude = latitude;
            _longitude = longitude;
            _deviceId = deviceId;
            _language = language;
            _isUnicode = isUnicode;
            _isNative = isNative;
            _showHeaderTop = showHeaderTop;
            _appVersion = appVersion; 
            _isIphone = isIphone;
        }

        [JsonProperty(PropertyName = "Username")]
        public string Username
        {
            get { return _username; }
        }
        [JsonProperty(PropertyName = "FirstName")]
        public string FirstName
        {
            get { return _firstname; }
        }
        [JsonProperty(PropertyName = "LastName")]
        public string LastName
        {
            get { return _lastname; }
        }
        [JsonProperty(PropertyName = "Email")]
        public string Email
        {
            get { return _email; }
        }
        [JsonProperty(PropertyName = "Gender")]
        public string Gender
        {
            get { return _gender; }
        }
        [JsonProperty(PropertyName = "DateofBirthDay")]
        public string DateofBirthDay
        {
            get { return _dateofbirthday; }
        }
        [JsonProperty(PropertyName = "DateofBirthMonth")]
        public string DateofBirthMonth
        {
            get { return _dateofbirthmonth; }
        }
        [JsonProperty(PropertyName = "DateofBirthYear")]
        public string DateofBirthYear
        {
            get { return _dateofbirthyear; }
        }
        [JsonProperty(PropertyName = "Address1")]
        public string Address1
        {
            get { return _address1; }
        }
        [JsonProperty(PropertyName = "Address2")]
        public string Address2
        {
            get { return _address2; }
        }
        [JsonProperty(PropertyName = "City")]
        public string City
        {
            get { return _city; }
        }
        [JsonProperty(PropertyName = "State")]
        public string State
        {
            get { return _state; }
        }
        [JsonProperty(PropertyName = "Country")]
        public string Country
        {
            get { return _country; }
        }
        [JsonProperty(PropertyName = "OtherEmail")]
        public string OtherEmail
        {
            get { return _otheremail; }
        }
        
        //----------------------- End Added Field
        [JsonProperty(PropertyName = "MobileNumber")]
        public string MobileNumber
        {
            get { return _mobileNumber; }
        }

        [JsonProperty(PropertyName = "Pwd")]
        public string Password
        {
            get { return _password; }
        }

        [JsonProperty(PropertyName = "Appid")]
        public string AppId
        {
            get { return _appId; }
        }

        [JsonProperty(PropertyName = "Simid")]
        public string SimId
        {
            get { return _simId; }
        }

        [JsonProperty(PropertyName = "Msid")]
        public string MsId
        {
            get { return _msId; }
        }

        [JsonProperty(PropertyName = "Lat")]
        public string Latitude
        {
            get { return _latitude; }
        }

        [JsonProperty(PropertyName = "Long")]
        public string Longitude
        {
            get { return _longitude; }
        }

        [JsonProperty(PropertyName = "DeviceID")]
        public string DeviceId
        {
            get { return _deviceId; }
        }

        [JsonProperty(PropertyName = "Language")]
        public string Language
        {
            get { return _language; }
        }

        [JsonProperty(PropertyName = "IsUnicode")]
        public bool IsUnicode
        {
            get { return _isUnicode; }
            set { _isUnicode = value; }
        }
        [JsonProperty(PropertyName = "ShowHeaderTop")]
        public bool ShowHeaderTop
        {
            get { return _showHeaderTop; }
            set { _showHeaderTop = value; }
        }
        [JsonProperty(PropertyName = "AppVersion")]
        public string AppVersion
        {
            get { return _appVersion; }
            set { _appVersion = value; }
        }
        [JsonProperty(PropertyName = "IsNativeApp")]
        public bool IsNative
        {
            get { return _isNative; }
            set { _isNative = value; }
        }
        [JsonProperty(PropertyName = "IsIphone")]
        public bool IsIphone
        {
            get { return _isIphone; }
            set { _isIphone = value; }
        }
    }
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models
{
    public class SMSRequest
    {
        public string Application { get; set; }
        public string DestinationNumber { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
    }
}

﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileApp.Models
{
    public class AppleNotification
    {
        [JsonProperty(PropertyName = "aps")]
        public AppleAps Aps { get; set; }
    }
}

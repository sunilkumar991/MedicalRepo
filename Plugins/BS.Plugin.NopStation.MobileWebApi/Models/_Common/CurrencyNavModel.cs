﻿using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;

namespace BS.Plugin.NopStation.MobileWebApi.Models._Common
{
    public partial class CurrencyNavModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public string CurrencySymbol { get; set; }
    }
}

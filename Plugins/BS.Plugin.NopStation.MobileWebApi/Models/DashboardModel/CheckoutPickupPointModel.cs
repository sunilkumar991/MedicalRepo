﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Models;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel
{
    public partial class CheckoutPickupPointModel: BaseNopModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ProviderSystemName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string CountryName { get; set; }

        public string ZipPostalCode { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public string PickupFee { get; set; }

        public string OpeningHours { get; set; }
    }
}

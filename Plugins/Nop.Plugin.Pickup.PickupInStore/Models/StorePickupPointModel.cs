﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Pickup.PickupInStore.Models
{
    public class StorePickupPointModel : BaseNopEntityModel
    {
        public StorePickupPointModel()
        {
            this.Address = new AddressModel();
            AvailableStores = new List<SelectListItem>();
        }

        public AddressModel Address { get; set; }

        [NopResourceDisplayName("Plugins.Pickup.PickupInStore.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Plugins.Pickup.PickupInStore.Fields.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Plugins.Pickup.PickupInStore.Fields.PickupFee")]
        public decimal PickupFee { get; set; }

        [NopResourceDisplayName("Plugins.Pickup.PickupInStore.Fields.OpeningHours")]
        public string OpeningHours { get; set; }

        [NopResourceDisplayName("Plugins.Pickup.PickupInStore.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Plugins.Pickup.PickupInStore.Fields.Latitude")]
        public decimal Latitude { get; set; }

        [NopResourceDisplayName("Plugins.Pickup.PickupInStore.Fields.Longitude")]
        public decimal Longitude { get; set; }

        public List<SelectListItem> AvailableStores { get; set; }
        [NopResourceDisplayName("Plugins.Pickup.PickupInStore.Fields.Store")]
        public int StoreId { get; set; }
        public string StoreName { get; set; }
    }

    public class AddressModel
    {
        public AddressModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
            AvailableCities = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Address.Fields.Country")]
        public int? CountryId { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.StateProvince")]
        public int? StateProvinceId { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.City")]
        public int? CityId { get; set; }
        public IList<SelectListItem> AvailableCities { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.County")]
        public string County { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.City")]
        public string City { get; set; }


        [NopResourceDisplayName("Admin.Address.Fields.Address1")]
        public string Address1 { get; set; }

        [NopResourceDisplayName("Admin.Address.Fields.ZipPostalCode")]
        public string ZipPostalCode { get; set; }

    }
}
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Vendor
{
    public class CreateProduct
    {
        public string Name { get; set; }

        public int VendorId { get; set; }

        public int DeliveryDateId { get; set; }

        public int CategoryId { get; set; }

        public string Barcode { get; set; }

        public IFormFile File { get; set; }
    }
}

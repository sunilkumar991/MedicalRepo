using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Medicine
{
    public class MedicineItem
    {
        public string MedicineName { get; set; }
        public int Quantity { get; set; }
        public string UnitPrice { get; set; }
        public string TotalAmount { get; set; }
        public bool IsAvailable { get; set; }
    }
}

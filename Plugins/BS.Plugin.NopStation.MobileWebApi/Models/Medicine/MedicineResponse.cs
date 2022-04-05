using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Medicine
{
    public class MedicineResponse : MedicineRequestData
    {
        public MedicineResponse()
        {
            MedicineItems = new List<MedicineItem>();
        }

        public int RequestStatusId { get; set; }

        public string RequestStatusMessage { get; set; }

        public decimal OrderTotal { get; set; }

        public string OrderTotalValue { get; set; }

        public string RejectedReason { get; set; }

        public List<MedicineItem> MedicineItems { get; set; }
    }
}

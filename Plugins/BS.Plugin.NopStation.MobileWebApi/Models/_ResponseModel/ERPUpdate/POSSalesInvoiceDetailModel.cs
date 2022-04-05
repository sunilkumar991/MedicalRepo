using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.ERPUpdate
{
    public partial class POSSalesInvoiceDetailModel
    {
        // public string Id { get; set; }

        public string ProductId { get; set; }
        //public string PickUpAddress { get; set; }
        public int Quantity { get; set; }

        public decimal SellingRate { get; set; }
        public int ecommid { get; set; }


    }
}

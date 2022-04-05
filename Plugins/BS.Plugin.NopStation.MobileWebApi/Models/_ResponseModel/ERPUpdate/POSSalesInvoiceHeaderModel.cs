using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.ERPUpdate
{
    public partial class POSSalesInvoiceHeaderModel
    {
        //public List<POSSalesInvoiceDetailModel> POSSalesInvoiceDetail { get; set; }

        //public int Id { get; set; }
        public string CompanyId { get; set; }
        public string BusinessUnitId { get; set; }
        public string InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string SalesInvoiceCode { get; set; }
        public string Remarks { get; set; }

    }
}

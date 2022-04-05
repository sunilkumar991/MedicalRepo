using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.ERPUpdate
{
    public partial class RootObjectModel
    {
        public List<POSSalesInvoiceDetailModel> POSSalesInvoiceDetail { get; set; }
        public POSSalesInvoiceHeaderModel POSSalesInvoiceHeaderModel { get; set; }
    }
}

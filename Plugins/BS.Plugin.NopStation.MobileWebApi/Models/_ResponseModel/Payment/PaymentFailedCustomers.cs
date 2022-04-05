using Nop.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Payment
{
    public class PaymentFailedCustomers : BaseEntity
    {
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionErrorMessage { get; set; }
        public decimal TransactionAmount { get; set; }
        public int Status { get; set; }
        public string Comments { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedOn { get; set; }
        public bool IsNew { get; set; }
    }
}

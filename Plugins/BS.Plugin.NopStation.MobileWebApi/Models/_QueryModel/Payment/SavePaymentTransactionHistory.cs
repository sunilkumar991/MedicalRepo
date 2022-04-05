using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Payment
{
    public class SavePaymentTransactionHistory
    {
        public string CGMStoreID { get; set; }
        public string CGMBusinessUnitID { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public string TransactionStatusCode { get; set; }
        public string TransactionDescription { get; set; }
        public decimal TransactionAmount { get; set; }
    }
}

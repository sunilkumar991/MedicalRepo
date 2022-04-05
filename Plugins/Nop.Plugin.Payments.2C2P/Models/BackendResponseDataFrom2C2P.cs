using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments._2C2P.Models
{
    public class BackendResponseDataFrom2C2P
    {
        public BackendResponseDataFrom2C2P()
        {
            PaymentResponse = new PaymentResponse();
        }
        public PaymentResponse PaymentResponse { get; set; }
    }

    public class PaymentResponse
    {
        public string version { get; set; }
        public string timeStamp { get; set; }
        public string merchantID { get; set; }
        public string respCode { get; set; }
        public string pan { get; set; }
        public string amt { get; set; }
        public string uniqueTransactionCode { get; set; }
        public string tranRef { get; set; }
        public string approvalCode { get; set; }
        public string refNumber { get; set; }
        public string eci { get; set; }
        public string dateTime { get; set; }
        public string status { get; set; }
        public string failReason { get; set; }
        public string userDefined1 { get; set; }
        public string userDefined2 { get; set; }
        public string userDefined3 { get; set; }
        public string userDefined4 { get; set; }
        public string userDefined5 { get; set; }
        public string storeCardUniqueID { get; set; }
        public string recurringUniqueID { get; set; }
        public string paymentChannel { get; set; }
        public string backendInvoice { get; set; }
        public string paidChannel { get; set; }
        public string paidAgent { get; set; }
        public string IssuerCountry { get; set; }
        public string BankName { get; set; }
        public string hashValue { get; set; }
    }
}

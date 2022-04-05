using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments._2C2P.Models
{
    public class TwoC2PSDKResponse
    {
        public string respCode { get; set; }
        public string tranRef { get; set; }
        public string approvalCode { get; set; }
        public string refNumber { get; set; }
        public string eci { get; set; }
        public string dateTime { get; set; }
        public string status { get; set; }
        public string raw { get; set; }
        public string failReason { get; set; }
        public string storeCardUniqueID { get; set; }
        public string recurringUniqueID { get; set; }
        public string ippPeriod { get; set; }
        public string ippInterestType { get; set; }
        public string ippInterestRate { get; set; }
        public string ippMerchantAbsorbRate { get; set; }
        public string paidChannel { get; set; }
        public string paidAgent { get; set; }
        public string paymentChannel { get; set; }
        public string backendInvoice { get; set; }
        public string issuerCountry { get; set; }
        public string bankName { get; set; }
        public Submerchantlist subMerchantList { get; set; }
        public string amt { get; set; }
        public string hashValue { get; set; }
        public string merchantID { get; set; }
        public string pan { get; set; }
        public string subMerchantID { get; set; }
        public string timeStamp { get; set; }
        public string uniqueTransactionCode { get; set; }
        public string userDefined1 { get; set; }
        public string userDefined2 { get; set; }
        public string userDefined3 { get; set; }
        public string userDefined4 { get; set; }
        public string userDefined5 { get; set; }
        public string version { get; set; }
    }
    public class Submerchantlist
    {
        public object[] subMerchant { get; set; }
    }
}

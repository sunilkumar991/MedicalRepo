using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments._2C2P.Models
{
    // Created by Alexandar Rajavel on 21-Mar-2019
    public class _2C2PResponseData
    {
        public string version { get; set; }
        public string request_timestamp { get; set; }
        public string merchant_id { get; set; }
        public string order_id { get; set; }
        public string invoice_no { get; set; }
        public string payment_channel { get; set; }
        public string payment_status { get; set; }
        public string channel_response_code { get; set; }
        public string channel_response_desc { get; set; }
        public string approval_code { get; set; }
        public string eci { get; set; }
        public string transaction_datetime { get; set; }
        public string transaction_ref { get; set; }
        public string masked_pan { get; set; }
        public string paid_agent { get; set; }
        public string paid_channel { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string user_defined_1 { get; set; }
        public string user_defined_2 { get; set; }
        public string user_defined_3 { get; set; }
        public string user_defined_4 { get; set; }
        public string user_defined_5 { get; set; }
        public string browser_info { get; set; }
        public string stored_card_unique_id { get; set; }
        public string backend_invoice { get; set; }
        public string recurring_unique_id { get; set; }
        public string ippPeriod { get; set; }
        public string ippInterestType { get; set; }
        public string ippInterestRate { get; set; }
        public string ippMerchantAbsorbRate { get; set; }
        public string hash_value { get; set; }
    }
}

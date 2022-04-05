using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments._2C2P.Models
{
    // Created by Alexandar Rajavel on 21-Mar-2019
    public class _2C2PRequestData
    {
        public string version { get; set; }
        public string merchant_id { get; set; }
        public string payment_description { get; set; }
        public string order_id { get; set; }
        public string invoice_no { get; set; }
        public string user_defined_1 { get; set; }
        public string user_defined_2 { get; set; }
        public string user_defined_3 { get; set; }
        public string user_defined_4 { get; set; }
        public string user_defined_5 { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string promotion { get; set; }
        public string customer_email { get; set; }
        public string pay_category_id { get; set; }
        public string result_url_1 { get; set; }
        public string result_url_2 { get; set; }
        public string payment_option { get; set; }
        public string ipp_interest_type { get; set; }
        public string payment_expiry { get; set; }
        public string default_lang { get; set; }
        public string enable_store_card { get; set; }
        public string stored_card_unique_id { get; set; }
        public string request_3ds { get; set; }
        public string recurring { get; set; }
        public string order_prefix { get; set; }
        public string recurring_amount { get; set; }
        public string allow_accumulate { get; set; }
        public string max_accumulate_amount { get; set; }
        public string recurring_interval { get; set; }
        public string recurring_count { get; set; }
        public string charge_next_date { get; set; }
        public string charge_on_date { get; set; }
        public string statement_descriptor { get; set; }
        public string hash_value { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.OKDollar.Models
{
    public class RequestToJson
    {
        public string ApiKey { get; set; }
        public string Destination { get; set; }
        public string MerchantName { get; set; }
        public string Source { get; set; }
        public decimal Amount { get; set; }
        public string RefNumber { get; set; }
    }
}

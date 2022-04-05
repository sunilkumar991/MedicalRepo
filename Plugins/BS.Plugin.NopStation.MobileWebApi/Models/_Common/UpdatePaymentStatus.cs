using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models._Common
{
    public class UpdatePaymentStatus
    {
        public int OrderId { get; set; }
        public string TransactionId { get; set; }
        public string TransactionStatus { get; set; }
    }
}

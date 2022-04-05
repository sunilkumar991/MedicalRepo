using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.OKDollar.Models
{
    public class ResponseFromOKDollarPaymentGateway : BaseNopModel
    {
        public string ResponseCode { get; set; }
        public string Destination { get; set; }
        public string Source { get; set; }
        public string Amount { get; set; }
        public string TransactionId { get; set; }
        public string TransactionTime { get; set; }
        public string AgentName { get; set; }
        public string Kickvalue { get; set; }
        public string Loyaltypoints { get; set; }
        public string Description { get; set; }
        public string MerRefNo { get; set; }
        public string MerchantUrl { get; set; }
        public object InitiatorOkAccNumber { get; set; }
        public object TransactionRequestedDateAndTime { get; set; }
        public object TransactionResponseDateAndTime { get; set; }
    }
}

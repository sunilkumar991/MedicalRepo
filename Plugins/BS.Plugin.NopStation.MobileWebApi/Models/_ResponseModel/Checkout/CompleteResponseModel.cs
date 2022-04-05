using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Checkout
{
    public class CompleteResponseModel : BaseResponse
    {
        public int OrderId { get; set; } 
        public int OrderGroupNumber { get; set; }  //Added By Ankur Shrivastava on 10-OCT-2018 for EC-151
        public List<int> OrderIds { get; set; }
        public string CustomOrderNumber { get; set; }
        public List<string> CustomOrderNumbers { get; set; }  //Added By Ankur Shrivastava on 10-OCT-2018 for EC-151
        public bool CompleteOrder { get; set; }
        public PaypalModel PayPal { get; set; }
        public int PaymentType { get; set; }
        //public List<Nop.Core.Domain.Orders.Order> Orders { get; set; }
        public class PaypalModel
        {
            public string ClientId { get; set; }

        }
    }
    
}

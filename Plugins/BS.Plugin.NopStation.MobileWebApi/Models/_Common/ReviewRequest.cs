using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models._Common
{
   public class ReviewRequest
    {
        public string ReviewText { get; set; }
        public string OrderNo { get; set; }
        public int Rating { get; set; }
        public int ReviewType { get; set; }
        //public int AddressId { get; set; }
        //public int CustomerId { get; set; }
        //public DateTime CreatedOnUtc { get; set; }


    }

}

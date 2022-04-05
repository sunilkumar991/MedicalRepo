﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Models;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Checkout
{
    public class CheckoutPaymentMethodResponseModel : BaseResponse
    {
        public CheckoutPaymentMethodResponseModel()
        {
            PaymentMethods = new List<PaymentMethodModel>();
        }

        public IList<PaymentMethodModel> PaymentMethods { get; set; }
        

        public bool DisplayRewardPoints { get; set; }
        public int RewardPointsBalance { get; set; }
        public string RewardPointsAmount { get; set; }
        public bool UseRewardPoints { get; set; }

        #region Nested classes

        public partial class PaymentMethodModel : BaseNopModel
        {
            public string PaymentMethodSystemName { get; set; }
            public string Name { get; set; }
            public string Fee { get; set; }
            public bool Selected { get; set; }
            public string LogoUrl { get; set; }

            public string PAYMENT_URL { get; set; }

            public string API_KEY { get; set; }

            public string SECRET_KEY { get; set; }

            public string IV_VALUE { get; set; }

            public IList<SubPaymentMethodName> subPaymentMethodNames { get; set; }
        }

        public class SubPaymentMethodName
        {
            public string Name { get; set; }
            public string PaymentNumber{ get; set; }

            public string LogoUrl { get; set; }
            public string MDRPercentage { get; set; }
        }
        #endregion
    }
}

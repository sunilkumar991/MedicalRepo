using System;
using System.Collections.Generic;
using System.Linq;
using BraintreeHttp;
using PayPal.Core;
using PayPal.v1.Payments;

namespace BS.Plugin.NopStation.MobileWebApi.Extensions.Paypal
{
    public class PayPalExtension
    {
        public  static string ClientId="AQGvRxqTvxUMakgJc8M5q4W7Jw-WryPypJNsz-5Ns_z9Z19SL1TumRnil6LUf5Z3MMFwo4Uv2ikqxPlt";
        public  static string ClientSecret = "EKld9w88GVm_A5aSaaPFtq9i1svLpM-N1rUqwkMVZtUVTc_9N6kYbdVSngsh7llfVOdtgt3FSujEUjoB";



        //private static Dictionary<string, string> Configuration() 
        //{
        //    var defaultConfig = new Dictionary<string, string>();
        //    // Default connection timeout in milliseconds
        //    defaultConfig[BaseConstants.HttpConnectionTimeoutConfig] = "30000";
        //    defaultConfig[BaseConstants.HttpConnectionRetryConfig] = "3";
        //    defaultConfig[BaseConstants.ApplicationModeConfig] = BaseConstants.SandboxMode;

        //    return defaultConfig;
        //}

        //// Create accessToken
        //private static string GetAccessToken()
        //{

        //    var config = Configuration();
        //    ClientSecret = "EKld9w88GVm_A5aSaaPFtq9i1svLpM-N1rUqwkMVZtUVTc_9N6kYbdVSngsh7llfVOdtgt3FSujEUjoB";
        //    string accessToken = new OAuthTokenCredential(ClientId, ClientSecret, config).GetAccessToken();
        //    return accessToken;
        //}

        //// Returns APIContext object
        //public static APIContext GetAPIContext(string accessToken = "")
        //{
             
        //    var apiContext = new APIContext(string.IsNullOrEmpty(accessToken) ? GetAccessToken() : accessToken);
        //    apiContext.Config = Configuration();


        //    return apiContext;
        //}

        public async System.Threading.Tasks.Task<PaypalDetailModel> GetAmountAsync(string paymentId)
        {
            var environment = new SandboxEnvironment(ClientId, ClientSecret);
            var client = new PayPalHttpClient(environment);

            var payment = new Payment()
            {
                Intent = "authorize",
                Transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        Amount = new Amount()
                        {
                            Total = "10",
                            Currency = "USD"
                        }
                    }
                },
                RedirectUrls = new RedirectUrls()
                {
                    CancelUrl = "https://example.com/cancel",
                    ReturnUrl = "https://example.com/return"
                },
                Payer = new Payer()
                {
                    PaymentMethod = "paypal"
                }
            };

            PaymentCreateRequest request = new PaymentCreateRequest();
            request.RequestBody(payment);

            var payDetail = new PaypalDetailModel();
            try
            {
                HttpResponse response = await client.Execute(request);
                var statusCode = response.StatusCode;
                Payment result = response.Result<Payment>();

                if (result.State.Equals("approved"))
                {
                    payDetail.PaymentStatus = result.State;
                    payDetail.Currency = result.Transactions.FirstOrDefault().Amount.Currency;
                    payDetail.Total = result.Transactions.FirstOrDefault().Amount.Total;
                    payDetail.PayeeId = result.Payer.PayerInfo.PayerId;
                }
            }
            catch (Exception)
            {
                payDetail = null;

            }
            return payDetail;
        }

    }
}
 

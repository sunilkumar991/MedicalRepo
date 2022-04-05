using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Models.AppUpgradeDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nop.Core;
using Nop.Services.Customers;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{

  
    public class IOSSMSVerifyController : Controller
    {
        private IConfiguration configuration;
        public IOSSMSVerifyController(IConfiguration iconfig)
        {
            configuration = iconfig;
        }

        [Route("api/SMSVerifyNumber")]
        [HttpGet]
        public IActionResult AWSKey()
        {
            string IOSVerifyNumber = configuration.GetValue<string>("IOSSMSVerifyNumber:MobileNumber");
            var result = new IOSSMSNumber();
            if (IOSVerifyNumber != null)
            {
                result.MobileNumber = IOSVerifyNumber;
            }
            else
            {
                var numbberResult = new IOSSMSNumber();
                numbberResult.StatusCode = (int)ErrorType.NotFound;
                numbberResult.ErrorList.Add("No Data");
                return Ok(numbberResult);
            }
            return Ok(result);
        }
    }
}

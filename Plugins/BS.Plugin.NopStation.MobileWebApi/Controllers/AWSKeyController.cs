using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class AWSKeyController : Controller
    {
        private IConfiguration configuration;
        public AWSKeyController(IConfiguration iconfig)
        {
            configuration = iconfig;
        }

        [Route("api/AWSKey")]
        [HttpGet]
        public IActionResult AWSKey()
        {
            string dbConn2 = configuration.GetValue<string>("AWSLoadBalancer:ALBKey");
            
                return Ok(dbConn2);
            
        }
    }
}

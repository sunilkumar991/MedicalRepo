using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace BS.Plugin.NopStation.MobileApp.Controllers
{
    public class BaseApiController : Controller
    {
        public string GetHeaderValue()
        {
           // IEnumerable<string> headerValues;

            StringValues headerValues;
           

            Request.Headers.TryGetValue("UserId", out headerValues);
           // var keyFound = Request.Headers.TryGetValues("UserId", out headerValues);
            var keyFound = headerValues.FirstOrDefault();
          
                return headerValues.FirstOrDefault();
         
        }
    }
}

using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Models.AppUpgradeDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nop.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class InvoicePDFAddreesController: Controller
    {
        private IConfiguration configuration;
        private readonly IWorkContext _workContext;
        public InvoicePDFAddreesController(IConfiguration iconfig, IWorkContext workContext)
        {
            configuration = iconfig;
            _workContext = workContext;
        }

        [Route("api/InvoicePDFAddrees")]
        [HttpGet]
        public IActionResult InvoicePDFAddrees()
        {
            string messageforupdate = string.Empty;
            if (_workContext.WorkingLanguage.Id == 1)
            {
                messageforupdate = configuration.GetValue<string>("InvoicePDFAddreesEnglish:Address");
            }
            else if (_workContext.WorkingLanguage.Id == 2)
            {
                messageforupdate = configuration.GetValue<string>("InvoicePDFAddreesZawgi:Address");
            }
            else
            {
                messageforupdate = configuration.GetValue<string>("InvoicePDFAddreesUnicode:Address");
            }



            var result = new PDFAddress();
            if (messageforupdate != null)
            {
                result.Address = messageforupdate;
            }
            else
            {
                var numbberResult = new PDFAddress();
                numbberResult.StatusCode = (int)ErrorType.NotFound;
                numbberResult.ErrorList.Add("No Data");
                return Ok(numbberResult);
            }
            return Ok(result);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Nop.Web.Controllers
{
    public class OKDollarResponseController : Controller
    {
       
        public IActionResult OKDollarResponse(string ResponseJson, string MsgString)
        {
            ViewBag.ResponseJson = ResponseJson;
            ViewBag.MsgString = (MsgString=="0"? "Your Payment is done" : "Payment Failure");
            return View();
        }
    }
}
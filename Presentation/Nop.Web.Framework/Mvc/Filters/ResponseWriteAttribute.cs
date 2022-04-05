using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Data;
using Nop.Services.Customers;

namespace Nop.Web.Framework.Mvc.Filters
{
    public class ResponseWriteAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public ResponseWriteAttribute() : base(typeof(ResponseWriteFilter))
        {
        }

        #endregion

        private class ResponseWriteFilter : IResultFilter
        {

            #region Fields

            //private readonly ICustomerService _customerService;
            //private readonly IWorkContext _workContext;
            private readonly IHttpContextAccessor _httpContextAccessor;

            #endregion

            #region Ctor

            public ResponseWriteFilter(IHttpContextAccessor httpContextAccessor)
            {
                this._httpContextAccessor = httpContextAccessor;
            }

            #endregion

            public void OnResultExecuting(ResultExecutingContext context)
            {
                //throw new NotImplementedException();
            }

            public void OnResultExecuted(ResultExecutedContext context)
            {
                try
                {
                    var bytes = Encoding.UTF8.GetBytes("Foo Bar");

                    // Seek to end
                    context.HttpContext.Response.Body.Seek(context.HttpContext.Response.Body.Length, SeekOrigin.Begin);
                    context.HttpContext.Response.Body.Write(bytes, 0, bytes.Length);
                }
                catch
                {
                    // ignored
                }

                //base.OnResultExecuted(context);
                //base(context);
            }


        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileApp.Infrastructure.WebApi.Logger
{
    public class ErrorResult : IActionResult
    {
        private readonly string _errorMessage;
        private readonly HttpRequestMessage _requestMessage;
        private readonly HttpStatusCode _statusCode;

        public ErrorResult(HttpRequestMessage requestMessage, HttpStatusCode statusCode, string errorMessage)
        {
            _requestMessage = requestMessage;
            _statusCode = statusCode;
            _errorMessage = errorMessage;
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            return Task.FromResult(_requestMessage.CreateResponse(_statusCode));
        }
    }
}

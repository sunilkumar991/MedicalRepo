
namespace Nop.Core.Domain.Messages
{
    public class VerifyMobileNumberResponse
    {
        public string OTP { get; set; }
        public string DestinationNumber { get; set; }
        public string AppVersionName { get; set; }
        public bool IsUserRegistered { get; set; }
        //public string Token { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        //added by Sunil at 30-04-19
        public string HashKey { get; set; }
    }
}

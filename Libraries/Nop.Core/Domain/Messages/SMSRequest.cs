
namespace Nop.Core.Domain.Messages
{
    public class SMSRequest
    {
        public string Application { get; set; }
        public string DestinationNumber { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
    }
}

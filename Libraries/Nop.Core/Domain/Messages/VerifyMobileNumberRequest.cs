
namespace Nop.Core.Domain.Messages
{
    public class VerifyMobileNumberRequest
    {
        public string Application { get; set; }
        public string DestinationNumber { get; set; }
        public string Operator { get; set; }
        public string AppVersionName { get; set; }
        public int DeviceTypeId { get; set; }
        //added by Sunil at 30-04-19
        public string BuildType { get; set; }
    }
}


namespace Nop.Core.Domain.Messages
{
    public class ReadSMS
    {
        public int code { get; set; }
        public string reason { get; set; }
        public string ssrc { get; set; }
        public int sms_num { get; set; }
        public int next_sms { get; set; }
        public object[][] data { get; set; }
    }
}

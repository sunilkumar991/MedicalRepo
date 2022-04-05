using System;

namespace Nop.Core.Domain.Messages
{
    public class AppleNotificationResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public int Id { get; set; }
        public DateTime? SentOnUtc { get; set; }
    }
}

using Nop.Core.Configuration;

namespace Nop.Core.Domain.Messages
{
    public class NotificationSettings : ISettings
    {
        #region ios settings
        public string AppleCertFileNameWithPath { get; set; }
        public string ApplePassword { get; set; }
        public bool IsAppleProductionMode { get; set; }
        #endregion

        #region Android settings
        /// <summary>
        ///Android GCM Notification-- YOUR Google API's Console API Access  API KEY for Server Apps HERE
        /// </summary>
        public string GoogleConsoleAPIAccess_KEY { get; set; }
        public string GoogleProject_Number { get; set; }
        #endregion
    }
}

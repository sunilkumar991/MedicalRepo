using Nop.Core;
using Nop.Core.Domain.AppUpgrade;
using System;
using System.Collections.Generic;

namespace Nop.Services.AppUpgrade
{
    /// <summary>
    /// App Upgrade Detail service
    /// Created By : Sunil Kumar S
    /// Created On : 03-Jan-2020
    /// </summary>
    public partial interface IAppUpgradeService
    {
        /// <summary>
        /// Get App Upgrade Detail where IsUpdateRequired == "true"
        /// </summary>
        /// <returns>AppUpgradeDetail</returns>
        Core.Domain.AppUpgrade.AppUpgradeDetails GetAppUpgradeDetail();

        Core.Domain.AppUpgrade.AppUpgradeDetails GetAppUpgradeDetailBasedonDevicerid(int deviceid);

        
    }
}

using Nop.Core.Data;
using Nop.Core.Domain.AppUpgrade;
using Nop.Services.Events;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using System.Linq;
using Nop.Services.AppUpgrade;

namespace Nop.Services.AppUpgradeDetails
{
    /// <summary>
    /// App Upgrade Detail service
    /// Created By : Sunil Kumar S
    /// Created On : 03-Jan-2020
    /// </summary>
    public partial class AppUpgradeService : IAppUpgradeService
    {
        private readonly IRepository<Core.Domain.AppUpgrade.AppUpgradeDetails> _AppUpgradeDetailRepository;
        private readonly ICacheManager _cacheManager;

        public AppUpgradeService(IRepository<Core.Domain.AppUpgrade.AppUpgradeDetails> AppUpgradeDetailRepository,  ICacheManager cacheManager)
        {
            _AppUpgradeDetailRepository = AppUpgradeDetailRepository;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Get App Upgrade Detail where IsUpdateRequired == "true"
        /// </summary>
        /// <returns>AppUpgradeDetail</returns>
        public virtual Core.Domain.AppUpgrade.AppUpgradeDetails GetAppUpgradeDetail()
        {
            var query = from p in _AppUpgradeDetailRepository.Table where p.DeviceId==10 && p.IsUpdateRequired == true  select p;
            var detail = query.ToList().FirstOrDefault();
            return detail;
        }



        public Core.Domain.AppUpgrade.AppUpgradeDetails GetAppUpgradeDetailBasedonDevicerid(int deviceid)
        {
            var query = from p in _AppUpgradeDetailRepository.Table where p.DeviceId==deviceid &&  p.IsUpdateRequired == true select p;
            var detail = query.ToList().FirstOrDefault();
            return detail;
        }
    }
}

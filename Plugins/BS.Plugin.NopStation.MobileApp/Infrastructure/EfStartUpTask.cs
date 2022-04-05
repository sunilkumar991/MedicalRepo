using Nop.Core.Infrastructure;
using BS.Plugin.NopStation.MobileApp.Data;

namespace BS.Plugin.NopStation.MobileApp.Infrastructure
{
    public class EfStartUpTask : IStartupTask
    {
        public void Execute()
        {
            //MobileAppMapperConfiguration.Init();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}

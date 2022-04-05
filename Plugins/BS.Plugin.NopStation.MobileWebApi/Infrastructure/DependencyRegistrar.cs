using Autofac;
using Autofac.Core;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using BS.Plugin.NopStation.MobileWebApi.Data;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using BS.Plugin.NopStation.MobileWebApi.Factories;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Nop.Services.Common;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace BS.Plugin.NopStation.MobileWebApi.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "bs_object_context_nopstation_MobileWebApi";
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterPluginDataContext<MobileWebApiObjectContext>(CONTEXT_NAME);

            builder.RegisterType<ProductServiceApi>().As<IProductServiceApi>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerServiceApi>().As<ICustomerServiceApi>().InstancePerLifetimeScope();
            builder.RegisterType<DeviceService>().As<IDeviceService>().InstancePerLifetimeScope();
            builder.RegisterType<BS_SliderService>().As<IBS_SliderService>().InstancePerLifetimeScope();

            builder.RegisterType<AddressModelFactoryApi>().As<IAddressModelFactoryApi>().InstancePerLifetimeScope();
            builder.RegisterType<CatalogModelFactoryApi>().As<ICatalogModelFactoryApi>().InstancePerLifetimeScope();
            builder.RegisterType<CheckoutModelFactoryApi>().As<ICheckoutModelFactoryApi>().InstancePerLifetimeScope();
            builder.RegisterType<CommonModelFactoryApi>().As<ICommonModelFactoryApi>().InstancePerLifetimeScope();
            builder.RegisterType<CountryModelFactoryApi>().As<ICountryModelFactoryApi>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerModelFactoryApi>().As<ICustomerModelFactoryApi>().InstancePerLifetimeScope();
            builder.RegisterType<OrderModelFactoryApi>().As<IOrderModelFactoryApi>().InstancePerLifetimeScope();
            builder.RegisterType<ProductModelFactoryApi>().As<IProductModelFactoryApi>().InstancePerLifetimeScope();
            builder.RegisterType<ShoppingCartModelFactoryApi>().As<IShoppingCartModelFactoryApi>().InstancePerLifetimeScope();
            builder.RegisterType<GenericAttributeServiceApi>().As<IGenericAttributeService>().InstancePerLifetimeScope();
            //builder.RegisterType<QueuedNotificationApiService>().As<IQueuedNotificationApiService>().InstancePerLifetimeScope();
            //added By Sunil Kumar at 16-04-2020
            builder.RegisterType<PickUpOrderModelFactoryApi>().As<IPickUpOrderModelFactoryApi>().InstancePerLifetimeScope();

            //work context
            builder.RegisterType<ApiWebWorkContext>().As<IWorkContext>().InstancePerLifetimeScope();
            builder.RegisterType<ExternalAuthorizerApi>().As<IExternalAuthorizerApi>().InstancePerLifetimeScope();

            //data context
            builder.RegisterType<EfRepository<Device>>()
                .As<IRepository<Device>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<BS_Slider>>()
                .As<IRepository<BS_Slider>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            builder.RegisterType<BsNopMobilePluginService>().As<IBsNopMobilePluginService>().InstancePerLifetimeScope();
            builder.RegisterType<ContentManagementService>().As<IContentManagementService>().InstancePerLifetimeScope();
            builder.RegisterType<ContentManagementTemplateService>().As<IContentManagementTemplateService>().InstancePerLifetimeScope();
            builder.RegisterType<CategoryIconService>().As<ICategoryIconService>().InstancePerLifetimeScope();

            //data context
            //builder.RegisterPluginDataContext<MobileWebApiObjectContext>(CONTEXT_NAME);

            //override required repository with our custom context
            builder.RegisterType<EfRepository<BS_FeaturedProducts>>()
                .As<IRepository<BS_FeaturedProducts>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<BS_ContentManagement>>()
             .As<IRepository<BS_ContentManagement>>()
             .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
             .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<BS_ContentManagementTemplate>>()
             .As<IRepository<BS_ContentManagementTemplate>>()
             .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
             .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<BS_CategoryIcons>>()
             .As<IRepository<BS_CategoryIcons>>()
             .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
             .InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 4; }
        }




    }
}

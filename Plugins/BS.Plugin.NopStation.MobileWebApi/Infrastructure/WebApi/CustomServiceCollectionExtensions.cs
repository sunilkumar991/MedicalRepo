using BS.Plugin.NopStation.MobileWebApi.Infrastructure.WebApi.Logger;
using Microsoft.Extensions.DependencyInjection;


namespace BS.Plugin.NopStation.MobileWebApi.Infrastructure.WebApi
{
    public static class CustomServiceCollectionExtensions
    {
        /// <summary>
        /// Add and configure MVC for the application
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <returns>A builder for configuring MVC services</returns>
        public static IMvcBuilder AddNopMvc(this IServiceCollection services)
        {
            //commented on 03-02-2020 by Sunil Kumar
            //add basic MVC feature
            //var mvcBuilder = services.AddMvc();

            //added on 03-02-2020 by Sunil Kumar
            var mvcBuilder = services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });


            //add global exception filter
            mvcBuilder.AddMvcOptions(options => options.Filters.Add(new SimpleExceptionFilter()));

           

            return mvcBuilder;
        }
    }
}

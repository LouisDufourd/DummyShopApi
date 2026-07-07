using DummyShopApi.BLL.Implementation;
using DummyShopApi.BLL.Interfaces;

namespace DummyShopApi.BLL
{
    /// <summary>
    /// Provides extension methods for registering Business Logic Layer services.
    /// </summary>
    public static class BLLExtensions
    {
        /// <summary>
        /// Registers all Business Logic Layer dependencies in the dependency injection container.
        /// </summary>
        /// <param name="services">The service collection used to register dependencies.</param>
        /// <returns>The service collection with the registered dependencies.</returns>
        public static void AddBLL(this IServiceCollection services)
        {
            services.AddTransient<IEcomService, EcommService>();
            services.AddTransient<ISecurityService, SecurityService>();
        }
    }
}
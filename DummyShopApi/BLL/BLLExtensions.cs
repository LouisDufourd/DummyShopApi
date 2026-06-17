using DummyShopApi.BLL.Interfaces;

namespace DummyShopApi.BLL
{
    public static class BLLExtensions
    {
        public static void AddBLL(this IServiceCollection services)
        {
            services.AddTransient<IEcomService, EcommService>();
        }
    }
}

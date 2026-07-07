namespace DummyShopApi.DAL
{
    public class DALOptions
    {
        public EDbType? TypeDB { get; set; }
        public string ConnectionString { get; set; }
    }

    public static class DALExtension
    {
        public static IServiceCollection AddDAL(this IServiceCollection services, Action<DALOptions>? configure)
        {
            DALOptions options = new DALOptions();

            configure?.Invoke(options);

            if (options.TypeDB is null)
            {
                throw new Exception("Property TypeDB needs to be set in appsettings.json");
            }

            if (options.ConnectionString is null)
            {
                throw new Exception("Property ConnectionString need to be set in appsettings.json");
            }

            services.AddScoped<IUOW, UOW>((_) => new UOW(options.ConnectionString, options.TypeDB.Value));

            return services;
        }
    }
}

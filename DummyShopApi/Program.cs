
using DummyShopApi.API.Filters;
using DummyShopApi.BLL;
using DummyShopApi.DAL;
using FluentValidation;
using System.Reflection;

namespace DummyShopApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            });
            builder.Services.AddBLL();
            builder.Services.AddDAL((DALOptions options) =>
            {
                var connectionString = builder.Configuration.GetValue<string>("ConnectionString");
                var eDBType = builder.Configuration.GetValue<EDBType?>("TypeDB");

                options.ConnectionString = connectionString;
                options.TypeDB = eDBType;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            builder.Services.AddScoped<ValidationFilter>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

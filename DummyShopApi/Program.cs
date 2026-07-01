
using DummyShopApi.API.Filters;
using DummyShopApi.BLL;
using DummyShopApi.DAL;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;


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

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidAudience = builder.Configuration["Jwt:Issuer"],
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["Jwt:Key"]!)),
                        ClockSkew = TimeSpan.Zero
                    };
                });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen((options) =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath, true);

                options.AddSecurityDefinition("jwt", new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.OAuth2,
                    Scheme = "Bearer",
                    In = ParameterLocation.Header,
                    Flows = new OpenApiOAuthFlows()
                    {
                        Password = new OpenApiOAuthFlow()
                        {
                            TokenUrl = new Uri("/api/authentification/loginSwagger", UriKind.Relative)
                        }
                    },
                });


                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "jwt" }
                        },
                        new string[] {}
                    }
                });
            });

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

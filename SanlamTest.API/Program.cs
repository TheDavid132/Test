
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SanlamTest.API.Models;
using SanlamTest.BLL.Interfaces;
using SanlamTest.BLL.Services;
using SanlamTest.Common.Interfaces;
using SanlamTest.Common.Services;
using SanlamTest.DAL.Factories;
using SanlamTest.DAL.Interfaces;
using SanlamTest.DAL.Interfaces;
using System.Text;

namespace SanlamTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Serilog.Debugging.SelfLog.Enable(Console.Error);
            var builder = WebApplication.CreateBuilder(args);

            // Add services.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = builder.Configuration["Jwt:Issuer"],
                   ValidAudience = builder.Configuration["Jwt:Audience"],
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
               };
           });


            ConfigureServices(builder);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication(); // Add this line
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            // Load configuration from appsettings.json and make it available
            var appSettings = builder.Configuration.Get<AppSettings>();
            builder.Services.AddSingleton(appSettings);

            //Common
            builder.Services.AddScoped<ILoggingService, SerilogLoggingService>();

            //BLL services
            builder.Services.AddScoped<ITransactionService, TransactionService>();

            //DAL 
            //repositories
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IAccountBalanceRepository, AccountBalanceRepository>();
            builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

            //misc
            builder.Services.AddScoped<IConnectionFactory, PostgreSqlConnectionFactory>();

            SetupSwagger(builder);

        }

        public static void SetupAuthentication(WebApplicationBuilder builder)
        {
            // Configure JWT authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(options =>
           {
               options.RequireHttpsMetadata = false;
               options.SaveToken = true;
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = builder.Configuration["Jwt:Issuer"],
                   ValidAudience = builder.Configuration["Jwt:Audience"],
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
               };
           });
        }

        public static void SetupSwagger(WebApplicationBuilder builder)
        {
            // Register Swagger services
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

                // Configure Swagger to use JWT tokens for authorization
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                // Add JWT bearer token authentication to Swagger
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string>()
                    }
                });
            });
        }
    }
}




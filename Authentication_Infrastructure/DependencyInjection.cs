using Authentication_Core.Interfaces;
using Authentication_Infrastructure.ApplicationDbContext;
using Authentication_Infrastructure.Options;
using Authentication_Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Authentication_Infrastructure.Messaging;
using RabbitMQ.Client;


namespace Authentication_Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            var connectionString = configuration.GetConnectionString("Default");
            var jwtOption = configuration.GetSection("Jwt").Get<JwtOptions>();
            var rabbitmqOptions = configuration.GetSection("RabbitMQ").Get<RabbitmqOptions>();


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOption.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtOption.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOption.SigningKey))
                    };
                });

            services.AddSingleton(jwtOption);
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddSingleton<RabbitMQConnection>();
            services.AddHostedService<RabbitMQInitializer>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEventBus, RabbitMQPublisher>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddSingleton<IConnectionFactory>(sp =>
            {
                return new ConnectionFactory()
                {
                    Uri = new Uri(rabbitmqOptions.Uri)
                };
            });
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
                connectionString, b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            return services;
        }
    }
}

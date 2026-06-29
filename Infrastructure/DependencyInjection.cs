using Application.Interfaces;
using Domain.Interfaces;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(opts =>
                opts.UseSqlServer(config.GetConnectionString("Default")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IJwtService, JwtService>();

            // ---- RabbitMQ demo ----
            var rabbitMqOptions = config.GetSection("RabbitMq").Get<RabbitMqOptions>() ?? new RabbitMqOptions();
            services.AddSingleton(rabbitMqOptions);
            services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
            services.AddHostedService<UserRegisteredConsumer>();

            return services;
        }
    }
}
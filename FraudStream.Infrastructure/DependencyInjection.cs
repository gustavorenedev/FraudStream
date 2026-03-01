using FraudStream.Application.Interfaces;
using FraudStream.Domain.Interfaces;
using FraudStream.Infrastructure.Cache;
using FraudStream.Infrastructure.Messaging.Consumers;
using FraudStream.Infrastructure.Messaging.Publishers;
using FraudStream.Infrastructure.Persistence;
using FraudStream.Infrastructure.Persistence.Repositories;
using FraudStream.Infrastructure.Rules;
using MassTransit;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace FraudStream.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ── PostgreSQL + EF Core ───────────────────────────────────
            services.AddDbContext<FraudStreamDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Postgres")));

            // Dapper — conexão aberta por request para queries de leitura
            services.AddScoped<IDbConnection>(_ =>
                new NpgsqlConnection(configuration.GetConnectionString("Postgres")));

            // ── Repositórios ───────────────────────────────────────────
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IFraudDecisionRepository, FraudDecisionRepository>();
            services.AddScoped<IFraudRuleRepository, FraudRuleRepository>();

            // ── Redis ──────────────────────────────────────────────────
            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));
            services.AddScoped<IVelocityCache, RedisVelocityCache>();

            // ── Cache em memória (FraudRuleRepository + BlockedMerchants) ──
            services.AddMemoryCache();

            // ── Fraud Rule Engine ──────────────────────────────────────
            services.AddScoped<IFraudRuleEngine, FraudRuleEngine>();

            // ── Event Publisher ────────────────────────────────────────
            services.AddScoped<IEventPublisher, EventPublisher>();

            // ── MassTransit + RabbitMQ ─────────────────────────────────
            services.AddMassTransit(bus =>
            {
                bus.AddConsumer<TransactionCreatedConsumer>();
                bus.AddConsumer<AuditConsumer>();
                bus.AddConsumer<NotificationConsumer>();

                bus.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(configuration["RabbitMq:Host"], configuration["RabbitMq:VHost"], h =>
                    {
                        h.Username(configuration["RabbitMq:Username"]!);
                        h.Password(configuration["RabbitMq:Password"]!);
                    });

                    // Retry com backoff exponencial antes de ir para DLQ
                    cfg.UseMessageRetry(r => r.Exponential(
                        retryLimit: 3,
                        minInterval: TimeSpan.FromSeconds(1),
                        maxInterval: TimeSpan.FromSeconds(10),
                        intervalDelta: TimeSpan.FromSeconds(2)));

                    cfg.ConfigureEndpoints(ctx);
                });
            });

            return services;
        }
    }
}

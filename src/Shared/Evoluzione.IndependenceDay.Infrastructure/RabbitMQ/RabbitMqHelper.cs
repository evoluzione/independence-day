using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Muflone.Transport.RabbitMQ;
using Muflone.Transport.RabbitMQ.Factories;
using Muflone.Transport.RabbitMQ.Models;

namespace Evoluzione.IndependenceDay.Infrastructure.RabbitMQ;

public static class RabbitMqHelper
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, RabbitMqSettings settings)
    {
        using var serviceProvider = services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

        var configuration = new RabbitMQConfiguration(
            settings.Host,
            settings.Username,
            settings.Password,
            settings.ExchangeCommandName,
            settings.ExchangeEventName,
            settings.ClientId);

        services.TryAddSingleton(new RabbitMQConnectionFactory(configuration, loggerFactory));
        services.AddMufloneTransportRabbitMQ(loggerFactory, configuration);

        return services;
    }
}

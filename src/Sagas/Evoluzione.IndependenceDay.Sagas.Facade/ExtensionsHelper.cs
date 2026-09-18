using Evoluzione.IndependenceDay.Contracts.Commands.Sagas;
using Evoluzione.IndependenceDay.Contracts.Events;
using Evoluzione.IndependenceDay.Infrastructure.RabbitMQ;
using Evoluzione.IndependenceDay.Sagas.Facade.IntegrationEventHandlers;
using Evoluzione.IndependenceDay.Sagas.Infrastructure;
using Evoluzione.IndependenceDay.Sagas.ShipInterception;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Muflone;
using Muflone.Saga;
using Muflone.Saga.Handlers;

namespace Evoluzione.IndependenceDay.Sagas.Facade;

public static class ExtensionsHelper
{
    internal static readonly string[] BusinessKeyFields = ["ShipId"];

    public static IServiceCollection AddSagas(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqSettings = configuration.GetSection("Sagas:RabbitMQ").Get<RabbitMqSettings>()
                               ?? throw new ArgumentException("Manca la sezione Sagas:RabbitMQ");

        services.AddRabbitMq(rabbitMqSettings);
        services.AddSagasInfrastructure(configuration, BusinessKeyFields);
        services.AddShipInterceptionSaga();

        return services;
    }

    private static void AddShipInterceptionSaga(this IServiceCollection services)
    {
        services.AddCommandHandler<SagaStartedByCommandHandler<StartShipInterception>>();
        services.AddIntegrationEventHandler<ShipDetectedIntegrationEventHandler>();

        services.AddSagaStarter<StartShipInterception, ShipInterceptionSaga>();

        // services.AddIntegrationEventHandler<SagaIntegrationEventHandler<ShipApproaching>>();
        // services.AddSagaEventHandler<ShipApproaching, ShipInterceptionSaga>();
    }
}

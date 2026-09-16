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
    /// <summary>
    /// Le proprieta' dello stato con cui un evento in ritardo ritrova il suo processo quando non puo'
    /// indirizzarlo per correlationId. Ognuna diventa un indice sulla collection delle saghe.
    /// </summary>
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

        // Ogni evento vuole due registrazioni, e servono entrambe: la prima lo fa arrivare dal bus, la
        // seconda lo consegna al processo. Dimenticare la prima non da' errore — l'evento
        // semplicemente non arriva mai, e il processo resta fermo sul gradino precedente.
        //
        // TODO: per ogni evento che il processo deve ascoltare, due righe.
        //
        //   services.AddIntegrationEventHandler<SagaIntegrationEventHandler<ShipApproaching>>();
        //   services.AddSagaEventHandler<ShipApproaching, ShipInterceptionSaga>();
        //
        // L'elenco completo di quello che la Terra manda sta in EVENTI.md, e quali servono a che
        // livello sta in REGOLE.md.
    }
}

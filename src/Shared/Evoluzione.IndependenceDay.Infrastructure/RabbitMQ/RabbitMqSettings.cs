namespace Evoluzione.IndependenceDay.Infrastructure.RabbitMQ;

public class RabbitMqSettings
{
    public required string Host { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string ExchangeCommandName { get; set; }
    public required string ExchangeEventName { get; set; }

    /// <summary>
    /// Il nome con cui questo servizio si presenta al broker. Diventa il prefisso delle sue code:
    /// due servizi con lo stesso ClientId si contendono gli stessi messaggi e ognuno ne vede metà.
    /// </summary>
    public required string ClientId { get; set; }
}

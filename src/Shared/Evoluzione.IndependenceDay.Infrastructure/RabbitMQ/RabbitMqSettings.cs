namespace Evoluzione.IndependenceDay.Infrastructure.RabbitMQ;

public class RabbitMqSettings
{
    public required string Host { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string ExchangeCommandName { get; set; }
    public required string ExchangeEventName { get; set; }

    public required string ClientId { get; set; }
}

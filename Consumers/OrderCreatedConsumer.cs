using mass_transit_bug.Models;
using MassTransit;

namespace mass_transit_bug.Consumers;

public sealed class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
    {
        _logger = logger;
    }
    
    public Task Consume(ConsumeContext<OrderCreated> context)
    {
        _logger.LogInformation("Consumned {@Message}", context.Message);
        return Task.CompletedTask;
    }
}
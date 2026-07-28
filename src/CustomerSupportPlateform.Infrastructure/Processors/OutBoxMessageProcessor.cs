using ConduitR.Abstractions;
using CustomerSupportPlateform.Domain.DDD;
using CustomerSupportPlateform.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CustomerSupportPlateform.Infrastructure.Processors;


public class OutBoxMessageProcessor(IServiceScopeFactory scopeFactory,ILogger<OutBoxMessageProcessor> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<OutBoxMessageProcessor> _logger = logger;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var scope = _scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {

                var outBoxMessages = await dbContext.OutBoxMessages
                    .Where(o => !o.IsProcessed)
                    .ToListAsync(stoppingToken);

                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                foreach (var outBoxMessage in outBoxMessages)
                {
                    var eventType = Type.GetType(outBoxMessage.EventType);
                    if (eventType == null)
                    {
                        _logger.LogWarning("Could not resolve Type:{Type}", outBoxMessage.EventType);
                        continue;
                    }
                    if (!typeof(IDomainEvent).IsAssignableFrom(eventType))
                    {
                        _logger.LogWarning("Type of {eventType} does not implement IDomainEvent", eventType);
                        continue;
                    }
                    var payload = JsonSerializer.Deserialize(outBoxMessage.Payload, eventType);
                    if (payload == null)
                    {
                        _logger.LogWarning("Could not deserialized payload:{Payload}", outBoxMessage.Payload);
                        continue;
                    }
                    _logger.LogInformation("Publishing event {eventType} with Id:{ID}", eventType, outBoxMessage.Id);
                    await mediator.Publish((dynamic)payload);
                    outBoxMessage.IsProcessed = true;
                    outBoxMessage.ProcessedAt = DateTime.UtcNow;
                    _logger.LogInformation("Successfully processed event:{EventType}", eventType);

                }

                await dbContext.SaveChangesAsync(stoppingToken);

            }catch(Exception ex)
            {
                _logger.LogError("Error occurred while Processing OutboxMessage");
                continue;
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }

        }
    }
}
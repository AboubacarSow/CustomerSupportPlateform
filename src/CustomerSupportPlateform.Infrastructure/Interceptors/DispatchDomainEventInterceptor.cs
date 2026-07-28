using ConduitR.Abstractions;
using CustomerSupportPlateform.Domain.DDD;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerSupportPlateform.Infrastructure.Interceptors;

public class DispatchDomainEventInterceptor(IServiceScopeFactory serviceScopeFactory) : SaveChangesInterceptor 
{
    private readonly IServiceScopeFactory _scopeFactory = serviceScopeFactory;

    // Because I want document being saved before my eventhandler starts its work
    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        await DispatchDomainEvent(eventData.Context, cancellationToken) ;
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
    
    private  async Task DispatchDomainEvent(DbContext? context,CancellationToken cancellationToken=default)
    {
        var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>() 
            ?? throw new ArgumentNullException("Mediator not instanciated");
        if (context == null)
            return;

        var entities = context
            .ChangeTracker.Entries<IHasDomainEvent>()
            .Where(e=>e.Entity.DomainEvents.Any())
            .ToList();

        var events = entities.SelectMany(e=>e.Entity.DomainEvents).ToList();

        entities.ForEach(e => e.Entity.ClearDomainEvents());


        foreach(var raisedevent in events)
        {
            await mediator.Publish((dynamic)raisedevent,cancellationToken);
        }
    }
}

using ConduitR.Abstractions;
using CustomerSupportPlateform.Domain.DDD;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace CustomerSupportPlateform.Infrastructure.Interceptors;

public class DispatchDomainEventInterceptor(IServiceScopeFactory serviceScopeFactory) : SaveChangesInterceptor 
{
    private readonly IServiceScopeFactory _scopeFactory = serviceScopeFactory;

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        DispatchDomainEvent(eventData.Context).GetAwaiter().GetResult();
        return base.SavingChanges(eventData, result);
    }
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        await DispatchDomainEvent(eventData.Context,cancellationToken);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private  async Task DispatchDomainEvent(DbContext? context,CancellationToken cancellationToken=default)
    {
        var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetService<IMediator>() 
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
            await mediator.Publish(raisedevent,cancellationToken);
        }
    }
}

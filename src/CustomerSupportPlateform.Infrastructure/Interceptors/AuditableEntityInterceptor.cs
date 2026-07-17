using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CustomerSupportPlateform.Infrastructure.Interceptors;

public class AuditableEntityInterceptor : SaveChangesInterceptor 
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateEntities(DbContext? context)
    {
        if (context == null)
            return;
        foreach(var entity in context.ChangeTracker.Entries<BaseEntity>())
        {
            var isAdded = entity.State == EntityState.Added;

            var isModified = entity.State == EntityState.Modified
                || entity.State == EntityState.Added
                || entity.HasChangedOwnedEntities();

            if(isAdded)
                entity.Entity.CreatedAt = DateTime.UtcNow;
            if(isModified)
                entity.Entity.LastUpdatedAt = DateTime.UtcNow;
        }
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added ||
            r.TargetEntry.State == EntityState.Modified));
}
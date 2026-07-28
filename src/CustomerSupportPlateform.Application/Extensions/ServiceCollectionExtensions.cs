using ConduitR.DependencyInjection;
using ConduitR.Validation.FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerSupportPlateform.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddCarter();
        services.AddConduit(options =>
        {
            options.AddHandlersFromAssemblies(typeof(AssemblyReference).Assembly);
            options.PublishStrategy = PublishStrategy.Parallel;

            options.AddBehavior(typeof(ValidationBehavior<,>));

        });
        services.AddConduitValidation(typeof(AssemblyReference).Assembly);
        return services;
    }
}
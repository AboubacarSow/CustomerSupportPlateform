using CustomerSupportPlateform.Application.Extensions;
using CustomerSupportPlateform.Infrastructure.Extensions;

namespace CustomerSupportPlateform.API.Extensions;


public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDependencies(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);
        services.AddApplication();
        return services;
    }
}
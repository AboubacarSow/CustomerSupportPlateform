using CustomerSupportPlateform.Infrastructure.ContentExtractors;
using CustomerSupportPlateform.Infrastructure.Embeddings;
using CustomerSupportPlateform.Infrastructure.Persistence;
using CustomerSupportPlateform.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerSupportPlateform.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
    IConfiguration configuration)
    {
        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        services.AddScoped<IBlobStorage, RailwayBucketStorage>();
        services.AddScoped<ITempStorageService,ITempStorageService>();
        services.AddScoped<IEmbeddingGenerator,OpenAiEmbeddingGenerator>();

        services.AddScoped<IContentExtractor, PdfExtractor>();
        services.AddScoped<IContentExtractor, DocxExtractor>();
        services.AddScoped<IContentExtractor, MarkDownExtractor>();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), 
                optionsBuilder => optionsBuilder.UseVector());
            
        });
        return services;
    }
}
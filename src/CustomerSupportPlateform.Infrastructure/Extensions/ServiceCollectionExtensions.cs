using CustomerSupportPlateform.Infrastructure.ChatCompletions;
using CustomerSupportPlateform.Infrastructure.Embeddings;
using CustomerSupportPlateform.Infrastructure.Ingestions;
using CustomerSupportPlateform.Infrastructure.Ingestions.ContentExtractors;
using CustomerSupportPlateform.Infrastructure.Persistence;
using CustomerSupportPlateform.Infrastructure.PromptPreparation;
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
        services.AddScoped<IContentCleaner,ContentCleaner>();
        services.AddScoped<IVectorSearchSimilarity, VectorSearchSimilarity>();

        services.AddScoped<IContentExtractor, PdfExtractor>();
        services.AddScoped<IContentExtractor, DocxExtractor>();
        services.AddScoped<IContentExtractor, MarkDownExtractor>();

        services.AddScoped<IChatCompletion,OllamaChatCompletion>();

        services.AddHttpClient("Ollama-Client", client =>
        {
            var endpoint = configuration["Ollama:Endpoint"];
            ArgumentException.ThrowIfNullOrEmpty(endpoint);
            client.BaseAddress = new Uri(endpoint);
            client.Timeout = TimeSpan.FromSeconds(1);
        });

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), 
                optionsBuilder => optionsBuilder.UseVector());
            
        });
        return services;
    }
}
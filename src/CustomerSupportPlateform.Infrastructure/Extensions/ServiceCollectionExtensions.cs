using Amazon.S3;
using CustomerSupportPlateform.Infrastructure.ChatCompletions;
using CustomerSupportPlateform.Infrastructure.Embeddings;
using CustomerSupportPlateform.Infrastructure.Ingestions;
using CustomerSupportPlateform.Infrastructure.Ingestions.ContentExtractors;
using CustomerSupportPlateform.Infrastructure.Interceptors;
using CustomerSupportPlateform.Infrastructure.Persistence;
using CustomerSupportPlateform.Infrastructure.Policies;
using CustomerSupportPlateform.Infrastructure.Processors;
using CustomerSupportPlateform.Infrastructure.PromptPreparation;
using CustomerSupportPlateform.Infrastructure.Storage;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerSupportPlateform.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
    IConfiguration configuration)
    {
        services.AddScoped<IApplicationDbContext>(serviceProvicer =>
            serviceProvicer.GetRequiredService<ApplicationDbContext>()
        );
        services.AddScoped<IBlobStorage, RailwayBucketStorage>();
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        services.AddAWSService<IAmazonS3>();
        services.AddScoped<ILocalStorage,LocalStorage>();
        services.AddTransient<IEmbeddingGenerator,OpenAiEmbeddingGenerator>();
        services.AddTransient<IEmbeddingGenerator,OllamaEmbeddingGenerator>();
        services.AddScoped<IContentCleaner,ContentCleaner>();
        services.AddScoped<IContentChunker,ContentChunker>();
        services.AddScoped<IVectorSearchSimilarity, VectorSearchSimilarity>();

        services.AddTransient<IContentExtractor, PdfExtractor>();
        services.AddTransient<IContentExtractor, DocxExtractor>();
        services.AddTransient<IContentExtractor, MarkDownExtractor>();

        services.AddTransient<IChatCompletion,OllamaChatCompletion>();
        services.AddTransient<IChatCompletion,OpenAiChatCompletion>();

        services.AddScoped<ISaveChangesInterceptor,DispatchDomainEventInterceptor>();

        services.AddHttpClient("Ollama-Client", client =>
        {
            var endpoint = configuration["Ollama:Endpoint"];
            ArgumentException.ThrowIfNullOrEmpty(endpoint);
            client.BaseAddress = new Uri(endpoint);
            client.Timeout = TimeSpan.FromMinutes(10);
        }).AddPolicyHandler(OllamaPolicy.ImmediatRetry);

        services.AddDbContext<ApplicationDbContext>((serviceProvider,options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), 
                optionsBuilder => optionsBuilder.UseVector());
            options.AddInterceptors(serviceProvider.GetRequiredService<ISaveChangesInterceptor>());
            
        });


        

        services.AddIdentityApiEndpoints<ApplicationUser>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedAccount=false;
            
            
        }).AddRoles<ApplicationRole>() 
            .AddApiEndpoints()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        

        services.AddAuthorization();


        services.AddHostedService<OutBoxMessageProcessor>();

        return services;
    }
}
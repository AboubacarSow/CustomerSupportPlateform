using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using webAdmin.Models;

namespace webAdmin.Services;


public interface IKnowledgeDocumentService
{
    Task<Guid> UploadDocumentAsync(
        string title,
        string? description,
        Language language,
        IBrowserFile file);

    Task<DocumentDto?> GetDocumentByIdAsync(Guid documentId);
}

public sealed class KnowledgeDocumentService(IHttpClientFactory httpClientFactory)
    : IKnowledgeDocumentService
{
    private readonly HttpClient _httpClient =
        httpClientFactory.CreateClient("ChatbotApi");

    public async Task<Guid> UploadDocumentAsync(
        string title,
        string? description,
        Language language,
        IBrowserFile file)
    {
        using var form = new MultipartFormDataContent();

        form.Add(new StringContent(title), "title");

        if (!string.IsNullOrWhiteSpace(description))
            form.Add(new StringContent(description), "description");

        form.Add(new StringContent(language.ToString()), "language");

        await using var stream = file.OpenReadStream(20 * 1024 * 1024);

        var fileContent = new StreamContent(stream);

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue(file.ContentType);

        form.Add(fileContent, "file", file.Name);

        var response = await _httpClient.PostAsync(
            "/knowledges",
            form);

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content.ReadFromJsonAsync<DocumentDto>();

        return result is null
            ? throw new InvalidOperationException(
                "The server returned an empty response.")
            : result.Id;
    }

    public async Task<DocumentDto?> GetDocumentByIdAsync(Guid documentId)
    {
        var response = await _httpClient.GetAsync(
            $"/knowledges/{documentId}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<DocumentDto>();
    }
}

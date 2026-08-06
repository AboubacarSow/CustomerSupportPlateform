using webAdmin.Models;

namespace webAdmin.Services;



public interface IChatCompletionService
{
    Task<string> GetMessageAsync(ChatRequestDto request);



}
public class ChatCompletionService(IHttpClientFactory httpClientFactory)
    : IChatCompletionService
{
    private readonly HttpClient _httpClient =
        httpClientFactory.CreateClient("ChatbotApi");

    public async Task<string> GetMessageAsync(ChatRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "/api/chat",
            request);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Chat request failed. Status Code: {response.StatusCode}");
        }

        var result = await response.Content
            .ReadFromJsonAsync<ChatCompletionResponse>();

        return result is null
            ? throw new InvalidOperationException(
                "The server returned an empty response.")
            : result.Message;
    }

    private sealed record ChatCompletionResponse(string Message);
}
namespace CustomerSupportPlateform.Infrastructure.ChatCompletions;

internal class OpenAiChatCompletion : IChatCompletion
{
    public ModelsEnvironment Environment => ModelsEnvironment.Production;

    public Task<string> RequestAsync(string context, string question)
    {
        throw new NotImplementedException();
    }
}
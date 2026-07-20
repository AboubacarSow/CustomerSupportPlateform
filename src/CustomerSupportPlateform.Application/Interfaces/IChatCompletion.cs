namespace CustomerSupportPlateform.Application.Interfaces;

public interface IChatCompletion
{
    ModelsEnvironment Environment {get;}
    Task<string> RequestAsync(List<string> context, string question);
}


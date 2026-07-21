namespace CustomerSupportPlateform.Application.Interfaces;

public interface IChatCompletion
{
    ModelsEnvironment Environment {get;}
    Task<string> RequestAsync(Guid SessionId,List<string> context, string question);
}


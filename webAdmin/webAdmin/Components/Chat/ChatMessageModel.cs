namespace webAdmin.Components.Chat;

public sealed record ChatMessageModel(
    ChatMessageRole Role,
    string Content);

public enum ChatMessageRole
{
    User,
    Assistant
}

namespace CustomerSupportPlateform.Domain.Entities;

public class ConversationMessage : BaseEntity
{
    public Guid SessionId { get; set; }
    public Session? Session { get; set; }
    public string Role { get; set; } = default!;
    public string Content { get; set; } = default!;

    private ConversationMessage(Guid sessionId,
        string role, string content)
    {
        SessionId = sessionId;
        Role = role;
        Content = content;
    }
    public static ConversationMessage CreateNew(Guid sessionId,
        string role, string content) => new(sessionId, role, content);
}




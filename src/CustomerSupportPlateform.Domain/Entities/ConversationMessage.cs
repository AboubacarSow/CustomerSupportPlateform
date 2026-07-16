namespace CustomerSupportPlateform.Domain.Entities;

public class ConversationMessage : BaseEntity
{
    public Guid SessionId { get; set; }
    public Session? Session { get; set; }
    public string Role { get; set; } = default!;
    public string Content { get; set; } = default!;
}




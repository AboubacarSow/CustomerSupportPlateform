namespace CustomerSupportPlateform.Domain.Entities;

public class Session : BaseEntity
{
    public List<ConversationMessage> Messages { get; set; } = [];
}




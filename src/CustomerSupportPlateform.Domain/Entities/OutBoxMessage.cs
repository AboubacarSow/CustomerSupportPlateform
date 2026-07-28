namespace CustomerSupportPlateform.Domain.Entities;

public class OutBoxMessage
{
    public Guid Id { get; set; }
    public string Payload { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public bool IsProcessed { get; set; }
    public string EventType { get; set; }
    public DateTime OccurredAt { get; set;  }

    public OutBoxMessage() { }
    public OutBoxMessage(string payload, string eventType)
    {
        Payload = payload;
        ProcessedAt = null;
        IsProcessed = false;
        EventType = eventType;
        OccurredAt = DateTime.UtcNow;
    }


}




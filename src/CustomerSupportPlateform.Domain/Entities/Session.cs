namespace CustomerSupportPlateform.Domain.Entities;

public class Session : BaseEntity 
{
    public void SetLastUpdatedAt()
    {
        LastUpdatedAt = DateTime.UtcNow;
    }
    private Session()
    {
        Id = Guid.NewGuid();    
    }
    private Session(Guid id)
    {
        Id = id;
    }
    public static Session CreateNew()=>new ();

    public static Session CreateNew(Guid id) => new(id);
}




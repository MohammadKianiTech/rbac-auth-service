namespace Evalify.Domain.Entities;

public class Evaluation : Entity
{
    private Evaluation(Guid id, Title title, Description description, DateRange duration, Guid createdBy) : base(id)
    {
        Title = title;
        Description = description;
        Duration = duration;
        CreatedBy = createdBy;
        Status = EvaluationStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }
    private Evaluation() { }
    public Title Title { get; private set; }
    public Description Description { get; private set; }
    public DateRange Duration { get; private set; }
    public EvaluationStatus Status { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public User? Creator { get; private set; }


    public static Evaluation Create(Title title, Description description, DateRange duration, Guid createdBy)
    {
        return new Evaluation(Guid.NewGuid(), title, description, duration, createdBy);
    }
    public void Update(Title title, Description description, DateRange duration)
    {
        if (Title.Value != title.Value)
        {
            Title = title;
        }

        if (Description.Value != description.Value)
        {
            Description = description;
        }

        Duration = duration;
    }
    public void Publish() => Status = EvaluationStatus.Active;
    public void Close() => Status = EvaluationStatus.Closed;
}
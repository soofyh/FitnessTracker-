namespace FitnessTracker.Core.Models;

public class FitnessGoal
{
    public Guid Id {get; set;}
    public string Title {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
    public DateTime StartDate {get; set;}
    public DateTime? TargetDate {get; set;}
    public bool IsCompleted {get; set;}
    public DateTime? CompletedAt {get; set;}
    public Guid UserId {get; set;}

    public User User {get; set;} = null!;
    public ICollection<ProgressPhoto> ProgressPhotos {get; set;} = List<ProgressPhoto>();
}
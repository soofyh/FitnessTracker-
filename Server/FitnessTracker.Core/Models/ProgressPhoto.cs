namespace FitnessTracker.Core.Models;

public class ProgressPhoto
{
    public Guid Id {get; set;}
    public string FileName {get; set;} = string.Empty;
    public string ContentType {get; set;} = string.Empty;
    public long Size {get; set;}
    public string StoragePath {get; set;} = string.Empty;
    public bool IsBeforePhoto {get; set;}
    public DateTime UploadedAt {get; set;} = DateTime.UtcNow;
    public Guid GoalId {get; set;}

    public FitnessGoal Goal {get; set;} = null!;
}
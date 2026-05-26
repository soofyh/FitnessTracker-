using FitnessTracker.Core.DTO.Progress;

namespace FitnessTracker.Core.DTO.Goals;

public class GoalResponse
{
    public Guid Id {get; set;}
    public string Title {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
    public DateTime StartDate {get; set;}
    public DateTime? TargetDate {get; set;}
    public bool IsCompleted {get; set;}
    public DateTime? CompletedAt {get; set;}
    public List<ProgressPhotoResponse> BeforePhotos {get; set;} = new();
    public List<ProgressPhotoResponse> AfterPhotos {get; set;} = new();
}
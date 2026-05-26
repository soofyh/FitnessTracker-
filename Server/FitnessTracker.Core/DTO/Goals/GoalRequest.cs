namespace FitnessTracker.Core.DTO;

public class GoalRequest
{
    public string Title {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
    public DateTime StartDate {get; set;}
    public DateTime? TargetDate {get; set;}
}
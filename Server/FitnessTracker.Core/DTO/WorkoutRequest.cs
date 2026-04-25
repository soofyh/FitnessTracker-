namespace FitnessTracker.Core.DTO;

public class WorkoutRequest
{
    public string Name {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
    public int DurationMinutes {get; set;}
    public int CaloriesBurned {get; set;}
    public DateTime Date {get; set;}
}
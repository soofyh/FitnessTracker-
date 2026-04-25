namespace FitnessTracker.Core.Models;

public class Workout
{
    public Guid Id {get; set;}
    public string Name {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
    public int DurationMinutes {get; set;}
    public int CaloriesBurned {get; set;}
    public DateTime Date {get; set;}
    public Guid UserId {get; set;}

    public User User {get; set;} = null!;
}
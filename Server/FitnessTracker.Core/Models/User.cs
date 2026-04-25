namespace FitnessTracker.Core.Models;

public class User
{
    public Guid Id {get; set;}
    public string Username {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
    public string PasswordHash {get; set;} = string.Empty;
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public ICollection<Workout> Workouts {get; set;} = new List<Workout>();
    public ICollection<UserFile> Files {get; set;} = new List<UserFile>();
}
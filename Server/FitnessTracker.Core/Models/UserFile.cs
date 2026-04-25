namespace FitnessTracker.Core.Models;

public class UserFile
{
    public Guid Id {get; set;}
    public string FileName {get; set;} = string.Empty;
    public string ContentType {get; set;} = string.Empty;
    public long Size {get; set;}
    public string StoragePath {get; set;} = string.Empty;
    public DateTime UploadedAt {get; set;} = DateTime.UtcNow;
    public Guid UserId {get; set;}

    public User User {get; set;} = null!;
}
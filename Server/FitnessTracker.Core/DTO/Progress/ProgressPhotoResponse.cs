namespace FitnessTracker.Core.DTO.Progress;

public class ProgressPhotoResponse
{
    public Guid Id {get; set;}
    public string FileName {get; set;} = string.Empty;
    public string ContentType {get; set;} = string.Empty;
    public long Size {get; set;}
    public bool IsBeforePhoto {get; set;}
    public DateTime UploadedAt {get; set;}
}
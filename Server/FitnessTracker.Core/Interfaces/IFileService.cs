using FitnessTracker.Core.DTO.Files;

namespace FitnessTracker.Core.Interfaces;

public interface IFileService
{
    Task<IEnumerable<FileResponse>> GetAllAsync(Guid userId);
    Task<FileResponse> UploadAsync(Stream fileStream, string FileName, string ContentType, long Size, Guid userId);
    Task<(Stream fileStream, string ContentType, string FileName)?> DownloadAsync(Guid fileId, Guid userId);
    Task<bool> DeleteAsync(Guid fileId, Guid userId);
}
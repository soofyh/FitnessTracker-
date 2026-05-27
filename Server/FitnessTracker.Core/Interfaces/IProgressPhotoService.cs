using FitnessTracker.Core.DTO.Progrss;

namespace FitnessTracker.Core.Interfaces;

public interface IPrigressPhotoService
{
    Task<IEnumerable<ProgressPhotoResponse>> GetPhotosGoalAsync(Guid goalId, Guid userId);
    Task<ProgressPhotoResponse> UploadPhotoAsync(Stream fileStream, string fileName, string contentType, long size, Guid goalId, Guid userId, bool isBeforePhoto);
    Task<(Stream FileStream, string ContentType, string FileName)?> DownloadPhotoAsync(Guid photoId, Guid userId);
    Task<bool> DeletePhotoAsync(Guid photoId, Guid userId);
}
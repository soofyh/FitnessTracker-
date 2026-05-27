using Microsoft.EntityFrameworkCore;
using FitnessTracker.Core.DTO.Progress;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Core.Models;
using FitnessTracker.Infrastructure.Data;

namespace FitnessTracker.Infrastructure.Services;

public class ProgressPhotoService : IProgressPhotoService
{
    private readonly AppDbContext _context;
    private readonly string _storagePath;
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };
    private const long MaxFileSize = 5 * 1024 * 1024;
    
    public ProgressPhotoService(AppDbContext context)
    {
        _context = context;
        _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "ProgressPhotos");
        Directory.CreateDirectory(_storagePath);
    }
    
    public async Task<IEnumerable<ProgressPhotoResponse>> GetPhotosForGoalAsync(Guid goalId, Guid userId)
    {
        var goal = await _context.FitnessGoals
            .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);
        if (goal == null) throw new UnauthorizedAccessException("Цель не найдена");
        
        return await _context.ProgressPhotos
            .Where(p => p.GoalId == goalId)
            .OrderByDescending(p => p.UploadedAt)
            .Select(p => new ProgressPhotoResponse
            {
                Id = p.Id, FileName = p.FileName, ContentType = p.ContentType,
                Size = p.Size, IsBeforePhoto = p.IsBeforePhoto, UploadedAt = p.UploadedAt
            }).ToListAsync();
    }
    
    public async Task<ProgressPhotoResponse> UploadPhotoAsync(
        Stream fileStream, string fileName, string contentType, long size,
        Guid goalId, Guid userId, bool isBeforePhoto)
    {
        var goal = await _context.FitnessGoals
            .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);
        if (goal == null) throw new UnauthorizedAccessException("Цель не найдена");
        
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            throw new InvalidOperationException("Разрешены только JPG и PNG");
        if (size > MaxFileSize)
            throw new InvalidOperationException($"Максимальный размер фото: {MaxFileSize / 1024 / 1024} MB");
        
        var photoId = Guid.NewGuid();
        var storageFileName = $"{photoId}{extension}";
        var filePath = Path.Combine(_storagePath, storageFileName);
        
        using (var fs = new FileStream(filePath, FileMode.Create))
            await fileStream.CopyToAsync(fs);
        
        var photo = new ProgressPhoto
        {
            Id = photoId, FileName = fileName, ContentType = contentType,
            Size = size, StoragePath = filePath, IsBeforePhoto = isBeforePhoto,
            GoalId = goalId, UploadedAt = DateTime.UtcNow
        };
        
        _context.ProgressPhotos.Add(photo);
        await _context.SaveChangesAsync();
        
        return new ProgressPhotoResponse
        {
            Id = photo.Id, FileName = photo.FileName, ContentType = photo.ContentType,
            Size = photo.Size, IsBeforePhoto = photo.IsBeforePhoto, UploadedAt = photo.UploadedAt
        };
    }
    
    public async Task<(Stream FileStream, string ContentType, string FileName)?> DownloadPhotoAsync(Guid photoId, Guid userId)
    {
        var photo = await _context.ProgressPhotos
            .Include(p => p.Goal)
            .FirstOrDefaultAsync(p => p.Id == photoId && p.Goal.UserId == userId);
        if (photo == null || !File.Exists(photo.StoragePath)) return null;
        
        return (new FileStream(photo.StoragePath, FileMode.Open, FileAccess.Read), photo.ContentType, photo.FileName);
    }
    
    public async Task<bool> DeletePhotoAsync(Guid photoId, Guid userId)
    {
        var photo = await _context.ProgressPhotos
            .Include(p => p.Goal)
            .FirstOrDefaultAsync(p => p.Id == photoId && p.Goal.UserId == userId);
        if (photo == null) return false;
        
        if (File.Exists(photo.StoragePath)) File.Delete(photo.StoragePath);
        
        _context.ProgressPhotos.Remove(photo);
        await _context.SaveChangesAsync();
        return true;
    }
}
using Microsoft.EntityFrameworkCore;
using FitnessTracker.Core.DTO.Files;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Core.Models;
using FitnessTracker.Infrastructure.Data;

namespace FitnessTracker.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly AppDbContext _context;
    private readonly string _storagePath;
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf", ".txt", ".doc", ".docx" };
    private const long MaxFileSize = 10 * 1024 * 1024;
    
    public FileService(AppDbContext context)
    {
        _context = context;
        _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        Directory.CreateDirectory(_storagePath);
    }
    
    public async Task<IEnumerable<FileResponse>> GetAllAsync(Guid userId)
    {
        return await _context.UserFiles
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.UploadedAt)
            .Select(f => new FileResponse
            {
                Id = f.Id, FileName = f.FileName,
                ContentType = f.ContentType, Size = f.Size, UploadedAt = f.UploadedAt
            })
            .ToListAsync();
    }
    
    public async Task<FileResponse> UploadAsync(Stream fileStream, string fileName, string contentType, long size, Guid userId)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            throw new InvalidOperationException($"Тип файла '{extension}' не разрешен");
        if (size > MaxFileSize)
            throw new InvalidOperationException($"Размер файла превышает {MaxFileSize / 1024 / 1024} MB");
        
        var fileId = Guid.NewGuid();
        var storageFileName = $"{fileId}{extension}";
        var filePath = Path.Combine(_storagePath, storageFileName);
        
        using (var fs = new FileStream(filePath, FileMode.Create))
            await fileStream.CopyToAsync(fs);
        
        var userFile = new UserFile
        {
            Id = fileId, FileName = fileName, ContentType = contentType,
            Size = size, StoragePath = filePath, UserId = userId, UploadedAt = DateTime.UtcNow
        };
        
        _context.UserFiles.Add(userFile);
        await _context.SaveChangesAsync();
        
        return new FileResponse
        {
            Id = userFile.Id, FileName = userFile.FileName,
            ContentType = userFile.ContentType, Size = userFile.Size, UploadedAt = userFile.UploadedAt
        };
    }
    
    public async Task<(Stream FileStream, string ContentType, string FileName)?> DownloadAsync(Guid fileId, Guid userId)
    {
        var file = await _context.UserFiles
            .FirstOrDefaultAsync(f => f.Id == fileId && f.UserId == userId);
        if (file == null || !System.IO.File.Exists(file.StoragePath)) return null;
        
        return (new FileStream(file.StoragePath, FileMode.Open, FileAccess.Read), file.ContentType, file.FileName);
    }
    
    public async Task<bool> DeleteAsync(Guid fileId, Guid userId)
    {
        var file = await _context.UserFiles
            .FirstOrDefaultAsync(f => f.Id == fileId && f.UserId == userId);
        if (file == null) return false;
        
        if (System.IO.File.Exists(file.StoragePath))
            System.IO.File.Delete(file.StoragePath);
        
        _context.UserFiles.Remove(file);
        await _context.SaveChangesAsync();
        return true;
    }
}
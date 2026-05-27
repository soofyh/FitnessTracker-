using Microsoft.EntityFrameworkCore;
using FitnessTracker.Core.DTO.Goals;
using FitnessTracker.Core.DTO.Progress;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Core.Models;
using FitnessTracker.Infrastructure.Data;

namespace FitnessTracker.Infrastructure.Services;

public class GoalService : IGoalService
{
    private readonly AppDbContext _context;
    
    public GoalService(AppDbContext context) => _context = context;
    
    public async Task<IEnumerable<GoalResponse>> GetAllAsync(Guid userId)
    {
        return await _context.FitnessGoals
            .Where(g => g.UserId == userId)
            .Include(g => g.ProgressPhotos)
            .OrderByDescending(g => g.StartDate)
            .ToListAsync()
            .ContinueWith(t => t.Result.Select(g => MapToResponse(g)));
    }
    
    public async Task<GoalResponse?> GetByIdAsync(Guid id, Guid userId)
    {
        var goal = await _context.FitnessGoals
            .Include(g => g.ProgressPhotos)
            .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);
        return goal == null ? null : MapToResponse(goal);
    }
    
    public async Task<GoalResponse> CreateAsync(GoalRequest request, Guid userId)
    {
        var goal = new FitnessGoal
        {
            Id = Guid.NewGuid(), Title = request.Title,
            Description = request.Description, StartDate = request.StartDate,
            TargetDate = request.TargetDate, UserId = userId
        };
        
        _context.FitnessGoals.Add(goal);
        await _context.SaveChangesAsync();
        return MapToResponse(goal);
    }
    
    public async Task<GoalResponse?> UpdateAsync(Guid id, GoalRequest request, Guid userId)
    {
        var goal = await _context.FitnessGoals
            .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);
        if (goal == null) return null;
        
        goal.Title = request.Title; goal.Description = request.Description;
        goal.StartDate = request.StartDate; goal.TargetDate = request.TargetDate;
        
        await _context.SaveChangesAsync();
        return await GetByIdAsync(goal.Id, userId);
    }
    
    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var goal = await _context.FitnessGoals
            .Include(g => g.ProgressPhotos)
            .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);
        if (goal == null) return false;
        
        foreach (var photo in goal.ProgressPhotos)
        {
            if (File.Exists(photo.StoragePath))
                File.Delete(photo.StoragePath);
        }
        
        _context.FitnessGoals.Remove(goal);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<GoalResponse?> CompleteGoalAsync(Guid id, Guid userId)
    {
        var goal = await _context.FitnessGoals
            .Include(g => g.ProgressPhotos)
            .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);
        if (goal == null) return null;
        
        var hasBefore = goal.ProgressPhotos.Any(p => p.IsBeforePhoto);
        var hasAfter = goal.ProgressPhotos.Any(p => !p.IsBeforePhoto);
        
        if (!hasBefore || !hasAfter)
            throw new InvalidOperationException("Для завершения цели нужны фото ДО и ПОСЛЕ");
        
        goal.IsCompleted = true;
        goal.CompletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        
        return MapToResponse(goal);
    }
    
    private GoalResponse MapToResponse(FitnessGoal goal) => new()
    {
        Id = goal.Id, Title = goal.Title, Description = goal.Description,
        StartDate = goal.StartDate, TargetDate = goal.TargetDate,
        IsCompleted = goal.IsCompleted, CompletedAt = goal.CompletedAt,
        BeforePhotos = goal.ProgressPhotos.Where(p => p.IsBeforePhoto)
            .Select(p => new ProgressPhotoResponse
            {
                Id = p.Id, FileName = p.FileName, ContentType = p.ContentType,
                Size = p.Size, IsBeforePhoto = true, UploadedAt = p.UploadedAt
            }).ToList(),
        AfterPhotos = goal.ProgressPhotos.Where(p => !p.IsBeforePhoto)
            .Select(p => new ProgressPhotoResponse
            {
                Id = p.Id, FileName = p.FileName, ContentType = p.ContentType,
                Size = p.Size, IsBeforePhoto = false, UploadedAt = p.UploadedAt
            }).ToList()
    };
}
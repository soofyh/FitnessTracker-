using Microsoft.EntityFrameworkCore;
using FitnessTracker.Core.DTO.Workouts;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Core.Models;
using FitnessTracker.Infrastructure.Data;

namespace FitnessTracker.Infrastructure.Services;

public class WorkoutService : IWorkoutService
{
    private readonly AppDbContext _context;
    
    public WorkoutService(AppDbContext context) => _context = context;
    
    public async Task<IEnumerable<WorkoutResponse>> GetAllAsync(Guid userId)
    {
        return await _context.Workouts
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.Date)
            .Select(w => MapToResponse(w))
            .ToListAsync();
    }
    
    public async Task<WorkoutResponse?> GetByIdAsync(Guid id, Guid userId)
    {
        var workout = await _context.Workouts
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
        return workout == null ? null : MapToResponse(workout);
    }
    
    public async Task<WorkoutResponse> CreateAsync(WorkoutRequest request, Guid userId)
    {
        var workout = new Workout
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            DurationMinutes = request.DurationMinutes,
            CaloriesBurned = request.CaloriesBurned,
            Date = request.Date,
            UserId = userId
        };
        
        _context.Workouts.Add(workout);
        await _context.SaveChangesAsync();
        return MapToResponse(workout);
    }
    
    public async Task<WorkoutResponse?> UpdateAsync(Guid id, WorkoutRequest request, Guid userId)
    {
        var workout = await _context.Workouts
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
        if (workout == null) return null;
        
        workout.Name = request.Name;
        workout.Description = request.Description;
        workout.DurationMinutes = request.DurationMinutes;
        workout.CaloriesBurned = request.CaloriesBurned;
        workout.Date = request.Date;
        
        await _context.SaveChangesAsync();
        return MapToResponse(workout);
    }
    
    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var workout = await _context.Workouts
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
        if (workout == null) return false;
        
        _context.Workouts.Remove(workout);
        await _context.SaveChangesAsync();
        return true;
    }
    
    private static WorkoutResponse MapToResponse(Workout workout) => new()
    {
        Id = workout.Id,
        Name = workout.Name,
        Description = workout.Description,
        DurationMinutes = workout.DurationMinutes,
        CaloriesBurned = workout.CaloriesBurned,
        Date = workout.Date
    };
}
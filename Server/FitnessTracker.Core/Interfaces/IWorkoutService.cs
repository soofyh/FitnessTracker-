using FitnessTracker.Core.DTO.Workouts;

namespace FitnessTracker.Core.Interfaces;

public interface IWorkoutService
{
    Task<IEnumerable<WorkoutResponse>> GetAllAsync(Guid userId);
    Task<WorkoutResponse?> GetByIdAsync(Guid id, Guid userId);
    Task<WorkoutResponse> CreateAsync(WorkoutRequest request, Guid userId);
    Task<WorkoutResponse?> UpdateAsync(Guid id, WorkoutRequest request, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
}
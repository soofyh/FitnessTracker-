using FitnessTracker.Core.DTO.Goals;

namespace FitnessTracker.Core.Interfaces;

public interface IGoalService
{
    Task<IEnumerable<GoalResponse>> GetAllAsync(Guid userId);
    Task<GoalResponse?> GetByIdAsync(Guid id, Guid userId);
    Task<GoalResponse> CreateAsync(GoalRequest request, Guid userId);
    Task<GoalResponse?> UpdateAsync(Guid id, GoalRequest request, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
    Task<GoalResponse?> CompleteGoalAsync(Guid id, Guid userId);
}
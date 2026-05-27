using Microsoft.EntityFrameworkCore;
using FitnessTracker.Core.DTO.Workouts;
using FitnessTracker.Infrastructure.Data;
using FitnessTracker.Infrastructure.Services;

namespace FitnessTracker.Tests;

public class WorkoutServiceTests
{
    private readonly AppDbContext _context;
    private readonly WorkoutService _service;
    private readonly Guid _userId = Guid.NewGuid();
    
    public WorkoutServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
            .Options;
        _context = new AppDbContext(options);
        _service = new WorkoutService(_context);
    }
    
    [Fact]
    public async Task CreateWorkout_ReturnsCreatedWorkout()
    {
        var request = new WorkoutRequest
        {
            Name = "Утренняя пробежка", Description = "5 км", DurationMinutes = 30,
            CaloriesBurned = 300, Date = DateTime.Today
        };
        var result = await _service.CreateAsync(request, _userId);
        
        Assert.Equal("Утренняя пробежка", result.Name);
        Assert.Equal(300, result.CaloriesBurned);
    }
    
    [Fact]
    public async Task DeleteWorkout_RemovesWorkout()
    {
        var created = await _service.CreateAsync(new WorkoutRequest
        {
            Name = "На удаление", Description = "", DurationMinutes = 5,
            CaloriesBurned = 50, Date = DateTime.Today
        }, _userId);
        
        var result = await _service.DeleteAsync(created.Id, _userId);
        Assert.True(result);
    }
}
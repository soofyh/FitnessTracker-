using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitnessTracker.Core.DTO.Workouts;
using FitnessTracker.Core.Interfaces;

namespace FitnessTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkoutsController : ControllerBase
{
    private readonly IWorkoutService _workoutService;
    public WorkoutsController(IWorkoutService workoutService) => _workoutService = workoutService;
    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var workouts = await _workoutService.GetAllAsync(GetUserId());
        return Ok(workouts);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var workout = await _workoutService.GetByIdAsync(id, GetUserId());
        return workout == null ? NotFound() : Ok(workout);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WorkoutRequest request)
    {
        var workout = await _workoutService.CreateAsync(request, GetUserId());
        return CreatedAtAction(nameof(GetById), new { id = workout.Id }, workout);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] WorkoutRequest request)
    {
        var workout = await _workoutService.UpdateAsync(id, request, GetUserId());
        return workout == null ? NotFound() : Ok(workout);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _workoutService.DeleteAsync(id, GetUserId());
        return result ? NoContent() : NotFound();
    }
}
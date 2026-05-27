using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitnessTracker.Core.DTO.Goals;
using FitnessTracker.Core.Interfaces;

namespace FitnessTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GoalsController : ControllerBase
{
    private readonly IGoalService _goalService;
    public GoalsController(IGoalService goalService) => _goalService = goalService;
    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var goals = await _goalService.GetAllAsync(GetUserId());
        return Ok(goals);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var goal = await _goalService.GetByIdAsync(id, GetUserId());
        return goal == null ? NotFound() : Ok(goal);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GoalRequest request)
    {
        var goal = await _goalService.CreateAsync(request, GetUserId());
        return CreatedAtAction(nameof(GetById), new { id = goal.Id }, goal);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] GoalRequest request)
    {
        var goal = await _goalService.UpdateAsync(id, request, GetUserId());
        return goal == null ? NotFound() : Ok(goal);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _goalService.DeleteAsync(id, GetUserId());
        return result ? NoContent() : NotFound();
    }
    
    [HttpPost("{id}/complete")]
    public async Task<IActionResult> Complete(Guid id)
    {
        try
        {
            var goal = await _goalService.CompleteGoalAsync(id, GetUserId());
            return Ok(goal);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
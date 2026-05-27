using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitnessTracker.Core.Interfaces;

namespace FitnessTracker.API.Controllers;

[ApiController]
[Route("api/goals/{goalId}/photos")]
[Authorize]
public class ProgressPhotosController : ControllerBase
{
    private readonly IProgressPhotoService _photoService;
    public ProgressPhotosController(IProgressPhotoService photoService) => _photoService = photoService;
    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    [HttpPost]
    public async Task<IActionResult> Upload(Guid goalId, IFormFile file, [FromQuery] bool isBeforePhoto = true)
    {
        try
        {
            using var stream = file.OpenReadStream();
            var result = await _photoService.UploadPhotoAsync(
                stream, file.FileName, file.ContentType, file.Length,
                goalId, GetUserId(), isBeforePhoto);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return NotFound(new { message = "Цель не найдена" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpGet("{photoId}")]
    public async Task<IActionResult> Download(Guid goalId, Guid photoId)
    {
        var result = await _photoService.DownloadPhotoAsync(photoId, GetUserId());
        if (result == null) return NotFound();
        return File(result.Value.FileStream, result.Value.ContentType, result.Value.FileName);
    }
    
    [HttpDelete("{photoId}")]
    public async Task<IActionResult> Delete(Guid goalId, Guid photoId)
    {
        var result = await _photoService.DeletePhotoAsync(photoId, GetUserId());
        return result ? NoContent() : NotFound();
    }
}
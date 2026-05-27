using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitnessTracker.Core.Interfaces;

namespace FitnessTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;
    public FilesController(IFileService fileService) => _fileService = fileService;
    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var files = await _fileService.GetAllAsync(GetUserId());
        return Ok(files);
    }
    
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        try
        {
            using var stream = file.OpenReadStream();
            var response = await _fileService.UploadAsync(
                stream, file.FileName, file.ContentType, file.Length, GetUserId());
            return CreatedAtAction(nameof(GetAll), null, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(Guid id)
    {
        var result = await _fileService.DownloadAsync(id, GetUserId());
        if (result == null) return NotFound();
        return File(result.Value.FileStream, result.Value.ContentType, result.Value.FileName);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _fileService.DeleteAsync(id, GetUserId());
        return result ? NoContent() : NotFound();
    }
}
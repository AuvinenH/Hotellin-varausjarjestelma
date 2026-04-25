using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Contracts.Images;
using Microsoft.AspNetCore.Mvc;

namespace HotelLakeview.Api.Controllers;

[ApiController]
[Route("api/rooms/{roomId:guid}/images")]
public class RoomImagesController : ControllerBase
{
    private readonly IRoomImageService _roomImageService;

    public RoomImagesController(IRoomImageService roomImageService)
    {
        _roomImageService = roomImageService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoomImageDto>>> GetAll(Guid roomId, CancellationToken cancellationToken)
    {
        var images = await _roomImageService.GetByRoomIdAsync(roomId, cancellationToken);
        return Ok(images);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<RoomImageDto>> Upload(Guid roomId, IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null)
        {
            throw new ArgumentException("Image file is required.");
        }

        await using var stream = file.OpenReadStream();
        var image = await _roomImageService.UploadAsync(
            roomId,
            file.FileName,
            file.ContentType,
            file.Length,
            stream,
            cancellationToken);

        return CreatedAtAction(nameof(GetFile), new { roomId, imageId = image.Id }, image);
    }

    [HttpGet("{imageId:guid}/file")]
    public async Task<IActionResult> GetFile(Guid roomId, Guid imageId, CancellationToken cancellationToken)
    {
        var image = await _roomImageService.OpenImageAsync(roomId, imageId, cancellationToken);
        return File(image.Content, image.ContentType, image.FileName);
    }

    [HttpDelete("{imageId:guid}")]
    public async Task<IActionResult> Delete(Guid roomId, Guid imageId, CancellationToken cancellationToken)
    {
        await _roomImageService.DeleteAsync(roomId, imageId, cancellationToken);
        return NoContent();
    }
}

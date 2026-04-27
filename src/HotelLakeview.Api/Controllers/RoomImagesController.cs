using HotelLakeview.Application.Contracts.Images;
using HotelLakeview.Application.Common;
using HotelLakeview.Application.CQRS.RoomImages;
using HotelLakeview.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HotelLakeview.Api.Controllers;

[ApiController]
[Route("api/rooms/{roomId:guid}/images")]
public class RoomImagesController : ControllerBase
{
    private readonly ISender _sender;

    public RoomImagesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid roomId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetRoomImagesQuery(roomId), cancellationToken);
        if (result.IsFailure)
        {
            return this.ToProblem(result.Error!);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(Guid roomId, IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null)
        {
            return this.ToProblem(ResultError.Validation("room_image.file_required", "Image file is required."));
        }

        await using var stream = file.OpenReadStream();
        var result = await _sender.Send(
            new UploadRoomImageCommand(roomId, file.FileName, file.ContentType, file.Length, stream),
            cancellationToken);

        if (result.IsFailure)
        {
            return this.ToProblem(result.Error!);
        }

        return CreatedAtAction(nameof(GetFile), new { roomId, imageId = result.Value.Id }, result.Value);
    }

    [HttpGet("{imageId:guid}/file")]
    public async Task<IActionResult> GetFile(Guid roomId, Guid imageId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new OpenRoomImageQuery(roomId, imageId), cancellationToken);
        return result.IsSuccess
            ? File(result.Value.Content, result.Value.ContentType, result.Value.FileName)
            : this.ToProblem(result.Error!);
    }

    [HttpDelete("{imageId:guid}")]
    public async Task<IActionResult> Delete(Guid roomId, Guid imageId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteRoomImageCommand(roomId, imageId), cancellationToken);
        return result.IsSuccess ? NoContent() : this.ToProblem(result.Error!);
    }
}

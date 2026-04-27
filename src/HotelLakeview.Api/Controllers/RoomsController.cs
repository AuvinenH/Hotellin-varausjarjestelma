using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Contracts.Rooms;
using HotelLakeview.Api.Extensions;
using HotelLakeview.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace HotelLakeview.Api.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _roomService.GetAllAsync(cancellationToken);
        if (result.IsFailure)
        {
            return this.ToProblem(result.Error!);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _roomService.GetByIdAsync(id, cancellationToken);
        if (result.IsFailure)
        {
            return this.ToProblem(result.Error!);
        }

        return Ok(result.Value);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable(
        [FromQuery] DateOnly checkInDate,
        [FromQuery] DateOnly checkOutDate,
        [FromQuery] int guestCount,
        [FromQuery] RoomCategory? category,
        CancellationToken cancellationToken)
    {
        var result = await _roomService.GetAvailableAsync(
            checkInDate,
            checkOutDate,
            guestCount,
            category,
            cancellationToken);

        if (result.IsFailure)
        {
            return this.ToProblem(result.Error!);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoomRequest request, CancellationToken cancellationToken)
    {
        var result = await _roomService.CreateAsync(request, cancellationToken);
        if (result.IsFailure)
        {
            return this.ToProblem(result.Error!);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoomRequest request, CancellationToken cancellationToken)
    {
        var result = await _roomService.UpdateAsync(id, request, cancellationToken);
        if (result.IsFailure)
        {
            return this.ToProblem(result.Error!);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _roomService.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : this.ToProblem(result.Error!);
    }
}

using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Contracts.Rooms;
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
    public async Task<ActionResult<IReadOnlyList<RoomDto>>> GetAll(CancellationToken cancellationToken)
    {
        var rooms = await _roomService.GetAllAsync(cancellationToken);
        return Ok(rooms);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoomDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var room = await _roomService.GetByIdAsync(id, cancellationToken);
        return Ok(room);
    }

    [HttpGet("available")]
    public async Task<ActionResult<IReadOnlyList<RoomDto>>> GetAvailable(
        [FromQuery] DateOnly checkInDate,
        [FromQuery] DateOnly checkOutDate,
        [FromQuery] int guestCount,
        [FromQuery] RoomCategory? category,
        CancellationToken cancellationToken)
    {
        var availableRooms = await _roomService.GetAvailableAsync(
            checkInDate,
            checkOutDate,
            guestCount,
            category,
            cancellationToken);

        return Ok(availableRooms);
    }

    [HttpPost]
    public async Task<ActionResult<RoomDto>> Create([FromBody] CreateRoomRequest request, CancellationToken cancellationToken)
    {
        var createdRoom = await _roomService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = createdRoom.Id }, createdRoom);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RoomDto>> Update(Guid id, [FromBody] UpdateRoomRequest request, CancellationToken cancellationToken)
    {
        var updatedRoom = await _roomService.UpdateAsync(id, request, cancellationToken);
        return Ok(updatedRoom);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _roomService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

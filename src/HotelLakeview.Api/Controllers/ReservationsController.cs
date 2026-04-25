using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Contracts.Reservations;
using Microsoft.AspNetCore.Mvc;

namespace HotelLakeview.Api.Controllers;

[ApiController]
[Route("api/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ReservationDto>>> GetAll(CancellationToken cancellationToken)
    {
        var reservations = await _reservationService.GetAllAsync(cancellationToken);
        return Ok(reservations);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReservationDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var reservation = await _reservationService.GetByIdAsync(id, cancellationToken);
        return Ok(reservation);
    }

    [HttpPost]
    public async Task<ActionResult<ReservationDto>> Create([FromBody] CreateReservationRequest request, CancellationToken cancellationToken)
    {
        var createdReservation = await _reservationService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = createdReservation.Id }, createdReservation);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ReservationDto>> Update(Guid id, [FromBody] UpdateReservationRequest request, CancellationToken cancellationToken)
    {
        var updatedReservation = await _reservationService.UpdateAsync(id, request, cancellationToken);
        return Ok(updatedReservation);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        await _reservationService.CancelAsync(id, cancellationToken);
        return NoContent();
    }
}

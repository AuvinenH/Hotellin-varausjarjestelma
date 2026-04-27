using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Contracts.Reservations;
using HotelLakeview.Api.Extensions;
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
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _reservationService.GetAllAsync(cancellationToken);
        if (result.IsFailure)
        {
            return this.ToProblem(result.Error!);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _reservationService.GetByIdAsync(id, cancellationToken);
        if (result.IsFailure)
        {
            return this.ToProblem(result.Error!);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationRequest request, CancellationToken cancellationToken)
    {
        var result = await _reservationService.CreateAsync(request, cancellationToken);
        if (result.IsFailure)
        {
            return this.ToProblem(result.Error!);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReservationRequest request, CancellationToken cancellationToken)
    {
        var result = await _reservationService.UpdateAsync(id, request, cancellationToken);
        if (result.IsFailure)
        {
            return this.ToProblem(result.Error!);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await _reservationService.CancelAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : this.ToProblem(result.Error!);
    }
}

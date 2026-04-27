using HotelLakeview.Application.CQRS.Reports;
using HotelLakeview.Application.Contracts.Reports;
using HotelLakeview.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HotelLakeview.Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly ISender _sender;

    public ReportsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("occupancy")]
    public async Task<IActionResult> GetOccupancy(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetOccupancyReportQuery(startDate, endDate), cancellationToken);
        if (result.IsFailure)
        {
            return this.ToProblem(result.Error!);
        }

        return Ok(result.Value);
    }

    [HttpGet("monthly-revenue")]
    public async Task<IActionResult> GetMonthlyRevenue(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetMonthlyRevenueReportQuery(startDate, endDate), cancellationToken);
        if (result.IsFailure)
        {
            return this.ToProblem(result.Error!);
        }

        return Ok(result.Value);
    }

    [HttpGet("popular-room-types")]
    public async Task<IActionResult> GetPopularRoomTypes(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetPopularRoomTypesReportQuery(startDate, endDate), cancellationToken);
        if (result.IsFailure)
        {
            return this.ToProblem(result.Error!);
        }

        return Ok(result.Value);
    }
}

using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Contracts.Reports;
using HotelLakeview.Api.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace HotelLakeview.Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("occupancy")]
    public async Task<IActionResult> GetOccupancy(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var result = await _reportService.GetOccupancyAsync(startDate, endDate, cancellationToken);
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
        var result = await _reportService.GetMonthlyRevenueAsync(startDate, endDate, cancellationToken);
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
        var result = await _reportService.GetPopularRoomTypesAsync(startDate, endDate, cancellationToken);
        if (result.IsFailure)
        {
            return this.ToProblem(result.Error!);
        }

        return Ok(result.Value);
    }
}

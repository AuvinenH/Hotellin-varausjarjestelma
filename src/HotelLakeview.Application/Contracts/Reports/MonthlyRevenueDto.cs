namespace HotelLakeview.Application.Contracts.Reports;

public sealed record MonthlyRevenueDto(
    int Year,
    int Month,
    decimal Revenue);

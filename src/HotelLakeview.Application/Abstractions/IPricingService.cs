using HotelLakeview.Domain.ValueObjects;

namespace HotelLakeview.Application.Abstractions;

public interface IPricingService
{
    decimal CalculateTotal(decimal basePricePerNight, DateRange dateRange);

    bool IsPeakSeason(DateOnly nightDate);
}

using HotelLakeview.Application.Abstractions;
using HotelLakeview.Domain.ValueObjects;

namespace HotelLakeview.Application.Services;

public class SeasonalPricingService : IPricingService
{
    private const decimal PeakMultiplier = 1.30m;

    public decimal CalculateTotal(decimal basePricePerNight, DateRange dateRange)
    {
        if (basePricePerNight <= 0)
        {
            throw new ArgumentException("Base price must be greater than zero.");
        }

        decimal total = 0;

        foreach (var nightDate in dateRange.EnumerateNights())
        {
            var multiplier = IsPeakSeason(nightDate) ? PeakMultiplier : 1.00m;
            total += basePricePerNight * multiplier;
        }

        return decimal.Round(total, 2, MidpointRounding.AwayFromZero);
    }

    public bool IsPeakSeason(DateOnly nightDate)
    {
        var isSummer = nightDate.Month is >= 6 and <= 8;
        var isChristmas = (nightDate.Month == 12 && nightDate.Day >= 20)
            || (nightDate.Month == 1 && nightDate.Day <= 6);

        return isSummer || isChristmas;
    }
}

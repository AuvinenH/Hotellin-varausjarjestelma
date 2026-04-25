using HotelLakeview.Application.Services;
using HotelLakeview.Domain.ValueObjects;

namespace HotelLakeview.UnitTests.Application;

public class SeasonalPricingServiceTests
{
    private readonly SeasonalPricingService _service = new();

    [Fact]
    public void CalculateTotal_UsesBasePriceOutsideSeason()
    {
        var range = new DateRange(new DateOnly(2026, 2, 10), new DateOnly(2026, 2, 12));

        var total = _service.CalculateTotal(100m, range);

        Assert.Equal(200m, total);
    }

    [Fact]
    public void CalculateTotal_UsesPeakMultiplierInSummer()
    {
        var range = new DateRange(new DateOnly(2026, 6, 5), new DateOnly(2026, 6, 8));

        var total = _service.CalculateTotal(100m, range);

        Assert.Equal(390m, total);
    }

    [Fact]
    public void CalculateTotal_UsesPeakMultiplierAtChristmas()
    {
        var range = new DateRange(new DateOnly(2026, 12, 24), new DateOnly(2026, 12, 26));

        var total = _service.CalculateTotal(100m, range);

        Assert.Equal(260m, total);
    }

    [Fact]
    public void CalculateTotal_UsesPeakMultiplierAtNewYearStart()
    {
        var range = new DateRange(new DateOnly(2027, 1, 4), new DateOnly(2027, 1, 6));

        var total = _service.CalculateTotal(100m, range);

        Assert.Equal(260m, total);
    }

    [Fact]
    public void CalculateTotal_MixesPeakAndNormalNights()
    {
        var range = new DateRange(new DateOnly(2026, 8, 31), new DateOnly(2026, 9, 2));

        var total = _service.CalculateTotal(100m, range);

        Assert.Equal(230m, total);
    }

    [Fact]
    public void IsPeakSeason_ReturnsTrueForBoundaryDates()
    {
        Assert.True(_service.IsPeakSeason(new DateOnly(2026, 6, 1)));
        Assert.True(_service.IsPeakSeason(new DateOnly(2026, 8, 31)));
        Assert.True(_service.IsPeakSeason(new DateOnly(2026, 12, 20)));
        Assert.True(_service.IsPeakSeason(new DateOnly(2027, 1, 6)));
    }

    [Fact]
    public void IsPeakSeason_ReturnsFalseOutsideDefinedPeriods()
    {
        Assert.False(_service.IsPeakSeason(new DateOnly(2026, 5, 31)));
        Assert.False(_service.IsPeakSeason(new DateOnly(2026, 9, 1)));
        Assert.False(_service.IsPeakSeason(new DateOnly(2026, 12, 19)));
        Assert.False(_service.IsPeakSeason(new DateOnly(2027, 1, 7)));
    }
}

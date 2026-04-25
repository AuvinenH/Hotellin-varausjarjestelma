using HotelLakeview.Application.Services;

namespace HotelLakeview.UnitTests.Application;

public class OccupancyCalculatorTests
{
    [Fact]
    public void CalculateOccupancyRatePercent_ReturnsZero_WhenNoRoomNights()
    {
        var result = OccupancyCalculator.CalculateOccupancyRatePercent(5, 0);

        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateOccupancyRatePercent_CalculatesRoundedPercent()
    {
        var result = OccupancyCalculator.CalculateOccupancyRatePercent(5, 12);

        Assert.Equal(41.67m, result);
    }

    [Fact]
    public void CalculateOccupancyRatePercent_ThrowsForNegativeValues()
    {
        Assert.Throws<ArgumentException>(() => OccupancyCalculator.CalculateOccupancyRatePercent(-1, 10));
        Assert.Throws<ArgumentException>(() => OccupancyCalculator.CalculateOccupancyRatePercent(1, -10));
    }
}

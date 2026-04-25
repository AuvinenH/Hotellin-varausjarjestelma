using HotelLakeview.Domain.ValueObjects;

namespace HotelLakeview.UnitTests.Domain;

public class DateRangeTests
{
    [Fact]
    public void Constructor_Throws_WhenCheckOutIsNotAfterCheckIn()
    {
        Assert.Throws<ArgumentException>(() => new DateRange(new DateOnly(2026, 3, 10), new DateOnly(2026, 3, 10)));
        Assert.Throws<ArgumentException>(() => new DateRange(new DateOnly(2026, 3, 11), new DateOnly(2026, 3, 10)));
    }

    [Fact]
    public void Nights_ReturnsCorrectCount()
    {
        var range = new DateRange(new DateOnly(2026, 4, 1), new DateOnly(2026, 4, 6));

        Assert.Equal(5, range.Nights);
    }

    [Fact]
    public void EnumerateNights_ReturnsExpectedDates()
    {
        var range = new DateRange(new DateOnly(2026, 4, 1), new DateOnly(2026, 4, 4));

        var nights = range.EnumerateNights().ToList();

        Assert.Equal(3, nights.Count);
        Assert.Equal(new DateOnly(2026, 4, 1), nights[0]);
        Assert.Equal(new DateOnly(2026, 4, 2), nights[1]);
        Assert.Equal(new DateOnly(2026, 4, 3), nights[2]);
    }

    [Fact]
    public void Overlaps_ReturnsTrue_WhenRangesOverlap()
    {
        var first = new DateRange(new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 5));
        var second = new DateRange(new DateOnly(2026, 5, 4), new DateOnly(2026, 5, 8));

        Assert.True(first.Overlaps(second));
    }

    [Fact]
    public void Overlaps_ReturnsFalse_WhenRangesAreAdjacent()
    {
        var first = new DateRange(new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 5));
        var second = new DateRange(new DateOnly(2026, 5, 5), new DateOnly(2026, 5, 8));

        Assert.False(first.Overlaps(second));
    }
}

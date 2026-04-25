namespace HotelLakeview.Application.Services;

public static class OccupancyCalculator
{
    public static decimal CalculateOccupancyRatePercent(int reservedNights, int totalRoomNights)
    {
        if (reservedNights < 0)
        {
            throw new ArgumentException("Reserved nights cannot be negative.");
        }

        if (totalRoomNights < 0)
        {
            throw new ArgumentException("Total room nights cannot be negative.");
        }

        if (totalRoomNights == 0)
        {
            return 0;
        }

        return decimal.Round((decimal)reservedNights / totalRoomNights * 100m, 2, MidpointRounding.AwayFromZero);
    }
}

using HotelLakeview.Domain.Enums;

namespace HotelLakeview.Domain.Entities;

public class Room
{
    public Room(Guid id, string number, RoomCategory category, int maxGuests, decimal basePricePerNight, string? description)
    {
        Id = id;
        UpdateDetails(number, category, maxGuests, basePricePerNight, description);
    }

    private Room()
    {
    }

    public Guid Id { get; private set; }

    public string Number { get; private set; } = string.Empty;

    public RoomCategory Category { get; private set; }

    public int MaxGuests { get; private set; }

    public decimal BasePricePerNight { get; private set; }

    public string? Description { get; private set; }

    public void UpdateDetails(string number, RoomCategory category, int maxGuests, decimal basePricePerNight, string? description)
    {
        Number = string.IsNullOrWhiteSpace(number)
            ? throw new ArgumentException("Room number is required.")
            : number.Trim().ToUpperInvariant();

        if (maxGuests < 1)
        {
            throw new ArgumentException("Max guests must be greater than zero.");
        }

        if (basePricePerNight <= 0)
        {
            throw new ArgumentException("Base price must be greater than zero.");
        }

        Category = category;
        MaxGuests = maxGuests;
        BasePricePerNight = decimal.Round(basePricePerNight, 2, MidpointRounding.AwayFromZero);
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}

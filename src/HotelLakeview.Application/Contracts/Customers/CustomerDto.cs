namespace HotelLakeview.Application.Contracts.Customers;

public sealed record CustomerDto(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string? Notes,
    DateTime CreatedAtUtc);

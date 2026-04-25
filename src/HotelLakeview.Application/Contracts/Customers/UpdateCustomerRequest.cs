namespace HotelLakeview.Application.Contracts.Customers;

public sealed record UpdateCustomerRequest(
    string FullName,
    string Email,
    string PhoneNumber,
    string? Notes);

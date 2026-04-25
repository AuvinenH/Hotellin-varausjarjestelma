using HotelLakeview.Application.Contracts.Customers;

namespace HotelLakeview.Application.Abstractions;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerDto>> GetAllAsync(string? search, CancellationToken cancellationToken);

    Task<CustomerDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken);

    Task<CustomerDto> UpdateAsync(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}

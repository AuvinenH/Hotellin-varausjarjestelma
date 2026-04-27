using HotelLakeview.Application.Contracts.Customers;
using HotelLakeview.Application.Common;

namespace HotelLakeview.Application.Abstractions;

public interface ICustomerService
{
    Task<Result<IReadOnlyList<CustomerDto>>> GetAllAsync(string? search, CancellationToken cancellationToken);

    Task<Result<CustomerDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<CustomerDto>> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken);

    Task<Result<CustomerDto>> UpdateAsync(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken);

    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}

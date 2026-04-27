using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Common;
using HotelLakeview.Application.Contracts.Customers;
using HotelLakeview.Domain.Entities;

namespace HotelLakeview.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<IReadOnlyList<CustomerDto>>> GetAllAsync(string? search, CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.GetAllAsync(search, cancellationToken);
        return Result<IReadOnlyList<CustomerDto>>.Success(customers.Select(Map).ToList());
    }

    public async Task<Result<CustomerDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);

        if (customer is null)
        {
            return Result<CustomerDto>.Failure(ResultError.NotFound("customer.not_found", $"Customer '{id}' was not found."));
        }

        return Result<CustomerDto>.Success(Map(customer));
    }

    public async Task<Result<CustomerDto>> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var existing = await _customerRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (existing is not null)
        {
            return Result<CustomerDto>.Failure(ResultError.Conflict("customer.email_conflict", "Customer with the same email already exists."));
        }

        Customer customer;
        try
        {
            customer = new Customer(Guid.NewGuid(), request.FullName, request.Email, request.PhoneNumber, request.Notes);
        }
        catch (ArgumentException exception)
        {
            return Result<CustomerDto>.Failure(ResultError.Validation("customer.invalid", exception.Message));
        }

        await _customerRepository.AddAsync(customer, cancellationToken);
        await _customerRepository.SaveChangesAsync(cancellationToken);

        return Result<CustomerDto>.Success(Map(customer));
    }

    public async Task<Result<CustomerDto>> UpdateAsync(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);

        if (customer is null)
        {
            return Result<CustomerDto>.Failure(ResultError.NotFound("customer.not_found", $"Customer '{id}' was not found."));
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var existing = await _customerRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (existing is not null && existing.Id != id)
        {
            return Result<CustomerDto>.Failure(ResultError.Conflict("customer.email_conflict", "Customer with the same email already exists."));
        }

        try
        {
            customer.UpdateDetails(request.FullName, request.Email, request.PhoneNumber, request.Notes);
        }
        catch (ArgumentException exception)
        {
            return Result<CustomerDto>.Failure(ResultError.Validation("customer.invalid", exception.Message));
        }

        await _customerRepository.SaveChangesAsync(cancellationToken);
        return Result<CustomerDto>.Success(Map(customer));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);

        if (customer is null)
        {
            return Result.Failure(ResultError.NotFound("customer.not_found", $"Customer '{id}' was not found."));
        }

        _customerRepository.Remove(customer);
        await _customerRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static CustomerDto Map(Customer customer)
    {
        return new CustomerDto(
            customer.Id,
            customer.FullName,
            customer.Email,
            customer.PhoneNumber,
            customer.Notes,
            customer.CreatedAtUtc);
    }
}

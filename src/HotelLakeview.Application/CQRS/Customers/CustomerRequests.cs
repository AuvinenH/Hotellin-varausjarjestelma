using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Common;
using HotelLakeview.Application.Contracts.Customers;
using MediatR;

namespace HotelLakeview.Application.CQRS.Customers;

public sealed record GetCustomersQuery(string? Search) : IRequest<Result<IReadOnlyList<CustomerDto>>>;

public sealed class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, Result<IReadOnlyList<CustomerDto>>>
{
    private readonly ICustomerService _customerService;

    public GetCustomersQueryHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public Task<Result<IReadOnlyList<CustomerDto>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        => _customerService.GetAllAsync(request.Search, cancellationToken);
}

public sealed record GetCustomerByIdQuery(Guid Id) : IRequest<Result<CustomerDto>>;

public sealed class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
{
    private readonly ICustomerService _customerService;

    public GetCustomerByIdQueryHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public Task<Result<CustomerDto>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        => _customerService.GetByIdAsync(request.Id, cancellationToken);
}

public sealed record CreateCustomerCommand(CreateCustomerRequest Request) : IRequest<Result<CustomerDto>>;

public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
{
    private readonly ICustomerService _customerService;

    public CreateCustomerCommandHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public Task<Result<CustomerDto>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        => _customerService.CreateAsync(request.Request, cancellationToken);
}

public sealed record UpdateCustomerCommand(Guid Id, UpdateCustomerRequest Request) : IRequest<Result<CustomerDto>>;

public sealed class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result<CustomerDto>>
{
    private readonly ICustomerService _customerService;

    public UpdateCustomerCommandHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public Task<Result<CustomerDto>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        => _customerService.UpdateAsync(request.Id, request.Request, cancellationToken);
}

public sealed record DeleteCustomerCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result>
{
    private readonly ICustomerService _customerService;

    public DeleteCustomerCommandHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public Task<Result> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        => _customerService.DeleteAsync(request.Id, cancellationToken);
}

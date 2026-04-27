using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Common;
using HotelLakeview.Application.Contracts.Reservations;
using MediatR;

namespace HotelLakeview.Application.CQRS.Reservations;

public sealed record GetReservationsQuery() : IRequest<Result<IReadOnlyList<ReservationDto>>>;

public sealed class GetReservationsQueryHandler : IRequestHandler<GetReservationsQuery, Result<IReadOnlyList<ReservationDto>>>
{
    private readonly IReservationService _reservationService;

    public GetReservationsQueryHandler(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    public Task<Result<IReadOnlyList<ReservationDto>>> Handle(GetReservationsQuery request, CancellationToken cancellationToken)
        => _reservationService.GetAllAsync(cancellationToken);
}

public sealed record GetReservationByIdQuery(Guid Id) : IRequest<Result<ReservationDto>>;

public sealed class GetReservationByIdQueryHandler : IRequestHandler<GetReservationByIdQuery, Result<ReservationDto>>
{
    private readonly IReservationService _reservationService;

    public GetReservationByIdQueryHandler(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    public Task<Result<ReservationDto>> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
        => _reservationService.GetByIdAsync(request.Id, cancellationToken);
}

public sealed record CreateReservationCommand(CreateReservationRequest Request) : IRequest<Result<ReservationDto>>;

public sealed class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, Result<ReservationDto>>
{
    private readonly IReservationService _reservationService;

    public CreateReservationCommandHandler(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    public Task<Result<ReservationDto>> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
        => _reservationService.CreateAsync(request.Request, cancellationToken);
}

public sealed record UpdateReservationCommand(Guid Id, UpdateReservationRequest Request) : IRequest<Result<ReservationDto>>;

public sealed class UpdateReservationCommandHandler : IRequestHandler<UpdateReservationCommand, Result<ReservationDto>>
{
    private readonly IReservationService _reservationService;

    public UpdateReservationCommandHandler(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    public Task<Result<ReservationDto>> Handle(UpdateReservationCommand request, CancellationToken cancellationToken)
        => _reservationService.UpdateAsync(request.Id, request.Request, cancellationToken);
}

public sealed record CancelReservationCommand(Guid Id) : IRequest<Result>;

public sealed class CancelReservationCommandHandler : IRequestHandler<CancelReservationCommand, Result>
{
    private readonly IReservationService _reservationService;

    public CancelReservationCommandHandler(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    public Task<Result> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
        => _reservationService.CancelAsync(request.Id, cancellationToken);
}

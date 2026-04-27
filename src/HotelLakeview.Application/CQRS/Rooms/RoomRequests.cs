using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Common;
using HotelLakeview.Application.Contracts.Rooms;
using HotelLakeview.Domain.Enums;
using MediatR;

namespace HotelLakeview.Application.CQRS.Rooms;

public sealed record GetRoomsQuery() : IRequest<Result<IReadOnlyList<RoomDto>>>;

public sealed class GetRoomsQueryHandler : IRequestHandler<GetRoomsQuery, Result<IReadOnlyList<RoomDto>>>
{
    private readonly IRoomService _roomService;

    public GetRoomsQueryHandler(IRoomService roomService)
    {
        _roomService = roomService;
    }

    public Task<Result<IReadOnlyList<RoomDto>>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
        => _roomService.GetAllAsync(cancellationToken);
}

public sealed record GetRoomByIdQuery(Guid Id) : IRequest<Result<RoomDto>>;

public sealed class GetRoomByIdQueryHandler : IRequestHandler<GetRoomByIdQuery, Result<RoomDto>>
{
    private readonly IRoomService _roomService;

    public GetRoomByIdQueryHandler(IRoomService roomService)
    {
        _roomService = roomService;
    }

    public Task<Result<RoomDto>> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
        => _roomService.GetByIdAsync(request.Id, cancellationToken);
}

public sealed record GetAvailableRoomsQuery(DateOnly CheckInDate, DateOnly CheckOutDate, int GuestCount, RoomCategory? Category) : IRequest<Result<IReadOnlyList<RoomDto>>>;

public sealed class GetAvailableRoomsQueryHandler : IRequestHandler<GetAvailableRoomsQuery, Result<IReadOnlyList<RoomDto>>>
{
    private readonly IRoomService _roomService;

    public GetAvailableRoomsQueryHandler(IRoomService roomService)
    {
        _roomService = roomService;
    }

    public Task<Result<IReadOnlyList<RoomDto>>> Handle(GetAvailableRoomsQuery request, CancellationToken cancellationToken)
        => _roomService.GetAvailableAsync(request.CheckInDate, request.CheckOutDate, request.GuestCount, request.Category, cancellationToken);
}

public sealed record CreateRoomCommand(CreateRoomRequest Request) : IRequest<Result<RoomDto>>;

public sealed class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand, Result<RoomDto>>
{
    private readonly IRoomService _roomService;

    public CreateRoomCommandHandler(IRoomService roomService)
    {
        _roomService = roomService;
    }

    public Task<Result<RoomDto>> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
        => _roomService.CreateAsync(request.Request, cancellationToken);
}

public sealed record UpdateRoomCommand(Guid Id, UpdateRoomRequest Request) : IRequest<Result<RoomDto>>;

public sealed class UpdateRoomCommandHandler : IRequestHandler<UpdateRoomCommand, Result<RoomDto>>
{
    private readonly IRoomService _roomService;

    public UpdateRoomCommandHandler(IRoomService roomService)
    {
        _roomService = roomService;
    }

    public Task<Result<RoomDto>> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
        => _roomService.UpdateAsync(request.Id, request.Request, cancellationToken);
}

public sealed record DeleteRoomCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteRoomCommandHandler : IRequestHandler<DeleteRoomCommand, Result>
{
    private readonly IRoomService _roomService;

    public DeleteRoomCommandHandler(IRoomService roomService)
    {
        _roomService = roomService;
    }

    public Task<Result> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
        => _roomService.DeleteAsync(request.Id, cancellationToken);
}

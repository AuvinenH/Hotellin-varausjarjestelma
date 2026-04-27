using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HotelLakeview.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(typeof(DependencyInjection).Assembly);

        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IRoomImageService, RoomImageService>();
        services.AddScoped<IPricingService, SeasonalPricingService>();

        return services;
    }
}

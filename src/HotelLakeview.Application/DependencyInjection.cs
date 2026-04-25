using HotelLakeview.Application.Abstractions;
using HotelLakeview.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelLakeview.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IRoomImageService, RoomImageService>();
        services.AddScoped<IPricingService, SeasonalPricingService>();

        return services;
    }
}

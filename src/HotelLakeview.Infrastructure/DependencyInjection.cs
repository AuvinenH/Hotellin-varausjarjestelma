using HotelLakeview.Application.Abstractions;
using HotelLakeview.Infrastructure.Persistence;
using HotelLakeview.Infrastructure.Repositories;
using HotelLakeview.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelLakeview.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=hotel-lakeview.db";

        services.AddDbContext<HotelDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IRoomImageRepository, RoomImageRepository>();
        services.AddScoped<IRoomImageStorage, LocalRoomImageStorage>();

        return services;
    }

    public static async Task EnsureDatabaseCreatedAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }
}

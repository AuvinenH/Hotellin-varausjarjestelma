using HotelLakeview.Application.Abstractions;
using HotelLakeview.Infrastructure.Persistence;
using HotelLakeview.Infrastructure.Repositories;
using HotelLakeview.Infrastructure.Storage;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace HotelLakeview.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var rawConnectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=hotel-lakeview.db";
        var connectionString = ExpandSqliteConnectionString(rawConnectionString);

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

    private static string ExpandSqliteConnectionString(string connectionString)
    {
        var expanded = Environment.ExpandEnvironmentVariables(connectionString);
        var builder = new SqliteConnectionStringBuilder(expanded);

        if (!string.IsNullOrWhiteSpace(builder.DataSource)
            && !builder.DataSource.Equals(":memory:", StringComparison.OrdinalIgnoreCase))
        {
            var fullPath = Path.GetFullPath(builder.DataSource);
            var directory = Path.GetDirectoryName(fullPath);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            builder.DataSource = fullPath;
        }

        return builder.ToString();
    }
}

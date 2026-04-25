using HotelLakeview.Domain.Entities;
using HotelLakeview.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelLakeview.Infrastructure.Persistence;

public class HotelDbContext : DbContext
{
    public HotelDbContext(DbContextOptions<HotelDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Room> Rooms => Set<Room>();

    public DbSet<Reservation> Reservations => Set<Reservation>();

    public DbSet<ReservationNight> ReservationNights => Set<ReservationNight>();

    public DbSet<RoomImage> RoomImages => Set<RoomImage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FullName).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(150).IsRequired();
            entity.Property(x => x.PhoneNumber).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Notes).HasMaxLength(2000);
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Number).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(1000);
            entity.Property(x => x.Category)
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();
            entity.Property(x => x.BasePricePerNight).HasColumnType("decimal(18,2)");
            entity.HasIndex(x => x.Number).IsUnique();
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();
            entity.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)");
            entity.HasOne<Room>()
                .WithMany()
                .HasForeignKey(x => x.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Customer>()
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ReservationNight>(entity =>
        {
            entity.HasKey(x => new { x.ReservationId, x.NightDate });
            entity.HasOne(x => x.Reservation)
                .WithMany()
                .HasForeignKey(x => x.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => new { x.RoomId, x.NightDate }).IsUnique();
        });

        modelBuilder.Entity<RoomImage>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FileName).HasMaxLength(260).IsRequired();
            entity.Property(x => x.StoredFileName).HasMaxLength(300).IsRequired();
            entity.Property(x => x.ContentType).HasMaxLength(120).IsRequired();
            entity.HasOne<Room>()
                .WithMany()
                .HasForeignKey(x => x.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        SeedRooms(modelBuilder);
    }

    private static void SeedRooms(ModelBuilder modelBuilder)
    {
        var rooms = new List<Room>();

        void AddRooms(int count, int startNumber, RoomCategory category, int maxGuests, decimal price)
        {
            for (var i = 0; i < count; i++)
            {
                rooms.Add(new Room(
                    Guid.Parse($"{startNumber + i:00000000}-0000-0000-0000-000000000001"),
                    (startNumber + i).ToString(),
                    category,
                    maxGuests,
                    price,
                    $"{category} room"));
            }
        }

        AddRooms(8, 101, RoomCategory.Economy, 1, 79m);
        AddRooms(10, 201, RoomCategory.Standard, 2, 119m);
        AddRooms(6, 301, RoomCategory.Superior, 2, 159m);
        AddRooms(4, 401, RoomCategory.JuniorSuite, 3, 219m);
        AddRooms(2, 501, RoomCategory.Suite, 4, 319m);

        modelBuilder.Entity<Room>().HasData(rooms);
    }
}

using Microsoft.EntityFrameworkCore;
using HotelBookingApi.Models;

namespace HotelBookingApi.Data;

public class HotelDbContext : DbContext
{
    public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
    {
    }

    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<RoomType> RoomTypes => Set<RoomType>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Hotel configuration
        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(h => h.Id);
            entity.Property(h => h.Name).IsRequired().HasMaxLength(200);
            entity.Property(h => h.Address).IsRequired().HasMaxLength(500);
            entity.HasIndex(h => h.Name);
        });

        // RoomType configuration
        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.HasKey(rt => rt.Id);
            entity.Property(rt => rt.Name).IsRequired().HasMaxLength(50);
            entity.Property(rt => rt.Capacity).IsRequired();

            // Add room types
            entity.HasData(
                new RoomType { Id = 1, Name = "Single", Capacity = 1 },
                new RoomType { Id = 2, Name = "Double", Capacity = 2 },
                new RoomType { Id = 3, Name = "Deluxe", Capacity = 2 }
            );
        });

        // Room configuration
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.RoomNumber).IsRequired().HasMaxLength(20);
            
            entity.HasOne(r => r.Hotel)
                .WithMany(h => h.Rooms)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.RoomType)
                .WithMany(rt => rt.Rooms)
                .HasForeignKey(r => r.RoomTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(r => new { r.HotelId, r.RoomNumber }).IsUnique();
        });

        // Booking configuration
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.BookingReference).IsRequired().HasMaxLength(50);
            entity.Property(b => b.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(b => b.LastName).IsRequired().HasMaxLength(100);
            entity.Property(b => b.Address).IsRequired().HasMaxLength(500);
            entity.Property(b => b.ContactNumber).IsRequired().HasMaxLength(50);
            entity.Property(b => b.GuestCount).IsRequired();
            entity.Property(b => b.CheckInDate).IsRequired();
            entity.Property(b => b.CheckOutDate).IsRequired();
            entity.Property(b => b.CreatedAt).IsRequired();

            entity.HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(b => b.BookingReference).IsUnique();
        });
    }
}

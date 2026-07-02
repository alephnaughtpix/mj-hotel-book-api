using Bogus;
using Microsoft.EntityFrameworkCore;
using HotelBookingApi.Models;

namespace HotelBookingApi.Data;

public class DataSeeder
{
    private readonly HotelDbContext _context;

    public DataSeeder(HotelDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Seeds the database with test data. 
    /// 
    /// This uses the Bogus library by Brian Chavez. ( https://github.com/bchavez/Bogus )
    /// </summary>
    /// <returns>True if data was seeded, or False if data already exists and seeding was skipped.</returns>
    public async Task<bool> SeedAsync()
    {
        // Only seed if no hotels exist 
        if (await _context.Hotels.AnyAsync())
        {
            return false; // Data already exists, seeding skipped
        }

        // Get room types from database
        var roomTypes = await _context.RoomTypes.ToListAsync();
        if (!roomTypes.Any())
        {
            throw new InvalidOperationException("Room types not found. Ensure migrations have been applied.");
        }

        // Create fake hotels
        var hotelFaker = new Faker<Hotel>()
            .RuleFor(h => h.Name, f => f.Company.CompanyName() + " Hotel")
            .RuleFor(h => h.Address, f => f.Address.FullAddress());

        var hotels = hotelFaker.Generate(10);
        await _context.Hotels.AddRangeAsync(hotels);
        await _context.SaveChangesAsync();

        // Create 6 rooms per hotel
        var rooms = new List<Room>();
        foreach (var hotel in hotels)
        {
            // There are 3 room types, so create two of each
            var roomNumber = 101;
            foreach (var roomType in roomTypes)
            {
                for (int i = 0; i < 2; i++)
                {
                    rooms.Add(new Room
                    {
                        HotelId = hotel.Id,
                        RoomTypeId = roomType.Id,
                        RoomNumber = roomNumber.ToString()
                    });
                    roomNumber++;
                }
            }
        }

        await _context.Rooms.AddRangeAsync(rooms);
        await _context.SaveChangesAsync();

        // Create some sample bookings
        var bookingFaker = new Faker<Booking>()
            .RuleFor(b => b.FirstName, f => f.Name.FirstName())
            .RuleFor(b => b.LastName, f => f.Name.LastName())
            .RuleFor(b => b.Address, f => f.Address.FullAddress())
            .RuleFor(b => b.ContactNumber, f => f.Phone.PhoneNumber())
            .RuleFor(b => b.CreatedAt, f => f.Date.Past(1));

        var random = new Random(42);
        var sampleBookings = new List<Booking>();
        
        // Reload rooms complete with the room type
        var savedRooms = await _context.Rooms.Include(r => r.RoomType).ToListAsync();
        
        // Create 2 random bookings in each hotel
        foreach (var hotel in hotels)
        {
            // Get the rooms for the current hotel
            var hotelRooms = savedRooms.Where(r => r.HotelId == hotel.Id).ToList();
            
            for (int i = 0; i < 2; i++)
            {
                var room = hotelRooms[random.Next(hotelRooms.Count)];   // Random room
                var checkIn = DateOnly.FromDateTime(DateTime.Today.AddDays(random.Next(1, 30)));    // Date within next month
                
                var booking = bookingFaker.Generate();
                booking.BookingReference = $"BK-SEED-{Guid.NewGuid().ToString()[..8].ToUpper()}";   // Booking reference
                booking.RoomId = room.Id;
                booking.GuestCount = random.Next(1, room.RoomType.Capacity + 1);
                booking.CheckInDate = checkIn;
                booking.CheckOutDate = checkIn.AddDays(random.Next(1, 5)); // Stay between 1-5 days
                
                sampleBookings.Add(booking);
            }
        }

        await _context.Bookings.AddRangeAsync(sampleBookings);
        await _context.SaveChangesAsync();

        return true; // Data seeded successfully
    }

    public async Task ResetAsync()
    {
        // Delete all data except room types 
        _context.Bookings.RemoveRange(_context.Bookings);
        _context.Rooms.RemoveRange(_context.Rooms);
        _context.Hotels.RemoveRange(_context.Hotels);
        
        await _context.SaveChangesAsync();
    }
}

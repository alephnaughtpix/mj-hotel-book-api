using Microsoft.EntityFrameworkCore;
using HotelBookingApi.Data;
using HotelBookingApi.DataTransfer;
using HotelBookingApi.Models;

namespace HotelBookingApi.Services;

public class BookingService : IBookingService
{
    private readonly HotelDbContext _context;
    private readonly IHotelService _hotelService;

    public BookingService(HotelDbContext context, IHotelService hotelService)
    {
        _context = context;
        _hotelService = hotelService;
    }

    /// <summary>
    /// Get booking details by booking reference.
    /// </summary>
    /// <param name="bookingReference">String contaaining the booking reference</param>
    /// <returns>BookingResponseDataTransfer object containing booking details, or null if not found</returns>
    public async Task<BookingResponseDataTransfer?> GetBookingByReferenceAsync(string bookingReference)
    {
        return await _context.Bookings
            .Include(b => b.Room)
                .ThenInclude(r => r.RoomType)
            .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
            .Where(b => b.BookingReference == bookingReference)
            .Select(b => new BookingResponseDataTransfer
            {
                Id = b.Id,
                BookingReference = b.BookingReference,
                FirstName = b.FirstName,
                LastName = b.LastName,
                Address = b.Address,
                ContactNumber = b.ContactNumber,
                GuestCount = b.GuestCount,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                CreatedAt = b.CreatedAt,
                HotelName = b.Room.Hotel.Name,
                HotelAddress = b.Room.Hotel.Address,
                Room = new RoomDataTransfer
                {
                    Id = b.Room.Id,
                    RoomNumber = b.Room.RoomNumber,
                    RoomTypeName = b.Room.RoomType.Name,
                    Capacity = b.Room.RoomType.Capacity
                }
            })
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Create a new booking, given the booking request data transfer object. 
    /// </summary>
    /// <param name="bookingRequest">Booking request data transfer object containing booking details</param>
    /// <returns></returns>
    public async Task<(BookingResponseDataTransfer? Booking, string? Error)> CreateBookingAsync(CreateBookingDataTransfer bookingRequest)
    {
        // Validate dates
        if (bookingRequest.CheckOutDate <= bookingRequest.CheckInDate)
            return (null, "Check-out date must be after check-in date.");
        if (bookingRequest.CheckInDate < DateOnly.FromDateTime(DateTime.Today))
            return (null, "Check-in date cannot be in the past.");

        // Validate room type if provided
        if (!string.IsNullOrWhiteSpace(bookingRequest.RoomType))
        {
            // Look up room type from database to validate and get capacity
            var roomType = await _hotelService.GetRoomTypeByNameAsync(bookingRequest.RoomType);

            if (roomType == null)
            {
                var validTypes = await _hotelService.GetValidRoomTypeNamesAsync();
                return (null, $"Invalid room type '{bookingRequest.RoomType}'. Valid types are: {string.Join(", ", validTypes)}.");
            }

            // Validate guest count against room type capacity
            if (bookingRequest.GuestCount > roomType.Capacity)
                return (null, $"A {roomType.Name} room has a capacity of {roomType.Capacity} and cannot accommodate {bookingRequest.GuestCount} guests.");
        }

        // Find available rooms that match the criteria (including room type if specified)
        var availableRooms = await _hotelService.GetAvailableRoomsAsync(
            bookingRequest.HotelId,
            bookingRequest.CheckInDate,
            bookingRequest.CheckOutDate,
            bookingRequest.GuestCount,
            bookingRequest.RoomType);

        var availableRoom = availableRooms.FirstOrDefault();

        if (availableRoom == null)
        {
            var message = string.IsNullOrWhiteSpace(bookingRequest.RoomType)
                ? "No rooms available for the specified dates and guest count. Please try different dates or a different hotel."
                : $"No '{bookingRequest.RoomType}' rooms available for the specified dates. Please try a different room type or different dates.";
            return (null, message);
        }

        // Get the full room data for the booking
        var room = await _context.Rooms
            .Include(r => r.RoomType)
            .Include(r => r.Hotel)
            .FirstOrDefaultAsync(r => r.Id == availableRoom.RoomId);

        if (room == null)
            return (null, "Room not found.");

        // Generate unique booking reference
        var bookingReference = await GenerateUniqueBookingReferenceAsync();

        // Create and save new booking
        var booking = new Booking
        {
            BookingReference = bookingReference,
            RoomId = room.Id,
            FirstName = bookingRequest.FirstName,
            LastName = bookingRequest.LastName,
            Address = bookingRequest.Address,
            ContactNumber = bookingRequest.ContactNumber,
            GuestCount = bookingRequest.GuestCount,
            CheckInDate = bookingRequest.CheckInDate,
            CheckOutDate = bookingRequest.CheckOutDate,
            CreatedAt = DateTime.UtcNow
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Return the response, confirming the details
        var response = new BookingResponseDataTransfer
        {
            Id = booking.Id,
            BookingReference = booking.BookingReference,
            FirstName = booking.FirstName,
            LastName = booking.LastName,
            Address = booking.Address,
            ContactNumber = booking.ContactNumber,
            GuestCount = booking.GuestCount,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            CreatedAt = booking.CreatedAt,
            HotelName = room.Hotel.Name,
            HotelAddress = room.Hotel.Address,
            Room = new RoomDataTransfer
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                RoomTypeName = room.RoomType.Name,
                Capacity = room.RoomType.Capacity
            }
        };

        return (response, null);
    }

    /// <summary>
    /// Generate unique booking reference
    /// </summary>
    /// <returns>Booking reference as string</returns>
    private async Task<string> GenerateUniqueBookingReferenceAsync()
    {
        string reference;
        bool exists;

        do
        {
            // Generate a reference, which includes a date and a GUID (eg BK-20250701-ABC123GW )
            reference = $"BK-{DateTime.UtcNow.ToString("yyyyMMdd")}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
            // Check if the reference just happens to clash with any existing bookings
            exists = await _context.Bookings.AnyAsync(b => b.BookingReference == reference);
        }
        while (exists);
        // If the reference has not been used, return it.
        return reference;
    }
}

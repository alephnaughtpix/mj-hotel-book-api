using Microsoft.EntityFrameworkCore;
using HotelBookingApi.Data;
using HotelBookingApi.DataTransfer;

namespace HotelBookingApi.Services;

public class HotelService : IHotelService
{
    private readonly HotelDbContext _context;

    public HotelService(HotelDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Search for a hotel by name. This does a case-insensitive search within hotel names, so works 
    /// as a partial search (eg. "hotel" will list anything with "hotel" in it's name.) 
    /// </summary>
    /// <param name="name">Name or partial name of hotel</param>
    /// <returns>A list of one or more hotels</returns>
    public async Task<IEnumerable<HotelDataTransfer>> SearchHotelsByNameAsync(string name)
    {
        return await _context.Hotels
            .Where(h => h.Name.ToLower().Contains(name.ToLower()))
            .Select(h => new HotelDataTransfer
            {
                Id = h.Id,
                Name = h.Name,
                Address = h.Address,
                TotalRooms = h.Rooms.Count
            })
            .ToListAsync();
    }

    public async Task<HotelDetailDataTransfer?> GetHotelByIdAsync(int id)
    {
        return await _context.Hotels
            .Where(h => h.Id == id)
            .Select(h => new HotelDetailDataTransfer
            {
                Id = h.Id,
                Name = h.Name,
                Address = h.Address,
                Rooms = h.Rooms.Select(r => new RoomDataTransfer
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    RoomTypeName = r.RoomType.Name,
                    Capacity = r.RoomType.Capacity
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Find available rooms in a hotel, given guest requirements
    /// </summary>
    /// <param name="hotelId">ID of hotel</param>
    /// <param name="checkIn">Date of check-in</param>
    /// <param name="checkOut">Date of check-out</param>
    /// <param name="guestCount">How many guests</param>
    /// <param name="roomType">Optional room type filter (Single, Double, Deluxe)</param>
    /// <returns>List of available rooms</returns>
    public async Task<IEnumerable<AvailableRoomDataTransfer>> GetAvailableRoomsAsync(
        int hotelId, 
        DateOnly checkIn, 
        DateOnly checkOut, 
        int guestCount,
        string? roomType = null)
    {
        var query = _context.Rooms
            .Include(r => r.RoomType)
            .Include(r => r.Hotel)
            .Where(r => r.HotelId == hotelId)                           // Only this hotel's rooms
            .Where(r => r.RoomType.Capacity >= guestCount)              // Capacity is big enough
            .Where(r => !r.Bookings.Any(b => 
                b.CheckInDate < checkOut && b.CheckOutDate > checkIn)); // Check it's not double-booked

        // Apply room type filter if specified
        if (!string.IsNullOrWhiteSpace(roomType))
        {
            query = query.Where(r => r.RoomType.Name.ToLower() == roomType.ToLower());
        }

        var availableRooms = await query
            .Select(r => new AvailableRoomDataTransfer
            {
                RoomId = r.Id,
                RoomNumber = r.RoomNumber,
                RoomTypeName = r.RoomType.Name,
                Capacity = r.RoomType.Capacity,
                HotelId = r.HotelId,
                HotelName = r.Hotel.Name
            })
            .ToListAsync();

        return availableRooms;
    }
}

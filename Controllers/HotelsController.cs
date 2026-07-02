using Microsoft.AspNetCore.Mvc;
using HotelBookingApi.DataTransfer;
using HotelBookingApi.Services;

namespace HotelBookingApi.Controllers;

[Route("api/[controller]")]
public class HotelsController : ApiControllerBase
{
    private readonly IHotelService _hotelService;
    private readonly ILogger<HotelsController> _logger;

    public HotelsController(IHotelService hotelService, ILogger<HotelsController> logger)
    {
        _hotelService = hotelService;
        _logger = logger;
    }

    /// <summary>
    /// Search for hotels by name
    /// </summary>
    /// <param name="name">The hotel name to search for (partial match supported)</param>
    /// <returns>List of hotels matching the search criteria</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<HotelDataTransfer>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<HotelDataTransfer>>> SearchHotels([FromQuery] string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest(new { message = "Hotel name is required for searching." });

        try
        {
            var hotels = await _hotelService.SearchHotelsByNameAsync(name);

            if (hotels.Any())
                return Ok(hotels);

            return NotFound($"No Hotels were found that contained '{name}'.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching hotels with name: {Name}", name);
            return InternalServerError(ex);
        }
    }

    /// <summary>
    /// Get hotel details by ID
    /// </summary>
    /// <param name="id">The hotels ID</param>
    /// <returns>Hotel details including rooms</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(HotelDetailDataTransfer), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HotelDetailDataTransfer>> GetHotel(int id)
    {
        try
        {
            var hotel = await _hotelService.GetHotelByIdAsync(id);

            if (hotel == null)
                return NotFound($"Hotel with ID {id} not found.");

            return Ok(hotel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hotel with ID: {Id}", id);
            return InternalServerError(ex);
        }
    }

    /// <summary>
    /// Find available rooms in a hotel for specified dates and guest count
    /// </summary>
    /// <param name="id">The hotel ID</param>
    /// <param name="checkIn">Check-in date (format: yyyy-MM-dd)</param>
    /// <param name="checkOut">Check-out date (format: yyyy-MM-dd)</param>
    /// <param name="guestCount">Number of guests</param>
    /// <param name="roomType">Optional room type filter (Single, Double, Deluxe)</param>
    /// <returns>List of available rooms</returns>
    [HttpGet("{id:int}/rooms/available")]
    [ProducesResponseType(typeof(IEnumerable<AvailableRoomDataTransfer>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AvailableRoomDataTransfer>>> GetAvailableRooms(
        int id,
        [FromQuery] DateOnly checkIn,
        [FromQuery] DateOnly checkOut,
        [FromQuery] int guestCount,
        [FromQuery] string? roomType = null)
    {
        // Validate dates
        if (checkOut <= checkIn)
            return BadRequest(new { message = "Check-out date must be after check-in date." });
        if (checkIn < DateOnly.FromDateTime(DateTime.Today))
            return BadRequest(new { message = "Check-in date cannot be in the past." });

        // Check number of guests
        if (guestCount < 1)
            return BadRequest(new { message = "At least 1 guest is required." });

        // Validate room type if provided
        if (!string.IsNullOrWhiteSpace(roomType))
        {
            var roomTypeInfo = await _hotelService.GetRoomTypeByNameAsync(roomType);
            if (roomTypeInfo == null)
            {
                var validTypes = await _hotelService.GetValidRoomTypeNamesAsync();
                return BadRequest(new { message = $"Invalid room type '{roomType}'. Valid types are: {string.Join(", ", validTypes)}." });
            }
        }

        try
        {
            // Check if hotel exists
            var hotel = await _hotelService.GetHotelByIdAsync(id);
            if (hotel == null)
                return NotFound($"Hotel with ID {id} not found.");

            var availableRooms = await _hotelService.GetAvailableRoomsAsync(id, checkIn, checkOut, guestCount, roomType);
            if (availableRooms.Any())
                return Ok(availableRooms);

            var message = string.IsNullOrWhiteSpace(roomType)
                ? "There are no available rooms in this hotel for the specified dates."
                : $"There are no available '{roomType}' rooms in this hotel for the specified dates.";
            return NotFound(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available rooms for hotel {Id}, checkIn: {CheckIn}, checkOut: {CheckOut}, guests: {GuestCount}, roomType: {RoomType}",
                id, checkIn, checkOut, guestCount, roomType);
            return InternalServerError(ex);
        }
    }
}

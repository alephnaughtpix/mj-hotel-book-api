using Microsoft.AspNetCore.Mvc;
using HotelBookingApi.DataTransfer;
using HotelBookingApi.Services;

namespace HotelBookingApi.Controllers;

[Route("api/[controller]")]
public class BookingsController : ApiControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IBookingService bookingService, ILogger<BookingsController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new room booking
    /// </summary>
    /// <param name="bookingRequest">Booking details</param>
    /// <returns>The created booking with reference number</returns>
    [HttpPost]
    [ProducesResponseType(typeof(BookingResponseDataTransfer), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookingResponseDataTransfer>> CreateBooking([FromBody] CreateBookingDataTransfer bookingRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var (booking, error) = await _bookingService.CreateBookingAsync(bookingRequest);

            if (error != null)
                return BadRequest(new { message = error });

            return CreatedAtAction(
                nameof(GetBookingByReference),
                new { reference = booking!.BookingReference },
                booking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for hotel {HotelId}", bookingRequest.HotelId);
            return InternalServerError(ex);
        }
    }

    /// <summary>
    /// Get booking details by reference number
    /// </summary>
    /// <param name="reference">Booking reference number</param>
    /// <returns>Booking details</returns>
    [HttpGet("{reference}")]
    [ProducesResponseType(typeof(BookingResponseDataTransfer), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookingResponseDataTransfer>> GetBookingByReference(string reference)
    {
        if (string.IsNullOrWhiteSpace(reference))
            return BadRequest(new { message = "Booking reference number is required." });

        try
        {
            var booking = await _bookingService.GetBookingByReferenceAsync(reference);

            if (booking == null)
                return NotFound($"Booking with reference '{reference}' not found.");

            return Ok(booking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting booking with reference: {Reference}", reference);
            return InternalServerError(ex);
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace HotelBookingApi.DataTransfer;

/// <summary>
/// Data required to book a room in a hotel
/// </summary>
public class CreateBookingDataTransfer
{
    /// <summary>
    /// Database ID of Hotel
    /// </summary>
    [Required]
    public int HotelId { get; set; }

    /// <summary>
    /// Optional preferred room type (Single, Double, Deluxe). If not specified, any available room will be allocated.
    /// </summary>
    public string? RoomType { get; set; }

    /// <summary>
    /// First name of guest
    /// </summary>
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name of guest
    /// </summary>
    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Address of guest
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Guest's contact phone number
    /// </summary>
    [Required]
    [StringLength(50)]
    public string ContactNumber { get; set; } = string.Empty;

    /// <summary>
    /// Number of guests
    /// </summary>
    [Required]
    [Range(1, 10)]
    public int GuestCount { get; set; }

    /// <summary>
    /// Date of check in, in YYYY-MM-DD format
    /// </summary>
    [Required]
    public DateOnly CheckInDate { get; set; }

    /// <summary>
    /// Date of check out, in YYYY-MM-DD format
    /// </summary>
    [Required]
    public DateOnly CheckOutDate { get; set; }
}

/// <summary>
/// Booking confirmation
/// </summary>
public class BookingResponseDataTransfer
{
    /// <summary>
    /// Database ID of booking
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Boooking reference number
    /// </summary>
    public string BookingReference { get; set; } = string.Empty;

    /// <summary>
    /// First name of guest
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name of guest
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Address of guest
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Contact phone number of guest
    /// </summary>
    public string ContactNumber { get; set; } = string.Empty;

    /// <summary>
    /// Number of guests
    /// </summary>
    public int GuestCount { get; set; }

    /// <summary>
    /// Date of check in, in YYYY-MM-DD format
    /// </summary>
    public DateOnly CheckInDate { get; set; }

    /// <summary>
    /// Date of check out, in YYYY-MM-DD format
    /// </summary>
    public DateOnly CheckOutDate { get; set; }

    /// <summary>
    /// Date booking was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Room details
    /// </summary>
    public RoomDataTransfer Room { get; set; } = null!;

    /// <summary>
    /// Name of hotel
    /// </summary>
    public string HotelName { get; set; } = string.Empty;

    /// <summary>
    /// Address of hotel
    /// </summary>
    public string HotelAddress { get; set; } = string.Empty;
}

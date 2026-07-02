using System.ComponentModel.DataAnnotations;

namespace HotelBookingApi.DataTransfer;

public class CreateBookingDataTransfer
{
    [Required]
    public int HotelId { get; set; }

    /// <summary>
    /// Optional preferred room type (Single, Double, Deluxe). If not specified, any available room will be allocated.
    /// </summary>
    public string? RoomType { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string ContactNumber { get; set; } = string.Empty;

    [Required]
    [Range(1, 10)]
    public int GuestCount { get; set; }

    [Required]
    public DateOnly CheckInDate { get; set; }

    [Required]
    public DateOnly CheckOutDate { get; set; }
}

public class BookingResponseDataTransfer
{
    public int Id { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public int GuestCount { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public RoomDataTransfer Room { get; set; } = null!;
    public string HotelName { get; set; } = string.Empty;
    public string HotelAddress { get; set; } = string.Empty;
}

namespace HotelBookingApi.Models;

public class Room
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    
    // Foreign keys
    public int HotelId { get; set; }
    public int RoomTypeId { get; set; }

    // Navigation properties
    public Hotel Hotel { get; set; } = null!;
    public RoomType RoomType { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

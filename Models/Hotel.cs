namespace HotelBookingApi.Models;

public class Hotel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    // Navigation property
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}

namespace HotelBookingApi.Models;

public class RoomType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }

    // Navigation property
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}

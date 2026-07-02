namespace HotelBookingApi.DataTransfer;

public class HotelDataTransfer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int TotalRooms { get; set; }
}

public class HotelDetailDataTransfer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public List<RoomDataTransfer> Rooms { get; set; } = new();
}

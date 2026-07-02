namespace HotelBookingApi.DataTransfer;

public class RoomDataTransfer
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomTypeName { get; set; } = string.Empty;
    public int Capacity { get; set; }
}

public class AvailableRoomDataTransfer
{
    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomTypeName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
}

public class RoomTypeDataTransfer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
}

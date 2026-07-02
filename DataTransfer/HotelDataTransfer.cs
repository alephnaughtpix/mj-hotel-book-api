namespace HotelBookingApi.DataTransfer;

/// <summary>
/// Basic hotel details
/// </summary>
public class HotelDataTransfer
{
    /// <summary>
    /// Database Id of hotel
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Hotel name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Hotel address
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Number of rooms in the hotel
    /// </summary>
    public int TotalRooms { get; set; }
}

/// <summary>
/// Hotel details including rooms
/// </summary>
public class HotelDetailDataTransfer
{
    /// <summary>
    /// Database Id of hotel
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Hotel name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Hotel address
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// List of rooms and their details
    /// </summary>
    public List<RoomDataTransfer> Rooms { get; set; } = new();
}

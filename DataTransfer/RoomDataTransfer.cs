namespace HotelBookingApi.DataTransfer;


/// <summary>
/// Room details
/// </summary>
public class RoomDataTransfer
{
    /// <summary>
    /// Database ID for room
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Human readable room number
    /// </summary>
    public string RoomNumber { get; set; } = string.Empty;

    /// <summary>
    /// Room type name
    /// </summary>
    public string RoomTypeName { get; set; } = string.Empty;

    /// <summary>
    /// Capacity of room in people
    /// </summary>
    public int Capacity { get; set; }
}

/// <summary>
/// Details about an available room
/// </summary>
public class AvailableRoomDataTransfer
{
    /// <summary>
    /// Database ID for room
    /// </summary>
    public int RoomId { get; set; }

    /// <summary>
    /// Human readable room number
    /// </summary>
    public string RoomNumber { get; set; } = string.Empty;

    /// <summary>
    /// Room type name
    /// </summary>
    public string RoomTypeName { get; set; } = string.Empty;

    /// <summary>
    /// Capacity of room in people
    /// </summary>
    public int Capacity { get; set; }

    /// <summary>
    /// Database ID for hotel the room is contained in
    /// </summary>
    public int HotelId { get; set; }

    /// <summary>
    /// Name of hotel the room is contained in
    /// </summary>
    public string HotelName { get; set; } = string.Empty;
}

/// <summary>
/// Details about room type
/// </summary>
public class RoomTypeDataTransfer
{
    /// <summary>
    /// Database ID for room type
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Room type name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Capacity of room type in people
    /// </summary>
    public int Capacity { get; set; }
}

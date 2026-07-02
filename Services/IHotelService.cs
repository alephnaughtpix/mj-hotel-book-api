using HotelBookingApi.DataTransfer;

namespace HotelBookingApi.Services;

public interface IHotelService
{
    Task<IEnumerable<HotelDataTransfer>> SearchHotelsByNameAsync(string name);
    Task<HotelDetailDataTransfer?> GetHotelByIdAsync(int id);
    Task<IEnumerable<AvailableRoomDataTransfer>> GetAvailableRoomsAsync(int hotelId, DateOnly checkIn, DateOnly checkOut, int guestCount, string? roomType = null);
    Task<RoomTypeDataTransfer?> GetRoomTypeByNameAsync(string roomTypeName);
    Task<IEnumerable<string>> GetValidRoomTypeNamesAsync();
}

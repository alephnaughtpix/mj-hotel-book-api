using HotelBookingApi.DataTransfer;

namespace HotelBookingApi.Services;

public interface IBookingService
{
    Task<BookingResponseDataTransfer?> GetBookingByReferenceAsync(string bookingReference);
    Task<(BookingResponseDataTransfer? Booking, string? Error)> CreateBookingAsync(CreateBookingDataTransfer bookingRequest);
}

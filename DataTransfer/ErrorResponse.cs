namespace HotelBookingApi.DataTransfer;

/// <summary>
/// Standard error response format for API errors.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// A user-friendly error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Technical error details (e.g., exception message).
    /// </summary>
    public string? Error { get; set; }

    public ErrorResponse() { }

    public ErrorResponse(string message, string? error = null)
    {
        Message = message;
        Error = error;
    }

    /// <summary>
    /// Creates a standard error response for unexpected exceptions.
    /// </summary>
    public static ErrorResponse FromException(Exception ex)
    {
        return new ErrorResponse(
            "An unexpected error occurred while processing your request.",
            ex.Message
        );
    }
}

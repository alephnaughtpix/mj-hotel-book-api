using Microsoft.AspNetCore.Mvc;
using HotelBookingApi.DataTransfer;

namespace HotelBookingApi.Controllers;

/// <summary>
/// Base controller providing common functionality for all API controllers.
/// </summary>
[ApiController]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Returns a 500 Internal Server Error response with a standardized error format.
    /// </summary>
    /// <param name="ex">The exception that occurred.</param>
    /// <returns>An ObjectResult with status code 500 and an ErrorResponse body.</returns>
    protected ObjectResult InternalServerError(Exception ex)
    {
        return StatusCode(StatusCodes.Status500InternalServerError, ErrorResponse.FromException(ex));
    }

}

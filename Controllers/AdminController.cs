using Microsoft.AspNetCore.Mvc;
using HotelBookingApi.Data;
using HotelBookingApi.DataTransfer;

namespace HotelBookingApi.Controllers;

[Route("api/[controller]")]
public class AdminController : ApiControllerBase
{
    private readonly DataSeeder _dataSeeder;
    private readonly ILogger<AdminController> _logger;

    public AdminController(DataSeeder dataSeeder, ILogger<AdminController> logger)
    {
        _dataSeeder = dataSeeder;
        _logger = logger;
    }

    /// <summary>
    /// Seed the database with test data.
    /// </summary>
    /// <remarks>
    /// This will populate the database with:
    /// - 10 Hotels with fake names and addresses
    /// - 6 Rooms per hotel (2 of each room type)
    /// - Sample bookings
    /// 
    /// Note: 
    /// * Room types (Single, Double, Deluxe) are created by database migrations.
    /// * If data already exists, seeding will be skipped.
    ///   Call the reset endpoint first if you want to start fresh.
    /// </remarks>
    /// <returns>Status message</returns>
    [HttpPost("seed")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SeedDatabase()
    {
        try
        {
            var success = await _dataSeeder.SeedAsync();
            var message = success
                ? "Database seeded successfully with test data."
                : "Database already contains data. Seeding was skipped. Use the reset endpoint first if you want to re-seed.";

            return Ok(new { message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding database");
            return InternalServerError(ex);
        }
    }

    /// <summary>
    /// Reset seeded test data
    /// </summary>
    /// <remarks>
    /// This will delete all test data from the database including bookings, rooms and hotels.
    /// 
    /// Note: Room types are not deleted.
    /// </remarks>
    /// <returns>Status message</returns>
    [HttpPost("reset")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ResetDatabase()
    {
        try
        {
            await _dataSeeder.ResetAsync();
            return Ok(new { message = "Database reset successfully. All test data has been removed." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting database");
            return InternalServerError(ex);
        }
    }
}

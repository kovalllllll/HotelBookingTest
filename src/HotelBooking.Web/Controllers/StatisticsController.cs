using HotelBooking.Application.Models.Statistics;
using HotelBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class StatisticsController(IStatisticsRepository statisticsRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<BookingStatisticsModel>> GetStatistics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var statistics = await statisticsRepository.GetBookingStatisticsAsync(startDate, endDate);
        return Ok(statistics);
    }
}
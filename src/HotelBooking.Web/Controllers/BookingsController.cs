using HotelBooking.Application.Models.Bookings;
using HotelBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController(IBookingService bookingService, IUserContext userContext) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<IEnumerable<BookingModel>>> GetAll()
    {
        var bookings = await bookingService.GetAllBookingsAsync();
        return Ok(bookings);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingModel>> GetById(int id)
    {
        var booking = await bookingService.GetBookingByIdAsync(id);
        if (booking == null)
            return NotFound();

        var userId = userContext.UserId;
        if (booking.UserId != userId && !userContext.IsInRole("Administrator"))
            return Forbid();

        return Ok(booking);
    }

    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<BookingModel>>> GetMyBookings()
    {
        var userId = userContext.UserId;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var bookings = await bookingService.GetUserBookingsAsync(userId);
        return Ok(bookings);
    }

    [HttpPost]
    public async Task<ActionResult<BookingModel>> Create([FromBody] CreateBookingModel model)
    {
        var userId = userContext.UserId;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var booking = await bookingService.CreateBookingAsync(userId, model);
            return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<BookingModel>> UpdateStatus(int id, [FromBody] UpdateBookingStatusModel model)
    {
        var booking = await bookingService.UpdateBookingStatusAsync(id, model);
        if (booking == null)
            return NotFound();
        return Ok(booking);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = userContext.UserId;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            if (userContext.IsInRole("Administrator"))
            {
                var booking = await bookingService.GetBookingByIdAsync(id);
                if (booking == null)
                    return NotFound();
                userId = booking.UserId;
            }

            var result = await bookingService.CancelBookingAsync(id, userId);
            if (!result)
                return NotFound();
            return Ok(new { message = "Booking cancelled successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}


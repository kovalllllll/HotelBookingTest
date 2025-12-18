using HotelBooking.Application.DTOs;
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
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetAll()
    {
        var bookings = await bookingService.GetAllBookingsAsync();
        return Ok(bookings);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingDto>> GetById(int id)
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
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetMyBookings()
    {
        var userId = userContext.UserId;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var bookings = await bookingService.GetUserBookingsAsync(userId);
        return Ok(bookings);
    }

    [HttpPost]
    public async Task<ActionResult<BookingDto>> Create([FromBody] CreateBookingDto dto)
    {
        var userId = userContext.UserId;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var booking = await bookingService.CreateBookingAsync(userId, dto);
            return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<BookingDto>> UpdateStatus(int id, [FromBody] UpdateBookingStatusDto dto)
    {
        var booking = await bookingService.UpdateBookingStatusAsync(id, dto);
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


using System.Security.Claims;
using HotelBooking.Application.DTOs;
using HotelBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBooking.Web.Pages;

[Authorize]
public class BookingsModel(IBookingService bookingService) : PageModel
{
    public IEnumerable<BookingDto> Bookings { get; set; } = [];

    public async Task OnGetAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
            Bookings = await bookingService.GetUserBookingsAsync(userId);
        }
    }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostCancelAsync(int bookingId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            await bookingService.CancelBookingAsync(bookingId, userId);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            ErrorMessage = ex.Message;
            return RedirectToPage();
        }

        return RedirectToPage();
    }
}


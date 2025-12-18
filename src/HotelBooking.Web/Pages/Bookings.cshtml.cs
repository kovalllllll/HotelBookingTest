using HotelBooking.Application.Models.Bookings;
using HotelBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBooking.Web.Pages;

[Authorize]
public class BookingsModel(IBookingService bookingService, IUserContext userContext) : PageModel
{
    public IEnumerable<BookingModel> Bookings { get; set; } = [];

    public async Task OnGetAsync()
    {
        var userId = userContext.UserId;
        if (!string.IsNullOrEmpty(userId))
        {
            Bookings = await bookingService.GetUserBookingsAsync(userId);
        }
    }

    [TempData] public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostCancelAsync(int bookingId)
    {
        var userId = userContext.UserId;
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
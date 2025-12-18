using HotelBooking.Application.Models.Bookings;
using HotelBooking.Application.Interfaces.Common;
using HotelBooking.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBooking.Web.Pages;

[Authorize]
public class BookingsModel(IBookingService bookingService, IUserContext userContext) : PageModel
{
    public IEnumerable<BookingModel> Bookings { get; set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = userContext.UserId;
        if (!string.IsNullOrEmpty(userId))
        {
            Bookings = await bookingService.GetUserBookingsAsync(userId, cancellationToken);
        }
    }

    [TempData] public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostCancelAsync(int bookingId, CancellationToken cancellationToken)
    {
        var userId = userContext.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            await bookingService.CancelBookingAsync(bookingId, userId, cancellationToken);
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
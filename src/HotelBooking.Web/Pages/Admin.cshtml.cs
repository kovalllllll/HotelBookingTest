using HotelBooking.Application.Models.Bookings;
using HotelBooking.Application.Models.Statistics;
using HotelBooking.Application.Interfaces.Persistence;
using HotelBooking.Application.Interfaces.Services;
using HotelBooking.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBooking.Web.Pages;

[Authorize(Roles = "Administrator")]
public class AdminModel(IBookingService bookingService, IStatisticsRepository statisticsRepository)
    : PageModel
{
    public BookingStatisticsModel Statistics { get; set; } = new();
    public IEnumerable<BookingModel> Bookings { get; set; } = Enumerable.Empty<BookingModel>();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Statistics = await statisticsRepository.GetBookingStatisticsAsync(cancellationToken: cancellationToken);
        Bookings = await bookingService.GetAllBookingsAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostConfirmAsync(int bookingId, CancellationToken cancellationToken)
    {
        await bookingService.UpdateBookingStatusAsync(bookingId, new UpdateBookingStatusModel 
        { 
            Status = BookingStatus.Confirmed 
        }, cancellationToken);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCancelAsync(int bookingId, CancellationToken cancellationToken)
    {
        await bookingService.UpdateBookingStatusAsync(bookingId, new UpdateBookingStatusModel 
        { 
            Status = BookingStatus.Cancelled 
        }, cancellationToken);
        return RedirectToPage();
    }
}


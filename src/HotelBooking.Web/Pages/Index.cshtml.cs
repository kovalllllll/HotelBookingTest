using HotelBooking.Application.Models.Hotels;
using HotelBooking.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBooking.Web.Pages;

public class IndexModel(IHotelService hotelService) : PageModel
{
    public IEnumerable<HotelModel> Hotels { get; set; } = [];
    
    [BindProperty(SupportsGet = true)]
    public string? City { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public DateTime? CheckIn { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public DateTime? CheckOut { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(City))
        {
            Hotels = await hotelService.GetHotelsByCityAsync(City, cancellationToken);
        }
        else
        {
            Hotels = await hotelService.GetAllHotelsAsync(cancellationToken);
        }
    }
}


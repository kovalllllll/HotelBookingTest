using HotelBooking.Web.Middleware;

namespace HotelBooking.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        
        services.AddHttpContextAccessor();
        
        services.AddControllers();
        services.AddRazorPages();
        
        return services;
    }
}


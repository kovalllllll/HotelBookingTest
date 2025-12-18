using HotelBooking.Application.Interfaces.Common;
using HotelBooking.Application.Interfaces.Persistence;
using HotelBooking.Application.Interfaces.Services;
using HotelBooking.Application.Services;
using HotelBooking.Infrastructure.Context;
using HotelBooking.Infrastructure.Identity;
using HotelBooking.Infrastructure.Persistence;
using HotelBooking.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HotelBooking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IHotelRepository, HotelRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IStatisticsRepository, StatisticsRepository>();

        services.AddScoped<IHotelService, HotelService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IBookingService, BookingService>();

        services.AddScoped<IUserContext, UserContext>();

        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
    {
        try
        {
            await SeedData.InitializeAsync(services);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
            logger.LogError(ex, "An error occurred while initializing the database.");
        }
    }
}
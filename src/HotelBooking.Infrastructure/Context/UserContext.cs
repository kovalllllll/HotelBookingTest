using System.Security.Claims;
using HotelBooking.Application.Interfaces.Common;
using Microsoft.AspNetCore.Http;

namespace HotelBooking.Infrastructure.Context;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public string? UserId => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
    
    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    
    public bool IsInRole(string role) => httpContextAccessor.HttpContext?.User.IsInRole(role) ?? false;
}


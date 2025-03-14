using ReservacionesHotel.Models.Entities;

namespace ReservacionesHotel.Interfaces.Services
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        string? Email { get; }
        string? Role { get; }
        bool IsAuthenticated { get; }
        bool IsAdmin { get; }
        Task<Users?> GetCurrentUserAsync();
    }
}
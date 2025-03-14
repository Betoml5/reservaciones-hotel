using ReservacionesHotel.Models.Entities;
using ReservacionesHotel.Models.Entities.DTOs;

namespace ReservacionesHotel.Interfaces.Services
{
    public interface IAuthService
    {
        Task<(bool success, string token)> AuthenticateAsync(LoginDTO user);
        Task<Users> RegisterAsync(UserCreateDTO user);
        Task<string> GenerateTokenAsync(Users user);
    }
}
using ReservacionesHotel.Models.Entities;
using ReservacionesHotel.Models.Entities.DTOs;

namespace ReservacionesHotel.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO> GetUserByIdAsync(int id);
        Task<UserDTO> GetUserByEmailAsync(string email);
        Task<UserDTO> CreateUserAsync(UserCreateDTO user);
        Task<UserDTO> UpdateUserAsync(UserUpdateDTO user);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ValidateUserCredentialsAsync(string email, string password);
        Task<IEnumerable<ReservationDTO>> GetUserReservationsAsync(int userId);
    }
}
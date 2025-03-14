using ReservacionesHotel.Models.Entities;
using ReservacionesHotel.Repositories;

namespace ReservacionesHotel.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<Users>
    {
        Task<Users> GetUserByEmailAsync(string email);
        Task<bool> ValidateUserCredentialsAsync(string email, string passwordHash);
    }
}
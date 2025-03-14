using Microsoft.EntityFrameworkCore;
using ReservacionesHotel.Interfaces.Repositories;
using ReservacionesHotel.Models.Entities;

namespace ReservacionesHotel.Repositories
{
    public class UserRepository : Repository<Users>, IUserRepository
    {
        public UserRepository(HotelContext context) : base(context)
        {
        }

        public async Task<Users> GetUserByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ValidateUserCredentialsAsync(string email, string passwordHash)
        {
            return await _dbSet.AnyAsync(u => u.Email == email && u.PasswordHash == passwordHash);
        }
    }
}
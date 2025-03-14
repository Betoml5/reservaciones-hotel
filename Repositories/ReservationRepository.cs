using Microsoft.EntityFrameworkCore;
using ReservacionesHotel.Interfaces.Repositories;
using ReservacionesHotel.Models.Entities;

namespace ReservacionesHotel.Repositories
{
    public class ReservationRepository : Repository<Reservations>, IReservationRepository
    {
        public ReservationRepository(HotelContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Reservations>> GetAllAsync()
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.Room)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservations>> GetUserReservationsAsync(int userId)
        {
            return await _dbSet
                .Include(r => r.Room)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservations>> GetRoomReservationsAsync(int roomId)
        {
            return await _dbSet
                .Include(r => r.User)
                .Where(r => r.RoomId == roomId)
                .ToListAsync();
        }

        public async Task<bool> IsRoomAvailableForDatesAsync(int roomId, DateOnly checkIn, DateOnly checkOut)
        {
            return !await _dbSet.AnyAsync(r =>
                r.RoomId == roomId &&
                ((checkIn >= r.CheckInDate && checkIn < r.CheckOutDate) ||
                (checkOut > r.CheckInDate && checkOut <= r.CheckOutDate)));
        }

        public async Task<bool> HasActiveReservationsForRoomAsync(int roomId)
        {
            return await _dbSet.AnyAsync(r => r.RoomId == roomId && r.Status == ReservationsStatus.BOOKED);
        }
    }
}
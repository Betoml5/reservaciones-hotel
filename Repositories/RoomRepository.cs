using Microsoft.EntityFrameworkCore;
using ReservacionesHotel.Interfaces.Repositories;
using ReservacionesHotel.Models.Entities;

namespace ReservacionesHotel.Repositories
{
    public class RoomRepository : Repository<Rooms>, IRoomRepository
    {
        public RoomRepository(HotelContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Rooms>> GetAvailableRoomsAsync(DateOnly checkIn, DateOnly checkOut)
        {
            return await _dbSet
                .Where(r => r.Status == RoomsStatus.AVAILABLE &&
                    !r.Reservations.Any(res =>
                        (checkIn >= res.CheckInDate && checkIn < res.CheckOutDate) ||
                        (checkOut > res.CheckInDate && checkOut <= res.CheckOutDate)))
                .ToListAsync();
        }

        public async Task<Rooms> GetRoomByNumberAsync(string roomNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.RoomNumber == roomNumber);
        }
    }
}
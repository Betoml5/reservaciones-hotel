using ReservacionesHotel.Models.Entities;
using ReservacionesHotel.Repositories;

namespace ReservacionesHotel.Interfaces.Repositories
{
    public interface IRoomRepository : IRepository<Rooms>
    {
        Task<IEnumerable<Rooms>> GetAvailableRoomsAsync(DateOnly checkIn, DateOnly checkOut);
        Task<Rooms> GetRoomByNumberAsync(string roomNumber);
    }
}
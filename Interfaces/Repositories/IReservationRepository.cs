using ReservacionesHotel.Models.Entities;
using ReservacionesHotel.Repositories;

namespace ReservacionesHotel.Interfaces.Repositories
{
    public interface IReservationRepository : IRepository<Reservations>
    {
        Task<IEnumerable<Reservations>> GetUserReservationsAsync(int userId);
        Task<IEnumerable<Reservations>> GetRoomReservationsAsync(int roomId);
        Task<bool> IsRoomAvailableForDatesAsync(int roomId, DateOnly checkIn, DateOnly checkOut);
        Task<bool> HasActiveReservationsForRoomAsync(int roomId);
    }
}
using ReservacionesHotel.Models.Entities;
using ReservacionesHotel.Models.Entities.DTOs;

namespace ReservacionesHotel.Interfaces.Services
{
    public interface IReservationService
    {
        Task<IEnumerable<ReservationDTO>> GetAllReservationsAsync();
        Task<ReservationDetailsDTO> GetReservationByIdAsync(int id);
        Task<IEnumerable<ReservationDTO>> GetUserReservationsAsync(int userId);
        Task<IEnumerable<ReservationDTO>> GetRoomReservationsAsync(int roomId);
        Task<ReservationDTO> CreateReservationAsync(ReservationCreateDTO reservationCreateDTO);
        Task<ReservationDTO> UpdateReservationAsync(ReservationUpdateDTO reservationUpdateDTO, int id);
        Task<bool> DeleteReservationAsync(int id);
        Task<bool> IsRoomAvailableForDatesAsync(int roomId, DateOnly checkIn, DateOnly checkOut);
        Task<decimal> CalculateTotalPriceAsync(int roomId, DateOnly checkIn, DateOnly checkOut);
    }
}
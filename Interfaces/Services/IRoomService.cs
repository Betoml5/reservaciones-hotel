using ReservacionesHotel.Models.Entities;
using ReservacionesHotel.Models.Entities.DTOs;

namespace ReservacionesHotel.Interfaces.Services
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomDTO>> GetAllRoomsAsync();
        Task<RoomDTO> GetRoomByIdAsync(int id);
        Task<RoomDTO> GetRoomByNumberAsync(string roomNumber);
        Task<IEnumerable<RoomDTO>> GetAvailableRoomsAsync(DateOnly checkIn, DateOnly checkOut);
        Task<RoomDTO> CreateRoomAsync(RoomCreateDTO room);
        Task<RoomDTO> UpdateRoomAsync(RoomUpdateDTO room);
        Task<bool> DeleteRoomAsync(int id);
        Task<bool> IsRoomAvailableAsync(int roomId, DateOnly checkIn, DateOnly checkOut);
    }
}
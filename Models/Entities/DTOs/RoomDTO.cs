using System;

namespace ReservacionesHotel.Models.Entities.DTOs
{
    public class RoomDTO
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }
        public decimal PricePerNight { get; set; }
        public bool IsAvailable { get; set; }
        public int Capacity { get; set; }
    }

    public class RoomCreateDTO
    {
        public string RoomNumber { get; set; }
        public decimal PricePerNight { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int Capacity { get; set; }
    }

    public class RoomUpdateDTO
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }
        public decimal PricePerNight { get; set; }
        public bool IsAvailable { get; set; }
        public int Capacity { get; set; }
    }
}
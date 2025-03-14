using System;

namespace ReservacionesHotel.Models.Entities.DTOs
{
    public class ReservationDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public UserDTO User { get; set; }
        public RoomDTO Room { get; set; }
    }

    public class ReservationCreateDTO
    {
        public int RoomId { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
    }

    public class ReservationUpdateDTO
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }

        public string Status { get; set; }
    }

    public class ReservationDetailsDTO
    {
        public int Id { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public RoomDTO Room { get; set; }

        public string UserName { get; set; }
        public string UserEmail { get; set; }
    }
}
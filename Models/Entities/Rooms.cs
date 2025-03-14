using System;
using System.Collections.Generic;

namespace ReservacionesHotel.Models.Entities;

public partial class Rooms
{
    public int Id { get; set; }

    public string RoomNumber { get; set; } = null!;

    public int Capacity { get; set; }

    public string? Status { get; set; }

    public decimal PricePerNight { get; set; }

    public virtual ICollection<Reservations> Reservations { get; set; } = new List<Reservations>();
}

using System;
using System.Collections.Generic;

namespace ReservacionesHotel.Models.Entities;

public partial class Reservations
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int RoomId { get; set; }

    public DateOnly CheckInDate { get; set; }

    public DateOnly CheckOutDate { get; set; }

    public decimal TotalPrice { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Rooms Room { get; set; } = null!;

    public virtual Users User { get; set; } = null!;
}

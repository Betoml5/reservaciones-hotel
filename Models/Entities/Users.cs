using System;
using System.Collections.Generic;

namespace ReservacionesHotel.Models.Entities;

public partial class Users
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Role { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Reservations> Reservations { get; set; } = new List<Reservations>();
}

using System;
using System.Collections.Generic;

namespace Acceloka.WebApiStandard.Entities;

public partial class Booking
{
    public int Id { get; set; }

    public DateTime BookingDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual ICollection<BookingTicket> BookingTickets { get; set; } = new List<BookingTicket>();
}

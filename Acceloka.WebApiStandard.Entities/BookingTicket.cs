using System;
using System.Collections.Generic;

namespace Acceloka.WebApiStandard.Entities;

public partial class BookingTicket
{
    public int BookingId { get; set; }

    public string TicketCode { get; set; } = null!;

    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual Booking Booking { get; set; } = null!;

    public virtual Ticket TicketCodeNavigation { get; set; } = null!;
}

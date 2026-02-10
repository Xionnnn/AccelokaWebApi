using System;
using System.Collections.Generic;

namespace Acceloka.WebApiStandard.Entities;

public partial class Ticket
{
    public string TicketCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime EventDate { get; set; }

    public decimal Price { get; set; }

    public int Quota { get; set; }

    public int? CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public virtual ICollection<BookingTicket> BookingTickets { get; set; } = new List<BookingTicket>();

    public virtual Category? Category { get; set; }
}

using Acceloka.WebApiStandard.Contracts.ResponseModels.ManageTickets;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acceloka.WebApiStandard.Contracts.RequestModels.ManageTickets
{
    public class BookTicketRequest : IRequest<BookTicketResponse>
    {
        public List<BookTicketDto> Items { get; set; } = new List<BookTicketDto>();

        public DateTime BookingDate = DateTime.UtcNow;
    }

    public class BookTicketDto
    {
        public string TicketCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}

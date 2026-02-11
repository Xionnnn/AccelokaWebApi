using Acceloka.WebApiStandard.Contracts.ResponseModels.ManageTickets;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acceloka.WebApiStandard.Contracts.RequestModels.ManageTickets
{
    public class BookTicketRequest : IRequest<BookTicketResponse>
    {
        public List<BookTicketDto> Items { get; set; }

        public DateTime bookingDate = DateTime.Now;
    }

    public class BookTicketDto
    {
        public string TicketCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}

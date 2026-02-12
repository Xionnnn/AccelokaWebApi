using Acceloka.WebApiStandard.Contracts.ResponseModels.ManageTickets;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acceloka.WebApiStandard.Contracts.RequestModels.ManageTickets
{
    public class EditBookedTicketRequest : IRequest<IReadOnlyList<EditBookedTicketResponse>>
    {
        public int BookedTicketId { get; set; }

        public IReadOnlyList<EditBookTicketDto> Items { get; set; } = new List<EditBookTicketDto>();
    }

    public class EditBookTicketDto
    {
        public string TicketCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}

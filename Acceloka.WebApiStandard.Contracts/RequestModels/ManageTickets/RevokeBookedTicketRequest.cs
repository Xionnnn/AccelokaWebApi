using Acceloka.WebApiStandard.Contracts.ResponseModels.ManageTickets;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acceloka.WebApiStandard.Contracts.RequestModels.ManageTickets
{
    public class RevokeBookedTicketRequest : IRequest<RevokeBookedTicketResponse>
    {
        public int BookedTicketId { get; set; }
        public string TicketCode { get; set; } = string.Empty;
        public int Qty { get; set; }
    }
}

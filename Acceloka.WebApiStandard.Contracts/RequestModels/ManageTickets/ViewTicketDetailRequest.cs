using Acceloka.WebApiStandard.Contracts.ResponseModels.ManageTickets;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acceloka.WebApiStandard.Contracts.RequestModels.ManageTickets
{
    public class ViewTicketDetailRequest : IRequest<IReadOnlyList<ViewTicketDetailResponse>>
    {
        public int BookedTicketId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Acceloka.WebApiStandard.Contracts.ResponseModels.ManageTickets
{
    public class ViewTicketDetailResponse
    {
        public int QtyPerCategory { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public IReadOnlyList<DetailTicketDto> Tickets { get; set; } = Array.Empty<DetailTicketDto>();

    }

    public class DetailTicketDto
    {
        public string TicketCode { get; set; } = string.Empty;
        public string TicketName { get; set; } = string.Empty;
        public string EventDate { get; set; } = string.Empty;
    }
}

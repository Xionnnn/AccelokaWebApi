using System;
using System.Collections.Generic;
using System.Text;

namespace Acceloka.WebApiStandard.Contracts.ResponseModels.ManageTickets
{
    public class GetAvailableTicketResponse
    {
        public IReadOnlyList<AvailableTicketDto> Tickets { get; set; } = Array.Empty<AvailableTicketDto>();

        //Pagination metadata
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalTickets { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
    public class AvailableTicketDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public string TicketCode { get; set; } = string.Empty;
        public string TicketName { get; set; } = string.Empty;
        public string EventDate { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quota { get; set; }
    }
}



using System;
using System.Collections.Generic;
using System.Text;

namespace Acceloka.WebApiStandard.Contracts.ResponseModels.ManageTickets
{
    public class GetBookingResponse
    {
        public IReadOnlyList<BookingDto> Bookings { get; set; } = Array.Empty<BookingDto>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalTickets { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

    public class BookingDto
    {
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public int BookingQuantity { get; set; }
        public decimal BookingPrice { get; set; }
    }
}

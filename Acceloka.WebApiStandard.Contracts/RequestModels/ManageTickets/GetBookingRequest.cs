using Acceloka.WebApiStandard.Contracts.ResponseModels.ManageTickets;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acceloka.WebApiStandard.Contracts.RequestModels.ManageTickets
{
    public class GetBookingRequest : IRequest<GetBookingResponse>
    {
        public int? BookingId { get; set; }
        public string OrderBy { get; set; } = string.Empty;
        public string OrderState { get; set; } = string.Empty;
        public DateTime? MinimalEventDate { get; set; }
        public DateTime? MaximalEventDate { get; set; }
        public decimal? BookingPrice { get; set;  }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 7;
    }
}

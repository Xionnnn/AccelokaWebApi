using Acceloka.WebApiStandard.Contracts.RequestModels.ManageTickets;
using Acceloka.WebApiStandard.Contracts.ResponseModels.ManageTickets;
using Acceloka.WebApiStandard.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Acceloka.WebApiStandard.RequestHandlers.ManageTickets
{
    public class EditBookedTicketHandler : IRequestHandler<EditBookedTicketRequest, IReadOnlyList<EditBookedTicketResponse>>
    {
        private readonly AccelokaDbContext _db;
        private readonly ILogger<BookTicketHandler> _logger;

        public EditBookedTicketHandler(
            AccelokaDbContext db,
            ILogger<BookTicketHandler> logger
            )
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IReadOnlyList<EditBookedTicketResponse>> Handle(EditBookedTicketRequest request, CancellationToken ct)
        {
            var filteredTicket = request.Items
                .GroupBy(i => i.TicketCode)
                .Select(x => new
                {
                    TicketCode = x.Key,
                    Quantity = x.Sum(i => i.Quantity)
                })
                .ToList();

            _logger.LogInformation($"Editing booked ticket id {request.BookedTicketId} with {filteredTicket.Count} items");

            var reqCodes = filteredTicket.Select(x => x.TicketCode).ToList();
            var qtyByCode = filteredTicket.ToDictionary(x => x.TicketCode, x => x.Quantity);

            var bookingInfo = await (from bt in _db.BookingTickets
                                     join t in _db.Tickets on bt.TicketCode equals t.TicketCode
                                     join c in _db.Categories on t.CategoryId equals c.Id
                                     where bt.BookingId == request.BookedTicketId && reqCodes.Contains(bt.TicketCode)
                                     select new
                                     {
                                         bookedTicket = bt,
                                         ticket = t,
                                         categoryName = c.Name,
                                     }).ToListAsync(ct);


            var reqTicketByCode = filteredTicket.ToDictionary(x => x.TicketCode, x => x);

            foreach(var item in bookingInfo)
            {
                var reqTicket = reqTicketByCode[item.ticket.TicketCode];

                //The ticket quota is now - the difference from the new qty - old qty
                //For example, if there are 7 remaining quota of the ticket, 3 has been booked and then changed to 5. The quota is reduced by 2 with 5 remaining
                //If there are 7 remaining quota, 5 have been booked and changed to 3. The quota is 3-5 = (-2) 7-(-2) so there are 9 remaining
                item.ticket.Quota -= (reqTicket.Quantity - item.bookedTicket.Quantity);
                item.bookedTicket.Quantity = reqTicket.Quantity;
            }


            var response = bookingInfo.Select(bi => new EditBookedTicketResponse
            {
                TicketCode = bi.ticket.TicketCode,
                TicketName = bi.ticket.Name,
                CategoryName = bi.categoryName,
                Quantity = bi.bookedTicket.Quantity
            }).ToList();


            await _db.SaveChangesAsync(ct);
            return response;
        }
    }
}

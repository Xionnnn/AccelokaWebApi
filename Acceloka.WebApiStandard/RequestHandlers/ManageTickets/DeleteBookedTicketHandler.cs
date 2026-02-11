using Acceloka.WebApiStandard.Contracts.RequestModels.ManageTickets;
using Acceloka.WebApiStandard.Contracts.ResponseModels.ManageTickets;
using Acceloka.WebApiStandard.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acceloka.WebApiStandard.RequestHandlers.ManageTickets
{
    public class DeleteBookedTicketHandler : IRequestHandler<DeleteBookedTicketRequest, DeleteBookedTicketResponse?>
    {
        private readonly AccelokaDbContext _db;
        private readonly ILogger<DeleteBookedTicketHandler> _logger;

        public DeleteBookedTicketHandler(AccelokaDbContext db, ILogger<DeleteBookedTicketHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<DeleteBookedTicketResponse?> Handle(DeleteBookedTicketRequest request, CancellationToken ct)
        {
            var bookingInfo = await (from bt in _db.BookingTickets
                                  join t in _db.Tickets on bt.TicketCode equals t.TicketCode
                                  join c in _db.Categories on t.CategoryId equals c.Id
                                  where bt.BookingId== request.BookedTicketId && t.TicketCode == request.TicketCode
                                  select new
                                  {
                                      bt,
                                      t.TicketCode,
                                      ticketName = t.Name,
                                      categoryName = c.Name,
                                  }).FirstOrDefaultAsync(ct);

            _logger.LogInformation($"Attempting to delete booked ticket: {bookingInfo}");

            if (bookingInfo is null)
            {
                throw new InvalidOperationException("Booked ticket not found.");
            }
            if( bookingInfo.bt.Quantity < request.Qty)
            {
                throw new InvalidOperationException("Insufficient quantity to delete.");
            }

            var quantityLeft = bookingInfo.bt.Quantity -= request.Qty;
            

            if (quantityLeft <= 0)
            {
                _db.BookingTickets.Remove(bookingInfo.bt);
            }

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation($"The booked Ticket quantity after revoked is : {quantityLeft}");

            return new DeleteBookedTicketResponse
            {
                TicketCode = bookingInfo.TicketCode,
                TicketName = bookingInfo.ticketName,
                TicketCategory = bookingInfo.categoryName,
                Quantity = quantityLeft
            };
        }
    }
}

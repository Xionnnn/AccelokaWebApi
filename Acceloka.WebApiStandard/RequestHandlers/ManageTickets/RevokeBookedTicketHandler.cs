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
    public class RevokeBookedTicketHandler : IRequestHandler<RevokeBookedTicketRequest, RevokeBookedTicketResponse?>
    {
        private readonly AccelokaDbContext _db;
        private readonly ILogger<RevokeBookedTicketHandler> _logger;

        public RevokeBookedTicketHandler(AccelokaDbContext db, ILogger<RevokeBookedTicketHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<RevokeBookedTicketResponse?> Handle(RevokeBookedTicketRequest request, CancellationToken ct)
        {
            var bookingInfo = await (from bt in _db.BookingTickets
                                  join t in _db.Tickets on bt.TicketCode equals t.TicketCode
                                  join c in _db.Categories on t.CategoryId equals c.Id
                                  where bt.BookingId== request.BookedTicketId && t.TicketCode == request.TicketCode
                                  select new
                                  {
                                      bt,
                                      t,
                                      categoryName = c.Name,
                                  }).FirstOrDefaultAsync(ct);

            _logger.LogInformation($"Attempting to delete booked ticket: {bookingInfo}");

            var quantityLeft = bookingInfo.bt.Quantity -= request.Qty;

            if (quantityLeft <= 0)
            {
                _db.BookingTickets.Remove(bookingInfo.bt);

                //Since the booking is split between booking and booking_ticket tables in the database, if all booking_tickets are deleted we need to delete the booking as well
                var hasBooking = await _db.BookingTickets
                    .Where(bt => bt.BookingId == request.BookedTicketId && bt.TicketCode != request.TicketCode)
                    .AnyAsync(ct);

                if (!hasBooking)
                {
                    var bookingToRemove = await _db.Bookings
                        .Where(b => b.Id == request.BookedTicketId)
                        .FirstOrDefaultAsync(ct);
                        
                    if(bookingToRemove != null)
                    {
                        _db.Bookings.Remove(bookingToRemove);
                    }
                }
            }

            //I assume that the quota of the revoked ticket will increase back
            bookingInfo.t.Quota += request.Qty;

            await _db.SaveChangesAsync(ct);

            _logger.LogInformation($"The booked Ticket quantity after revoked is : {quantityLeft}");

            return new RevokeBookedTicketResponse
            {
                TicketCode = bookingInfo.t.TicketCode,
                TicketName = bookingInfo.t.Name,
                TicketCategory = bookingInfo.categoryName,
                Quantity = quantityLeft
            };
        }
    }
}

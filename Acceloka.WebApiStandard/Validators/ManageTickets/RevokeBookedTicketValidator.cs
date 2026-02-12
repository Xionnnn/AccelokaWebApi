using Acceloka.WebApiStandard.Contracts.RequestModels.ManageTickets;
using Acceloka.WebApiStandard.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acceloka.WebApiStandard.Validators.ManageTickets
{
    public class RevokeBookedTicketValidator : AbstractValidator<RevokeBookedTicketRequest>
    {
        private readonly AccelokaDbContext _db;
        public RevokeBookedTicketValidator(AccelokaDbContext db)
        {
            _db = db;
            RuleFor(x => x.BookedTicketId)
                .NotEmpty().WithMessage("BookedTicketId cannot be emtpy.");

            RuleFor(x => x.TicketCode)
                .NotEmpty().WithMessage("TicketCode cannot be emtpy.");

            RuleFor(x => x.Qty)
                .NotEmpty().WithErrorCode("Quantity cannot be emtpy.")
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

            RuleFor(x => x).CustomAsync(ValidateBusinessAsync);
        }

        public async Task ValidateBusinessAsync(RevokeBookedTicketRequest req, ValidationContext<RevokeBookedTicketRequest> ctx, CancellationToken ct)
        {
            if (req.BookedTicketId <= 0 || string.IsNullOrWhiteSpace(req.TicketCode) || req.Qty <= 0)
            {
                return;
            }
            var bookingExists = await _db.Bookings.AnyAsync(b => b.Id == req.BookedTicketId, ct);
            if (!bookingExists)
            {
                ctx.AddFailure(nameof(RevokeBookedTicketRequest.BookedTicketId), $"Booked ticket {req.BookedTicketId} not found.");
                return;
            }
            var ticketExists = await _db.Tickets.AnyAsync(t => t.TicketCode == req.TicketCode, ct);
            if (!ticketExists)
            {
                ctx.AddFailure(nameof(RevokeBookedTicketRequest.TicketCode), $"Ticket {req.TicketCode} not found in database.");
                return;
            }
            var bookedTicket = await _db.BookingTickets
                .Where(bt => bt.BookingId == req.BookedTicketId && bt.TicketCode == req.TicketCode)
                .FirstOrDefaultAsync(ct);

            if (bookedTicket is null)
            {
                ctx.AddFailure(nameof(RevokeBookedTicketRequest.TicketCode), $"Ticket {req.TicketCode} Is not currenty booked in booking id {req.BookedTicketId}.");

                return;
            }

            if(req.Qty > bookedTicket.Quantity)
            {
                ctx.AddFailure(nameof(RevokeBookedTicketRequest.Qty), $"Quantity to revoke {req.Qty} exceeds the quantity currently booked {bookedTicket.Quantity}.");
                return;
            }
        }
    }
}

using Acceloka.WebApiStandard.Contracts.RequestModels.ManageTickets;
using Acceloka.WebApiStandard.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Acceloka.WebApiStandard.Validators.ManageTickets
{
    public sealed class EditBookedTicketValidator : AbstractValidator<EditBookedTicketRequest>
    {
        private readonly AccelokaDbContext _db;

        public EditBookedTicketValidator(AccelokaDbContext db)
        {
            _db = db;

            RuleFor(x => x.BookedTicketId)
                .GreaterThan(0).WithMessage("BookedTicketId must be greater than 0.");

            RuleFor(x => x.Items)
                .NotNull().WithMessage("Items is required.")
                .Must(items => items is { Count: > 0 }).WithMessage("Items cannot be empty.");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.TicketCode)
                    .NotEmpty().WithMessage("TicketCode cannot be empty.");

                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("Quantity minimal 1.");
            });

            RuleFor(x => x).CustomAsync(ValidateBusinessAsync);
        }

        private async Task ValidateBusinessAsync(EditBookedTicketRequest req, ValidationContext<EditBookedTicketRequest> ctx, CancellationToken ct)
        {
            if (req.BookedTicketId <= 0 || req.Items is null || req.Items.Count == 0)
            {
                return;
            }

            var groupedReq = req.Items
                .Where(i => !string.IsNullOrWhiteSpace(i.TicketCode))
                .GroupBy(i => i.TicketCode.Trim())
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

            if (groupedReq.Count == 0)
            {
                return;
            }

            var bookingExists = await _db.Bookings.AnyAsync(b => b.Id == req.BookedTicketId, ct);
            if (!bookingExists)
            {
                ctx.AddFailure(nameof(req.BookedTicketId), "BookedTicketId not found.");
                return;
            }

            var requestedCodes = groupedReq.Keys.ToList();

            var bookingInfo = await (
                from bt in _db.BookingTickets
                join t in _db.Tickets on bt.TicketCode equals t.TicketCode
                where bt.BookingId == req.BookedTicketId && requestedCodes.Contains(bt.TicketCode.ToUpper())
                select new
                {
                    TicketCode = bt.TicketCode,
                    BookedQuantity = bt.Quantity,
                    TicketQuota = t.Quota
                }
            ).ToListAsync(ct);

            var dbMap = bookingInfo
                .GroupBy(x => x.TicketCode.Trim())
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var (code, newQty) in groupedReq)
            {
                if (!dbMap.TryGetValue(code, out var db))
                {
                    ctx.AddFailure(nameof(req.Items), $"TicketCode not found in booked ticket: {code}");
                    continue;
                }

                var maxAllowed = db.BookedQuantity + db.TicketQuota;
                if (newQty > maxAllowed)
                {
                    ctx.AddFailure(nameof(req.Items), $"Quantity for {code} exceeds available quota.");
                }
            }
        }
    }

}

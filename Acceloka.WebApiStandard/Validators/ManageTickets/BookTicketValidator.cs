using Acceloka.WebApiStandard.Contracts.RequestModels.ManageTickets;
using Acceloka.WebApiStandard.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acceloka.WebApiStandard.Validators.ManageTickets
{
    public class BookTicketValidator : AbstractValidator<BookTicketRequest>
    {
        private readonly AccelokaDbContext _db;

        public BookTicketValidator(AccelokaDbContext db)
        {
            _db = db;


            RuleFor(x => x.Items)
                .NotNull().WithMessage("Items cannot be null.")
                .NotEmpty().WithMessage("Items cannot be empty.");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.TicketCode)
                    .NotEmpty().WithMessage("TicketCode cannot be empty.");

                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
            });

            RuleFor(x => x).CustomAsync(ValidateBusinessAsync);
        }

        private async Task ValidateBusinessAsync(
            BookTicketRequest req,
            ValidationContext<BookTicketRequest> ctx,
            CancellationToken ct)
        {
            if (req.Items is null || req.Items.Count == 0)
            {
                return;
            }

            var filteredRequest = req.Items
                .GroupBy(x => x.TicketCode)
                .Select(g => new { TicketCode = g.Key, Quantity = g.Sum(i => i.Quantity) })
                .ToList();

            var reqCodes = filteredRequest.Select(x => x.TicketCode).ToList();
            if (reqCodes.Count == 0)
            {
                return;
            }

            var qtyByCode = filteredRequest.ToDictionary(x => x.TicketCode, x => x.Quantity);

            var tickets = await _db.Tickets
                .AsNoTracking()
                .Where(t => reqCodes.Contains(t.TicketCode))
                .Select(t => new { t.TicketCode, t.Quota, t.EventDate })
                .ToListAsync(ct);

            var foundCodes = tickets.Select(t => t.TicketCode).ToHashSet();

            foreach (var code in reqCodes.Where(c => !foundCodes.Contains(c)))
            {
                ctx.AddFailure(nameof(BookTicketRequest.Items), $"Ticket not found: {code}");
                return;
            }


            var ticketByCode = tickets.ToDictionary(t => t.TicketCode);

            foreach (var code in reqCodes)
            {
                var ticket = ticketByCode[code];
                var requestedQty = qtyByCode[code];

                //This validation can be used to validate tickets that are sold out as well as requests that exceed the ticket quota
                if (ticket.Quota < requestedQty)
                {
                    ctx.AddFailure(nameof(BookTicketRequest.Items), $"Not enough quota: {code}");
                }

                if (ticket.EventDate <= req.BookingDate)
                {
                    ctx.AddFailure(nameof(BookTicketRequest.BookingDate), $"Event date must be after booking date: {code}");
                }
            }
        }
    }
}

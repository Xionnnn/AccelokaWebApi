using Acceloka.WebApiStandard.Contracts.RequestModels.ManageTickets;
using Acceloka.WebApiStandard.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acceloka.WebApiStandard.Validators.ManageTickets
{
    public class ViewTicketDetailValidator : AbstractValidator<ViewTicketDetailRequest>
    {
        private readonly AccelokaDbContext _db;
        public ViewTicketDetailValidator(AccelokaDbContext db)
        {
            _db = db;
            RuleFor(x => x.BookedTicketId)
                .NotEmpty().WithMessage("BookedTicketId cannot be emtpy.")
                .DependentRules(() =>
                {
                    RuleFor(x => x).CustomAsync(ValidateBusinessAsync);
                });

            
        }

        public async Task ValidateBusinessAsync(ViewTicketDetailRequest req, ValidationContext<ViewTicketDetailRequest> ctx, CancellationToken ct)
        {
            var bookedTicket = await _db.BookingTickets
                .Where(x => x.BookingId == req.BookedTicketId)
                .AnyAsync(ct);

            if (!bookedTicket)
            {
                ctx.AddFailure("BookedTicketId", $"BookedTicketId {req.BookedTicketId} does not exist.");
            }
        }
    }
}

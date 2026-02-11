using Acceloka.WebApiStandard.Contracts.RequestModels.ManageTickets;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acceloka.WebApiStandard.Validators.ManageTickets
{
    public class DeleteBookedTicketValidator : AbstractValidator<DeleteBookedTicketRequest>
    {
        public DeleteBookedTicketValidator()
        {
            RuleFor(x => x.BookedTicketId)
                .NotEmpty().WithMessage("BookedTicketId cannot be emtpy.");

            RuleFor(x => x.TicketCode)
                .NotEmpty().WithMessage("TicketCode cannot be emtpy.");

            RuleFor(x => x.Qty)
                .NotEmpty().WithErrorCode("Quantity cannot be emtpy.")
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        }
    }
}

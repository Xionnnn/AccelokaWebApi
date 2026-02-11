using Acceloka.WebApiStandard.Contracts.RequestModels.ManageTickets;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acceloka.WebApiStandard.Validators.ManageTickets
{
    public class BookTicketValidator : AbstractValidator<BookTicketRequest>
    {
        public BookTicketValidator()
        {
            RuleFor(x => x.Items)
                .NotNull().WithMessage("Items is required.")
                .NotEmpty().WithMessage("At least one ticket must be booked.");

        }
    }
}

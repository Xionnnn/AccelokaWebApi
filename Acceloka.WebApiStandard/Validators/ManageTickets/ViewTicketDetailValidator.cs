using Acceloka.WebApiStandard.Contracts.RequestModels.ManageTickets;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acceloka.WebApiStandard.Validators.ManageTickets
{
    public class ViewTicketDetailValidator : AbstractValidator<ViewTicketDetailRequest>
    {
        public ViewTicketDetailValidator()
        {
            RuleFor(x=>x.BookedTicketId)
                .NotEmpty().WithMessage("BookingId cannot be empty.");
        }
    }
}

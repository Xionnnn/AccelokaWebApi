using Acceloka.WebApiStandard.Contracts.RequestModels.ManageTickets;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acceloka.WebApiStandard.Validators.ManageTickets
{
    public class GetAvailableTicketValidator : AbstractValidator<GetAvailableTicketRequest>
    {
        private static readonly string[] AllowedOrderStates = ["asc", "desc"];

        private static readonly string[] AllowedOrderBy =
        [
            "CategoryName",
            "TicketCode",
            "TicketName",
            "EventDate",
            "Price",
            "Quota"
        ];

        public GetAvailableTicketValidator()
        {
            RuleFor(x => x.CategoryName)
                .Must(v => v == string.Empty || !string.IsNullOrWhiteSpace(v))
                .WithMessage("CategoryName cannot contain only whitespace characters.");

            RuleFor(x => x.TicketCode)
                .Must(v => v == string.Empty || !string.IsNullOrWhiteSpace(v))
                .WithMessage("TicketCode cannot contain only whitespace characters.");

            RuleFor(x => x.TicketName)
                .Must(v => v == string.Empty || !string.IsNullOrWhiteSpace(v))
                .WithMessage("TicketName cannot contain only whitespace characters.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Price.HasValue)
                .WithMessage("Price have be more than 0.");

            RuleFor(x => x)
                .Must(x =>
                    !x.MinimalEventDate.HasValue ||
                    !x.MaximalEventDate.HasValue ||
                    x.MinimalEventDate.Value <= x.MaximalEventDate.Value)
                .WithMessage("MinimalEventDate have to be <= MaximalEventDate and both have to exist.");

            RuleFor(x => x.OrderState)
                .Must(v => string.IsNullOrWhiteSpace(v) || AllowedOrderStates.Contains(v.Trim().ToLowerInvariant()))
                .WithMessage($"OrderState can only be: {string.Join(", ", AllowedOrderStates)}.");

            RuleFor(x => x.OrderBy)
                .Must(v => string.IsNullOrWhiteSpace(v) || AllowedOrderBy.Contains(v.Trim()))
                .WithMessage($"OrderBy can only be: {string.Join(", ", AllowedOrderBy)}.");
        }
    }
}

using Acceloka.WebApiStandard.Contracts.RequestModels.ManageTickets;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acceloka.WebApiStandard.Validators.ManageTickets
{
    public class GetBookingValidator: AbstractValidator<GetBookingRequest>
    {
        private static readonly string[] AllowedOrderStates = ["asc", "desc"];

        private static readonly string[] AllowedOrderBy =
        [
            "BookingId",
            "BookingQuantity",
            "BookingDate",
            "BookingPrice",
        ];

        public GetBookingValidator()
        {
            RuleFor(x => x.BookingId)
                  .GreaterThanOrEqualTo(0)
                  .When(x=> x.BookingId.HasValue)
                  .WithMessage("BookingId can't be less than 0.");

            RuleFor(x => x.BookingPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.BookingPrice.HasValue)
                .WithMessage("BookingPrice can't be less than 0.");

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

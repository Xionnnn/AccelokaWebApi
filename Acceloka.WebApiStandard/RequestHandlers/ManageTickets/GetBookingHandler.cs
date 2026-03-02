using Acceloka.WebApiStandard.Contracts.RequestModels.ManageTickets;
using Acceloka.WebApiStandard.Contracts.ResponseModels.ManageTickets;
using Acceloka.WebApiStandard.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Acceloka.WebApiStandard.RequestHandlers.ManageTickets
{
    public class GetBookingHandler : IRequestHandler<GetBookingRequest, GetBookingResponse>
    {
        private readonly AccelokaDbContext _db;
        private readonly ILogger<GetBookingHandler> _logger;

        public GetBookingHandler(AccelokaDbContext db, ILogger<GetBookingHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<GetBookingResponse> Handle(GetBookingRequest request, CancellationToken ct)
        {
            var query = _db.Bookings.AsNoTracking();

            if (request.BookingId > 0 && request.BookingId.HasValue)
            {
                query = query.Where(x => x.Id == request.BookingId);
            }
            if (request.MinimalEventDate.HasValue)
            {
                var startUtc = DateTime.SpecifyKind(request.MinimalEventDate.Value.Date, DateTimeKind.Utc);
                query = query.Where(t => t.BookingDate >= startUtc);
            }

            if (request.MaximalEventDate.HasValue)
            {
                var endExclusiveUtc = DateTime.SpecifyKind(request.MaximalEventDate.Value.Date.AddDays(1), DateTimeKind.Utc);
                query = query.Where(t => t.BookingDate <= endExclusiveUtc);
            }

            if (request.BookingPrice.HasValue)
            {
               query = query.Where(t => t.BookingPrice <= request.BookingPrice.Value);
            }

            query = ApplySorting(query, request.OrderBy, request.OrderState);

            var totalItems = await query.CountAsync(ct);

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(t => new BookingDto
                {
                    BookingId = t.Id,
                    BookingQuantity = t.BookingQuantity,
                    BookingPrice = t.BookingPrice,
                    BookingDate = t.BookingDate
                })
                .ToListAsync(ct);

            var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

            return new GetBookingResponse
            {
                Bookings = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalTickets = totalItems,
                TotalPages = totalPages
            };
        }

        private IQueryable<Booking> ApplySorting(
            IQueryable<Booking> query,
            string orderBy,
            string orderState)
        {
            var isDescending = orderState.Equals("desc", StringComparison.OrdinalIgnoreCase);

            return orderBy switch
            {
                "BookingId" => isDescending
                    ? query.OrderByDescending(t => t.Id)
                    : query.OrderBy(t => t.Id),

                "BookingQuantity" => isDescending
                    ? query.OrderByDescending(t => t.BookingQuantity)
                    : query.OrderBy(t => t.BookingQuantity),

                "BookingPrice" => isDescending
                    ? query.OrderByDescending(t => t.BookingPrice)
                    : query.OrderBy(t => t.BookingPrice),

                "BookingDate" => isDescending
                    ? query.OrderByDescending(t => t.BookingDate)
                    : query.OrderBy(t => t.BookingDate),
                _ => query.OrderBy(t => t.Id)
            };
        }


    }
}

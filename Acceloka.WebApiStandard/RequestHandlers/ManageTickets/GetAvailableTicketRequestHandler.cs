using Acceloka.WebApiStandard.Contracts.RequestModels.ManageTickets;
using Acceloka.WebApiStandard.Contracts.ResponseModels.ManageTickets;
using Acceloka.WebApiStandard.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acceloka.WebApiStandard.RequestHandlers.ManageTickets
{
    public class GetAvailableTicketRequestHandler : IRequestHandler<GetAvailableTicketRequest, GetAvailableTicketResponse?>
    {
        private readonly AccelokaDbContext _db;
        private readonly ILogger<GetAvailableTicketRequestHandler> _logger;
        public GetAvailableTicketRequestHandler(AccelokaDbContext db, ILogger<GetAvailableTicketRequestHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<GetAvailableTicketResponse?> Handle(GetAvailableTicketRequest request, CancellationToken ct)
        {
            _logger.LogInformation("Getting available tickets with filters: CategoryName={CategoryName}, TicketCode={TicketCode}", request.CategoryName, request.TicketCode);

            var query = _db.Tickets
               .Include(t => t.Category)
               .Where(t => t.Quota > 0)
               .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.CategoryName))
            {
                query = query.Where(t => t.Category.Name.Contains(request.CategoryName));
            }

            if (!string.IsNullOrWhiteSpace(request.TicketCode))
            {
                query = query.Where(t => t.TicketCode.Contains(request.TicketCode));
            }

            if (!string.IsNullOrWhiteSpace(request.TicketName))
            {
                query = query.Where(t => t.Name.Contains(request.TicketName));
            }

            if (request.Price.HasValue)
            {
                query = query.Where(t => t.Price <= request.Price.Value);
            }

            if (request.MinimalEventDate.HasValue)
            {
                query = query.Where(t => t.EventDate >= request.MinimalEventDate.Value);
            }

            if (request.MaximalEventDate.HasValue)
            {
                query = query.Where(t => t.EventDate <= request.MaximalEventDate.Value);
            }

            query = ApplySorting(query, request.OrderBy, request.OrderState);

            var totalItems = await query.CountAsync(ct);

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(t => new AvailableTicketDto
                {
                    CategoryName = t.Category.Name,
                    TicketCode = t.TicketCode,
                    TicketName = t.Name,
                    EventDate = t.EventDate,
                    Price = t.Price,
                    Quota = t.Quota
                })
                .ToListAsync(ct);

            _logger.LogInformation("Found {Count} tickets out of {Total} total", items.Count, totalItems);

            var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

            return new GetAvailableTicketResponse
            {
                Tickets = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

        }

        private IQueryable<Ticket> ApplySorting(
            IQueryable<Ticket> query,
            string orderBy,
            string orderState)
        {
            var isDescending = orderState.Equals("desc", StringComparison.OrdinalIgnoreCase);

            return orderBy switch
            {
                "CategoryName" => isDescending
                    ? query.OrderByDescending(t => t.Category.Name)
                    : query.OrderBy(t => t.Category.Name),

                "TicketCode" => isDescending
                    ? query.OrderByDescending(t => t.TicketCode)
                    : query.OrderBy(t => t.TicketCode),

                "TicketName" => isDescending
                    ? query.OrderByDescending(t => t.Name)
                    : query.OrderBy(t => t.Name),

                "EventDate" => isDescending
                    ? query.OrderByDescending(t => t.EventDate)
                    : query.OrderBy(t => t.EventDate),

                "Price" => isDescending
                    ? query.OrderByDescending(t => t.Price)
                    : query.OrderBy(t => t.Price),

                "Quota" => isDescending
                    ? query.OrderByDescending(t => t.Quota)
                    : query.OrderBy(t => t.Quota),

                // The default ordering is TicketCode ascending
                _ => query.OrderBy(t => t.TicketCode)
            };
        }

    }
}

    using Acceloka.WebApiStandard.Contracts.RequestModels.ManageTickets;
    using Acceloka.WebApiStandard.Contracts.ResponseModels.ManageTickets;
    using Acceloka.WebApiStandard.Entities;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Text;

    namespace Acceloka.WebApiStandard.RequestHandlers.ManageTickets
    {
        public class ViewTicketDetailHandler : IRequestHandler<ViewTicketDetailRequest, IReadOnlyList<ViewTicketDetailResponse>>
        {
            private readonly AccelokaDbContext _db;
            private readonly ILogger<ViewTicketDetailHandler> _logger;

            public ViewTicketDetailHandler(
                AccelokaDbContext db,
                ILogger<ViewTicketDetailHandler> logger)
            {
                _db = db;
                _logger = logger;
            }


            public async Task<IReadOnlyList<ViewTicketDetailResponse>> Handle(ViewTicketDetailRequest request, CancellationToken ct)
            {
                var tickets = await (
                    from b in _db.Bookings
                    join bt in _db.BookingTickets
                    on b.Id equals bt.BookingId
                    join t in _db.Tickets
                    on bt.TicketCode equals t.TicketCode
                    join c in _db.Categories
                    on t.CategoryId equals c.Id
                    where b.Id == request.BookedTicketId
                    select new
                    {
                        t.TicketCode,
                        ticketName = t.Name,
                        t.EventDate,
                        c.Id,
                        categoryName = c.Name
                    }
                    ).ToListAsync(ct);


                var results = tickets
                    .GroupBy(x => new { x.Id, x.categoryName })
                    .Select(g => new ViewTicketDetailResponse
                    {
                        CategoryName = g.Key.categoryName,
                        QtyPerCategory = g.Count(),
                        Tickets = g.Select(t => new DetailTicketDto
                        {
                            TicketCode = t.TicketCode,
                            TicketName = t.ticketName,
                            EventDate = t.EventDate
                        }).ToList()
                    }).ToList();

                return results;
            }
        }
    }

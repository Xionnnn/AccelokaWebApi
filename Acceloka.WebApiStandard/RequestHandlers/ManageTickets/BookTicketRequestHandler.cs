using Acceloka.WebApiStandard.Contracts.RequestModels.ManageTickets;
using Acceloka.WebApiStandard.Contracts.ResponseModels.ManageTickets;
using Acceloka.WebApiStandard.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Acceloka.WebApiStandard.RequestHandlers.ManageTickets
{
    public class BookTicketRequestHandler : IRequestHandler<BookTicketRequest, BookTicketResponse?>
    {
        private readonly AccelokaDbContext _db;
        private readonly ILogger<BookTicketRequestHandler> _logger;

        public BookTicketRequestHandler(
            AccelokaDbContext db,
            ILogger<BookTicketRequestHandler> logger
            )
        {
            _db = db;
            _logger = logger;
        }

        public async Task<BookTicketResponse?> Handle(BookTicketRequest request, CancellationToken ct)
        {
           _logger.LogInformation($"BookTicketRequest{request}");

            var filteredRequest = request.Items
                .GroupBy(x => x.TicketCode)
                .Select(g => new
                {
                    TicketCode = g.Key,
                    Quantity = g.Sum(i => i.Quantity)
                })
                .ToList();

            var reqCodes = filteredRequest.Select(x => x.TicketCode).ToList();
            var qtyByCode = filteredRequest.ToDictionary(x => x.TicketCode, x => x.Quantity);

            var tickets = await _db.Tickets
                .Where(x => reqCodes.Contains(x.TicketCode))
                .ToListAsync(ct);

            _logger.LogInformation($"Found {tickets.Count} tickets.");

            var foundCodes = tickets.Select(t => t.TicketCode).ToHashSet();
            var missingCodes = reqCodes.Where(c => !foundCodes.Contains(c)).ToList();
            if (missingCodes.Count > 0)
            {
                throw new InvalidOperationException($"Ticket not found: {string.Join(", ", missingCodes)}");
            }

            var ticketByCode = tickets.ToDictionary(x => x.TicketCode);

            var bookedTickets = new List<BookingTicket>(filteredRequest.Count);

            foreach (var item in filteredRequest)
            {
                var ticket = ticketByCode[item.TicketCode];

                if (ticket.Quota < item.Quantity)
                {
                    throw new InvalidOperationException($"Not enough quota: {item.TicketCode}");
                }

                if (ticket.EventDate <= request.bookingDate)
                {
                    throw new InvalidOperationException($"Event date must be after booking date: {item.TicketCode}");
                }

                ticket.Quota -= item.Quantity;

                bookedTickets.Add(new BookingTicket
                {
                    TicketCode = item.TicketCode,
                    Quantity = item.Quantity
                });
            }

            var booking = new Booking
            {
                BookingDate = request.bookingDate,
                BookingTickets = bookedTickets
            };

            _db.Add(booking);
            await _db.SaveChangesAsync(ct);

            var categoryIds = tickets.Select(t => t.CategoryId).Distinct().ToList();

            var categories = await _db.Categories
                .Where(c => categoryIds.Contains(c.Id))
                .ToListAsync(ct);

            var categoryNameById = categories.ToDictionary(c => c.Id, c => c.Name);

            var categoriesDto = tickets
                .Where(t => t.CategoryId.HasValue)
                .GroupBy(t => t.CategoryId!.Value)
                .Select(g => new categoryDto
                {
                    CategoryName = categoryNameById[g.Key],
                    SummaryPrice = g.Sum(t => t.Price * qtyByCode[t.TicketCode]),
                    Tickets = g.Select(t => new ticketDto
                    {
                        TicketCode = t.TicketCode,
                        TicketName = t.Name,
                        Price = t.Price
                    }).ToList()
                })
                .ToList();

            return new BookTicketResponse
            {
                PriceSummary = categoriesDto.Sum(x => x.SummaryPrice),
                TicketsPerCategories = categoriesDto
            };
        }
    }
}

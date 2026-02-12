using Acceloka.WebApiStandard.Contracts.RequestModels.ManageTickets;
using Acceloka.WebApiStandard.Contracts.ResponseModels.ManageTickets;
using Acceloka.WebApiStandard.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Acceloka.WebApiStandard.RequestHandlers.ManageTickets
{
    public class BookTicketHandler : IRequestHandler<BookTicketRequest, BookTicketResponse?>
    {
        private readonly AccelokaDbContext _db;
        private readonly ILogger<BookTicketHandler> _logger;

        public BookTicketHandler(
            AccelokaDbContext db,
            ILogger<BookTicketHandler> logger
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


            var ticketByCode = tickets.ToDictionary(x => x.TicketCode);

            var bookedTickets = new List<BookingTicket>(filteredRequest.Count);

            foreach (var item in filteredRequest)
            {
                var ticket = ticketByCode[item.TicketCode];

                ticket.Quota -= item.Quantity;

                bookedTickets.Add(new BookingTicket
                {
                    TicketCode = item.TicketCode,
                    Quantity = item.Quantity
                });
            }

            var booking = new Booking
            {
                BookingDate = request.BookingDate,
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
                .Select(g => new CategoryDto
                {
                    CategoryName = categoryNameById[g.Key],
                    SummaryPrice = g.Sum(t => t.Price * qtyByCode[t.TicketCode]),
                    Tickets = g.Select(t => new TicketDto
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

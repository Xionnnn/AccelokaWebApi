namespace Acceloka.WebApiStandard.Contracts.ResponseModels.ManageTickets
{
    public class BookTicketResponse
    {
        public decimal PriceSummary { get; set; }
        public IReadOnlyList<categoryDto> TicketsPerCategories { get; set; } = Array.Empty<categoryDto>();

    }

    public class categoryDto
    {
        public string CategoryName { get; set; } = string.Empty;

        public decimal SummaryPrice { get; set; }
        public IReadOnlyList<ticketDto> Tickets { get; set; } = Array.Empty<ticketDto>();
    }

    public class ticketDto
    {
        public string TicketCode { get; set; } = string.Empty;
        public string TicketName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}

namespace Acceloka.WebApiStandard.Contracts.ResponseModels.ManageTickets
{
    public class BookTicketResponse
    {
        public decimal PriceSummary { get; set; }
        public IReadOnlyList<CategoryDto> TicketsPerCategories { get; set; } = Array.Empty<CategoryDto>();

    }

    public class CategoryDto
    {
        public string CategoryName { get; set; } = string.Empty;

        public decimal SummaryPrice { get; set; }
        public IReadOnlyList<TicketDto> Tickets { get; set; } = Array.Empty<TicketDto>();
    }

    public class TicketDto
    {
        public string TicketCode { get; set; } = string.Empty;
        public string TicketName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}

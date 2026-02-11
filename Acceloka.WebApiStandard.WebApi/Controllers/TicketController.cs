using Acceloka.WebApiStandard.Contracts.RequestModels.ManageTickets;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Acceloka.WebApiStandard.WebApi.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class TicketController : ControllerBase
    {

        private readonly IMediator _mediator;

        public TicketController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("get-available-ticket")]
        public async Task<IActionResult> GetAvailable([FromQuery] GetAvailableTicketRequest request, CancellationToken ct)
        {
            var response = await _mediator.Send(request, ct);
            return Ok(response);
        }


        [HttpPost("book-ticket")]
        public async Task<IActionResult> BookTicket([FromBody] BookTicketRequest request, CancellationToken ct)
        {
            var response = await _mediator.Send(request, ct);
            return Ok(response);
        }

        [HttpGet("get-booked-ticket/{BookedTicketId}")]
        public async Task<IActionResult> ViewBookingDetailById([FromRoute] ViewTicketDetailRequest request, CancellationToken ct)
        {
            var response = await _mediator.Send(request, ct);
            return Ok(response);
        }


        [HttpDelete("revoke-ticket/{BookedTicketId}/{TicketCode}/{Qty}")]
        public async Task<IActionResult> revokeTicketById([FromRoute] DeleteBookedTicketRequest request, CancellationToken ct)
        {
            var response = await _mediator.Send(request, ct);
            return Ok(response);
        }
    }
}

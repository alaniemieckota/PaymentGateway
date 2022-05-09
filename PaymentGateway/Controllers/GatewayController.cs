using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.Authorize;
using PaymentGateway.Application.Capture;
using PaymentGateway.Application.Refund;
using PaymentGateway.Application.Void;
using PaymentGateway.Services;
using PaymentGateway.Validators;

namespace PaymentGateway.Controllers
{
    [ApiController]
    public class GatewayController : Controller
    {
        private readonly IMediator mediator;
        private readonly IIdempotencyService idempotencyService;

        public GatewayController(IMediator mediator, IIdempotencyService idempotencyService)
        {
            this.mediator = mediator;
            this.idempotencyService = idempotencyService;
        }

        // TODO: remove this method
        [HttpGet("/validate")]
        public bool ValidateCCNumber(string creditCardNumber)
        {
            return new LuhnValidator().IsValid(creditCardNumber);
        }

        [HttpPost("/authorize")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorizeResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AuthorizeResponse>> Authorize(
            [FromBody]AuthorizeRequest request,
            [FromHeader(Name = "Idempotency-Key")] string idempotencyKey
            )
        {
            // Idempotency check can be also done by middleware so controller method does not have to worry about it
            await this.idempotencyService.CheckUniqness(idempotencyKey); 

            var response = await mediator.Send(request);
            
            return Ok(response); // TODO: response logic
        }

        [HttpPost("/capture")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CaptureResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<CaptureResponse>> Capture(
            [FromBody] CaptureRequest request,
            [FromHeader(Name = "Idempotency-Key")] string idempotencyKey
            )
        {
            // Idempotency check can be also done by middleware so controller method does not have to worry about it
            await this.idempotencyService.CheckUniqness(idempotencyKey);

            var response = await mediator.Send(request);

            return Ok(response); // TODO: response logic
        }

        [HttpPut("/void/{authorizationId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CaptureResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<VoidResponse>> Void(
            [FromRoute] string authorizationId,
            [FromHeader(Name = "Idempotency-Key")] string idempotencyKey
            )
        {
            // Idempotency check can be also done by middleware so controller method does not have to worry about it
            await this.idempotencyService.CheckUniqness(idempotencyKey);

            var response = await mediator.Send(new VoidRequest(authorizationId));
            
            return Ok(response);
        }

        [HttpPost("/refund")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RefundResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<RefundResponse>> Refund(
            [FromBody] RefundRequest request,
            [FromHeader(Name = "Idempotency-Key")] string idempotencyKey
            )
        {
            // Idempotency check can be also done by middleware so controller method does not have to worry about it
            await this.idempotencyService.CheckUniqness(idempotencyKey);

            var response = await mediator.Send(request);

            return Ok(response);
        }
    }
}
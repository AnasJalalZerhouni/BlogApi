using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Features.Users
{
    [ApiController]
    [Route("user")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator mediator;

        public UsersController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("registre")]
        public Task<UserEnvelope> Create([FromBody] Create.Command command,
            CancellationToken cancellationToken)
        {
            return mediator.Send(command, cancellationToken);
        }

        [HttpPost("login")]
        public Task<UserEnvelope> Login([FromBody] Login.Command command,
            CancellationToken cancellationToken)
        {
            return mediator.Send(command, cancellationToken);
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Features.Users
{

    [Route("api/user")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator mediator;

        public UsersController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("registre")]
        public async Task<UserEnvelope> Create([FromBody] Create.Command command,
            CancellationToken cancellationToken)
        {
            return await mediator.Send(command, cancellationToken);
        }

        [HttpPost("login")]
        public Task<UserEnvelope> Login([FromBody] Login.Command command,
            CancellationToken cancellationToken)
        {
            return mediator.Send(command, cancellationToken);
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Features.Users
{
    [ApiController]
    [Route("Users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator mediator;

        public UsersController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public Task<UserEnvelope> Create([FromBody] Create.Command command,
            CancellationToken cancellationToken)
        {
            return mediator.Send(command, cancellationToken);
        }
    }
}

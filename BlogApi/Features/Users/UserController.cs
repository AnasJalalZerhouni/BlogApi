using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Features.Users
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IMediator mediator;

        public UserController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public Task<UserEnvelope> GetCurrent(CancellationToken cancellationToken)
        {
            return mediator.Send(new Details.Query("<Unknown>"),cancellationToken);
        }

        [HttpPut]
        public Task<UserEnvelope> UpdateUser([FromBody] Edit.Command command,
            CancellationToken cancellationToken)
        {
            return mediator.Send(command,cancellationToken);
        }

    }
}

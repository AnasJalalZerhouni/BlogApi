using BlogApi.Infrastructure;
using BlogApi.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Features.Users
{
    [ApiController]
    [Route("user")]
    [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
    public class UserController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly ICurrentUserAccessor currentUser;

        public UserController(IMediator mediator, ICurrentUserAccessor currentUser)
        {
            this.mediator = mediator;
            this.currentUser = currentUser;
        }

        [HttpGet]
        public Task<UserEnvelope> GetCurrent(CancellationToken cancellationToken)
        {
            return mediator.Send(new Details.Query(currentUser.GetCurrentUserId() ?? "<Unknown>"),cancellationToken);
        }

        [HttpPut]
        public Task<UserEnvelope> UpdateUser([FromBody] Edit.Command command,
            CancellationToken cancellationToken)
        {
            return mediator.Send(command,cancellationToken);
        }

    }
}

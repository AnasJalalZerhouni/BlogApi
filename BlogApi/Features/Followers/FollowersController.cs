using BlogApi.Features.Profiles;
using BlogApi.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Features.Followers
{

    [Route("api/profiles")]
    public class FollowersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FollowersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{username}/follow")]
        [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
        public Task<ProfileEnvolop> Follow(string username,CancellationToken cancellationToken)
        {
            return _mediator.Send(new Add.Command(username),cancellationToken);
        }

        [HttpDelete("{username}/unfollow")]
        [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
        public Task<ProfileEnvolop> Unfollow(string username, CancellationToken cancellationToken)
        {
            return _mediator.Send(new Delete.Command(username), cancellationToken);
        }
    }
}

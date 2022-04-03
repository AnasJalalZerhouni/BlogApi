using BlogApi.Features.Profiles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Features.Followers
{
    [ApiController]
    [Route("/api/profiles")]
    public class FollowersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FollowersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{username}/follow")]
        public Task<ProfileEnvolop> Follow(string username,CancellationToken cancellationToken)
        {
            return _mediator.Send(new Add.Command(username),cancellationToken);
        }

        [HttpDelete("{username}/unfollow")]
        public Task<ProfileEnvolop> Unfollow(string username, CancellationToken cancellationToken)
        {
            return _mediator.Send(new Delete.Command(username), cancellationToken);
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Features.Profiles
{
    [ApiController]
    [Route("/api/profiles")]
    public class ProfilesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProfilesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{username}")]
        public Task<ProfileEnvolop> Get(string username,CancellationToken cancellationToken)
        {
            return _mediator.Send(new Details.Query(username),cancellationToken);
        }

    }
}

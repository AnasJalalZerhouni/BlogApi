using BlogApi.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Features.Tags
{
    [ApiController]
    [Route("/api/Tags")]
    public class TagsController : ControllerBase
    {
        private readonly IMediator mediator;
        public TagsController(IMediator mediator)
        {
            this.mediator= mediator;
        }
        [HttpGet]
        public Task<TagsEnvolop> Get(CancellationToken cancellationToken)
        {
            return mediator.Send(new List.Query(),cancellationToken);
        }
    }
}

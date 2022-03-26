using BlogApi.Domain;
using BlogApi.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Features.Articles
{
    [ApiController]
    [Route("/api/Articles")]
    public class ArticlesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ArticlesController(IMediator _mediator)
        {
            this._mediator = _mediator;
        }

        [HttpGet]
        public Task<ArticlesEnvelope> Get([FromQuery] string? tag,
                [FromQuery] string? author,
                [FromQuery] string? favorited,
                [FromQuery] int? limit,
                [FromQuery] int? offset,
                CancellationToken cancellationToken)
        {
            return _mediator.Send(new List.Query(tag, author, favorited, limit, offset));
        }

        [HttpGet("Feed")]
        public Task<ArticlesEnvelope> GetFeed([FromQuery] string? tag,
                [FromQuery] string? author,
                [FromQuery] string? favorited,
                [FromQuery] int? limit,
                [FromQuery] int? offset,
                CancellationToken cancellationToken)
        {
            return _mediator.Send(new List.Query(tag, author, favorited, limit, offset,true));
        }

        [HttpGet("{slug}")]
        public Task<ArticleEnvelope> Get(string slug,CancellationToken cancellationToken)
        {
            return _mediator.Send(new Details.Query(slug),cancellationToken);

        }

        [HttpPost]
        public Task<ArticleEnvelope> Create([FromBody] Create.Command command,
            CancellationToken cancellationToken)
        {
            return _mediator.Send(command,cancellationToken);
        }


        [HttpPut("{slug}")]
        public Task<ArticleEnvelope> Edit(string slug, [FromBody] Edit.ArticleData model, CancellationToken cancellationToken)
        {
            return _mediator.Send(new Edit.Command(model, slug), cancellationToken);
        }

        [HttpDelete("{slug}")]
        public Task Delete(string slug ,CancellationToken cancellationToken)
        {
            return _mediator.Send(new Delete.Command(slug), cancellationToken);
        }
    }
}

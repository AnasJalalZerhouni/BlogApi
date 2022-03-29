
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Features.Favorites
{
    [ApiController]
    [Route("/api/Favorites")]
    public class FavoritesController : ControllerBase
    {

        private readonly IMediator _mediator;
        public FavoritesController(IMediator _mediator)
        {
            this._mediator = _mediator;
        }

        [HttpPost("{slug}/favorites")]
        public Task<Articles.ArticleEnvelope> FavoritesAdd(string slug,CancellationToken token)
        {
            return _mediator.Send(new Create.Command(slug), token);
        }

        [HttpDelete("{slug}/favorites")]
        public Task FavoritesDelete(string slug, CancellationToken token)
        {
            return _mediator.Send(new Delete.Command(slug), token);
        }
    }
}

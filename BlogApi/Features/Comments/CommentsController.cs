using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Features.Comments
{
    [ApiController]
    [Route("/articles")]
    public class CommentsController : ControllerBase
    {
        private readonly IMediator mediatr;

        public CommentsController(IMediator mediatr)
        {
            this.mediatr = mediatr;
        }

        [HttpGet("{slug}/comments")]
        public  Task<CommentsEnvolop> Get(string slug, CancellationToken cancellationtoken)
        {
            return mediatr.Send(new List.Query(slug),cancellationtoken);
        }
    }
}

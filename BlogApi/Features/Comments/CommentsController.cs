using BlogApi.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BlogApi.Features.Comments
{

    [Route("api/articles")]
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

        [HttpPost("{slug}/comments")]
        [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
        public Task<CommentEnvolop> Create(string slug,[FromBody] Create.CommentData data,CancellationToken cancellationToken)
        {
            return mediatr.Send(new Create.Command(slug, data),cancellationToken);
        }

        [HttpDelete("{slug}/comments/{id}")]
        [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
        public Task Delete(string slug,int id, CancellationToken cancellationToken)
        {
            return mediatr.Send(new Delete.Command(slug, id), cancellationToken);
        }
    }
}

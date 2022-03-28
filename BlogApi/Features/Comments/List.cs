using BlogApi.Infrastructure;
using BlogApi.Infrastructure.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BlogApi.Features.Comments
{
    public class List
    {
        public record Query(string Slug):IRequest<CommentsEnvolop>;

        public class QueryHandler : IRequestHandler<Query, CommentsEnvolop>
        {
            private readonly BlogContext context;
            public QueryHandler(BlogContext context)
            {
                this.context = context;
            }

            public async Task<CommentsEnvolop> Handle(Query request, CancellationToken cancellationToken)
            {
                var Article = await context.Articles
                        .Include(x=>x.Comments)
                        .ThenInclude(x=>x.Author)
                        .FirstOrDefaultAsync(x => x.Slug == request.Slug,cancellationToken);

                if (Article ==null)
                {
                    throw new RestException(HttpStatusCode.NotFound,new {Article=Constants.NOT_FOUND});
                }

                return new CommentsEnvolop(Article.Comments);
            }
        }
    }
}

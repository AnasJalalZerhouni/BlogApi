using BlogApi.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Features.Tags
{
    public class List
    {
        public record Query():IRequest<TagsEnvolop>;

        public class QueryHandler : IRequestHandler<Query, TagsEnvolop>
        {
            private readonly BlogContext context;
            public QueryHandler(BlogContext context)
            {
                this.context = context;
            }

            public async Task<TagsEnvolop> Handle(Query message, CancellationToken cancellationToken)
            {
                var tags = await context.Tags
                    .OrderBy(x=>x.TagId)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return new TagsEnvolop
                {
                    Tags=tags?.Select(x=>x.TagId ?? String.Empty).ToList() ?? new List<string>()
                };
            }
        }

    }
}

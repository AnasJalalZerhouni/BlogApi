using BlogApi.Domain;
using BlogApi.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Features.Articles
{
    public class List
    {
        public record Query(string Tag,
            string Author,
            string FavoritedUsername,
            int? Limit,
            int? Offset,
            bool IsFeed = false) : IRequest<ArticlesEnvelope>;

        public class QueryHandler : IRequestHandler<Query, ArticlesEnvelope>
        {
            private readonly BlogContext _context;

            public QueryHandler(BlogContext _context)
            {
                this._context = _context;
            }
            public async Task<ArticlesEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                IQueryable<Article> queryable = _context.Articles.GetAllData();


                if (!string.IsNullOrWhiteSpace(message.Tag))
                {
                    var tag = await _context.ArticleTags.FirstOrDefaultAsync(x => x.TagId == message.Tag, cancellationToken);
                    if (tag != null)
                    {
                        queryable = queryable.Where(x => x.ArticleTags.Select(y => y.TagId).Contains(tag.TagId));
                    }
                    else
                    {
                        return new ArticlesEnvelope();
                    }
                }

                if (!string.IsNullOrWhiteSpace(message.Author))
                {
                    var author = await _context.Persons.FirstOrDefaultAsync(x => x.Username == message.Author, cancellationToken);
                    if (author != null)
                    {
                        queryable = queryable.Where(x => x.Author == author);
                    }
                    else
                    {
                        return new ArticlesEnvelope();
                    }
                }

                if (!string.IsNullOrWhiteSpace(message.FavoritedUsername))
                {
                    var author = await _context.Persons.FirstOrDefaultAsync(x => x.Username == message.FavoritedUsername, cancellationToken);
                    if (author != null)
                    {
                        queryable = queryable.Where(x => x.ArticleFavorites.Any(y => y.PersonId == author.PersonId));
                    }
                    else
                    {
                        return new ArticlesEnvelope();
                    }
                }

                var articles = await queryable
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip(message.Offset ?? 0)
                    .Take(message.Limit ?? 20)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return new ArticlesEnvelope()
                {
                    Articles = articles,
                    ArticlesCount = queryable.Count()
                };
            }
        }
    }
}

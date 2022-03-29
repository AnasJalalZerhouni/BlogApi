using BlogApi.Domain;
using BlogApi.Features.Articles;
using BlogApi.Infrastructure;
using BlogApi.Infrastructure.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Features.Favorites
{
    public class Create
    {
        public record Command(string Slug):IRequest<ArticleEnvelope>;

        public class handler : IRequestHandler<Command, ArticleEnvelope>
        {
            private readonly BlogContext context;

            public handler(BlogContext context)
            {
                this.context = context;
            }

            public async Task<ArticleEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var article = await context.Articles
                    .FirstOrDefaultAsync(x=>x.Slug==message.Slug,cancellationToken);

                if (article == null)
                {
                    throw new RestException(System.Net.HttpStatusCode.NotFound,
                           new { article = Constants.NOT_FOUND });
                }

                var person = context.Persons.FirstOrDefault();

                var favorite = await context.ArticleFavorites
                    .FirstOrDefaultAsync(x => x.ArticleId == article.ArticleId 
                    && x.PersonId == person.PersonId, cancellationToken);

                if (favorite == null)
                {
                    favorite = new ArticleFavorite()
                    {
                        Article = article,
                        ArticleId = article.ArticleId,
                        Person = person,
                        PersonId = person.PersonId
                    };
                    await context.ArticleFavorites.AddAsync(favorite, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);
                }

                return new ArticleEnvelope(article);
            }
        }
    }
}

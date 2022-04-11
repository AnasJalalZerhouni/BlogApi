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
            private readonly ICurrentUserAccessor currentUser;

            public handler(BlogContext context, ICurrentUserAccessor currentUser)
            {
                this.context = context;
                this.currentUser = currentUser;
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

                var person = await context.Persons
                    .FirstOrDefaultAsync(x=>x.PersonId == currentUser.GetCurrentUserId(),
                    cancellationToken);

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

using BlogApi.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Features.Favorites
{
    public class Delete
    {
        public record Command(string Slug):IRequest;

        public class handler : IRequestHandler<Command>
        {
            private readonly BlogContext context;
            public handler(BlogContext context)
            {
                this.context = context;
            }

            public async Task<Unit> Handle(Command message, CancellationToken cancellationToken)
            {
                var person = context.Persons
                    .Include(x=>x.ArticleFavorites)
                    .ThenInclude(x=>x.Article)
                    .FirstOrDefault();

                var favorite = person.ArticleFavorites
                    .FirstOrDefault(x=> x.Article !=null 
                                    && x.Article.Slug ==message.Slug);
                if (favorite != null)
                {
                    context.ArticleFavorites.Remove(favorite);
                    await context.SaveChangesAsync(cancellationToken);
                }

                return Unit.Value;
            }
        }
    }
}

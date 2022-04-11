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
            private readonly ICurrentUserAccessor currentUser;

            public handler(BlogContext context, ICurrentUserAccessor currentUser)
            {
                this.context = context;
                this.currentUser = currentUser;
            }

            public async Task<Unit> Handle(Command message, CancellationToken cancellationToken)
            {
                var person = context.Persons
                    .Include(x=>x.ArticleFavorites)
                    .ThenInclude(x=>x.Article)
                    .FirstOrDefault(x=>x.PersonId == currentUser.GetCurrentUserId());

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

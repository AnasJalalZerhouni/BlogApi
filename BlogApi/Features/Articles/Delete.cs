using BlogApi.Infrastructure;
using BlogApi.Infrastructure.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Features.Articles
{
    public class Delete
    {
        public record Command(string slug):IRequest;

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x=>x.slug).NotNull().NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly BlogContext context;
            private readonly ICurrentUserAccessor currentUser;
            public Handler(BlogContext context, ICurrentUserAccessor currentUser)
            {
                this.context = context;
                this.currentUser = currentUser;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
               
                var article = await context.Articles
                    .FirstOrDefaultAsync(x=>x.Slug ==request.slug && x.Author.PersonId ==currentUser.GetCurrentUserId() ,
                    cancellationToken);

                if (article == null)
                {
                    throw new RestException(System.Net.HttpStatusCode.NotFound,
                        new {Article = Constants.NOT_FOUND});
                }

                context.Articles.Remove(article);
                await context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}

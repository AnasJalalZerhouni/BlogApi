using BlogApi.Domain;
using BlogApi.Infrastructure;
using BlogApi.Infrastructure.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Features.Comments
{
    public class Create
    {
        public record CommentData(string? body);
        public record Command(string slug,CommentData comment):IRequest<CommentEnvolop>;

        public class CommandValidator : AbstractValidator<CommentData>
        {
            public CommandValidator()
            {
                RuleFor(x => x.body).NotNull().NotEmpty();
            }
        }
        
        public class handler : IRequestHandler<Command, CommentEnvolop>
        {
            private readonly BlogContext context;
            private ICurrentUserAccessor currentUser;
            public handler(BlogContext context, ICurrentUserAccessor currentUser)
            {
                this.context = context;
                this.currentUser = currentUser;
            }
            public async Task<CommentEnvolop> Handle(Command message, CancellationToken cancellationToken)
            {
                var article = await context.Articles
                        .FirstOrDefaultAsync(x=>x.Slug == message.slug,cancellationToken);

                if (article == null)
                {
                    throw new RestException(System.Net.HttpStatusCode.NotFound,
                            new {article=Constants.NOT_FOUND });
                }

                var user = await context.Persons.FirstOrDefaultAsync(x=>x.PersonId == currentUser.GetCurrentUserId(),cancellationToken);

                var comment = new Comment
                {
                    Article=article,
                    ArticleId=article.ArticleId,
                    Author=user,
                    AuthorId=user.PersonId,
                    Body=message.comment.body,
                    CreatedAt=DateTime.Now,
                    UpdatedAt=DateTime.Now
                };

                context.Comments.Add(comment);
                await context.SaveChangesAsync(cancellationToken);

                return new CommentEnvolop(comment);
            }
        }

    }
}

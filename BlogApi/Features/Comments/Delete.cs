using BlogApi.Infrastructure;
using BlogApi.Infrastructure.Errors;
using MediatR;

namespace BlogApi.Features.Comments
{
    public class Delete
    {
        public record Command(string slug,int id):IRequest;

        public class Handler : IRequestHandler<Command>
        {
            private readonly BlogContext context;
            public Handler(BlogContext context)
            {
                this.context = context;
            }

            public async Task<Unit> Handle(Command message, CancellationToken cancellationToken)
            {
                var comment = context.Comments.FirstOrDefault(x=>x.CommentId==message.id &&
                       x.Article!=null &&  x.Article.Slug == message.slug);

                if (comment == null)
                {
                    throw new RestException(System.Net.HttpStatusCode.NotFound,
                        new {Comment=Constants.NOT_FOUND});
                }

                context.Comments.Remove(comment);
                await context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}

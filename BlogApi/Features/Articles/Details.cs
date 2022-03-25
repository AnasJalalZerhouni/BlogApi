using BlogApi.Infrastructure;
using BlogApi.Infrastructure.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BlogApi.Features.Articles
{
    public class Details
    {
        public record Query(string slug) : IRequest<ArticleEnvelope>;



        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x=>x.slug).NotNull().NotEmpty();
            }
        }
        public class QueryHandler : IRequestHandler<Query, ArticleEnvelope>
        {
            private readonly BlogContext _context;
            public QueryHandler(BlogContext context)
            {
                _context = context;
            }
            public async Task<ArticleEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                var article = await _context.Articles.GetAllData()
                    .FirstOrDefaultAsync(x => x.Slug == message.slug, cancellationToken) ??
                    throw new RestException(
                        HttpStatusCode.NotFound,
                        new { Article = Constants.NOT_FOUND }
                    );

                return new ArticleEnvelope(article);
            }
        }
    }
}

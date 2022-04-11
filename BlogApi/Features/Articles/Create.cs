using BlogApi.Domain;
using BlogApi.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Features.Articles
{
    public class Create
    {
        public class ArticleData
        {
            public string? Title { get; set; }

            public string? Description { get; set; }

            public string? Body { get; set; }

            public string[]? TagList { get; set; }
        }

        public class ArticleDataValidator : AbstractValidator<ArticleData>
        {
            public ArticleDataValidator()
            {
                RuleFor(x => x.Title).NotNull().NotEmpty();
                RuleFor(x => x.Description).NotNull().NotEmpty();
                RuleFor(x => x.Body).NotNull().NotEmpty();
            }
        }


        public record Command(ArticleData Article) : IRequest<ArticleEnvelope>;

        public class ComandValidator : AbstractValidator<Command>
        {
            public ComandValidator()
            {
                RuleFor(x => x.Article).NotNull().SetValidator(new ArticleDataValidator());
            }
        }

        public class Handler : IRequestHandler<Command, ArticleEnvelope>
        {

            private readonly BlogContext _context;
            private readonly ICurrentUserAccessor currentUser;

            public Handler(BlogContext context, ICurrentUserAccessor currentUser)
            {
                _context = context;
                this.currentUser = currentUser;
            }

            public async Task<ArticleEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var author = await _context.Persons.FirstOrDefaultAsync(x=>x.PersonId== currentUser.GetCurrentUserId(), cancellationToken);
                var tags = new List<Tag>();

                foreach (var tag in (message.Article.TagList ?? Enumerable.Empty<string>() ))
                {
                    var t = await _context.Tags.FindAsync(tag);
                    if (t == null)
                    {
                        t = new Tag
                        {
                            TagId=tag
                        };
                        await _context.Tags.AddAsync(t);
                        await _context.SaveChangesAsync();
                    }
                    tags.Add(t);
                }

                var article = new Article
                {
                    Author = author,
                    Body= message.Article.Body,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Title =message.Article.Title,
                    Description = message.Article.Description,
                    Slug = message.Article.Title.GenerateSlug()
                };

                await _context.Articles.AddAsync(article, cancellationToken);

                await _context.ArticleTags.AddRangeAsync(tags.Select(x=> new ArticleTag
                {
                    Article=article,
                    Tag=x
                }),cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return new ArticleEnvelope(article);
            }
        }

    }
}

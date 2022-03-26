using BlogApi.Domain;
using BlogApi.Infrastructure;
using BlogApi.Infrastructure.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BlogApi.Features.Articles
{
    public class Edit
    {
        public class ArticleData
        {
            public string? Title { get; set; }

            public string? Description { get; set; }

            public string? Body { get; set; }

            public string[]? TagList { get; set; }
        }

        public record Command(ArticleData Article,string slug) : IRequest<ArticleEnvelope>;

        //public record Model(ArticleData Data);

        public class ArticleDataValidator : AbstractValidator<Command>
        {
            public ArticleDataValidator()
            {
                RuleFor(x => x.Article).NotNull();
            }
        }


        public class Handler : IRequestHandler<Command, ArticleEnvelope>
        {

            private readonly BlogContext _context;

            public Handler(BlogContext context)
            {
                _context = context;
            }

            public async Task<ArticleEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                                  var article = await _context.Articles
                     .Include(x => x.ArticleTags)
                     .FirstOrDefaultAsync(x => x.Slug == message.slug, cancellationToken);
                if (article == null)
                {
                    throw new RestException(HttpStatusCode.NotFound,new {Article = Constants.NOT_FOUND});
                }

                article.Title = message.Article.Title ?? article.Title;
                article.Description = message.Article.Description ?? article.Description;
                article.Body = message.Article.Body ?? article.Body;
                article.Slug = article.Title.GenerateSlug();


                var ArticleTagList = (message.Article.TagList ?? Enumerable.Empty<string>());

                var articleTagsToAdd = GetArticleTagsToCreate(article, ArticleTagList);
                var articleTagsToDelete = GetArticleTagsToDelete(article, ArticleTagList);

                if (_context.ChangeTracker.Entries().First(x => x.Entity == article).State == EntityState.Modified
                    || articleTagsToAdd.Any() || articleTagsToDelete.Any())
                {
                    article.UpdatedAt = DateTime.UtcNow;
                }
                var ww = articleTagsToAdd.Where(x => x.Tag is not null).Select(a => a.Tag!).ToArray();
                // ensure context is tracking any tags that are about to be created so that it won't attempt to insert a duplicate
                _context.Tags.AddRange(articleTagsToAdd.Where(x => x.Tag is not null).Select(a => a.Tag!).ToArray());


                await _context.ArticleTags.AddRangeAsync(articleTagsToAdd, cancellationToken);

                _context.ArticleTags.RemoveRange(articleTagsToDelete);

                await _context.SaveChangesAsync(cancellationToken);

                return new ArticleEnvelope(article);
            }

            public List<ArticleTag> GetArticleTagsToCreate(Article article,IEnumerable<string> newTags )
            {
                var articleTagsToCreate = new List<ArticleTag>();
                foreach (var tag in newTags)
                {
                    var t = article.ArticleTags?
                        .FirstOrDefault(x=>x.TagId==tag);
                    if (t is null)
                    {
                        articleTagsToCreate.Add(new ArticleTag
                        {
                            TagId = tag,
                            Article = article,
                            Tag = new Tag { TagId=tag},
                            ArticleId = article.ArticleId
                        });
                    }
                }
                return articleTagsToCreate;
            }

            public List<ArticleTag> GetArticleTagsToDelete(Article article, IEnumerable<string> newTags)
            {
                var articleTagsToDelete = new List<ArticleTag>();
                foreach (var ArticleTag in article.ArticleTags)
                {
                    var t = newTags.FirstOrDefault(x => x == ArticleTag.TagId);
                    if (t is null)
                    {
                        articleTagsToDelete.Add(ArticleTag);
                    }
                }
                return articleTagsToDelete;
            }
        }
    }
}

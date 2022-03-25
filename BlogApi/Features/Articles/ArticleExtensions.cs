using BlogApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Features.Articles
{
    public static class ArticleExtensions
    {
        public static IQueryable<Article> GetAllData(this DbSet<Article> articles)
        {
            return articles
                .Include(z=>z.ArticleTags)
                .Include(z=>z.Author)
                .Include(z=>z.ArticleFavorites)
                .AsNoTracking();
        }
    }
}

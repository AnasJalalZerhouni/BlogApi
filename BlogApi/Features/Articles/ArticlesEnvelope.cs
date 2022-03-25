using BlogApi.Domain;

namespace BlogApi.Features.Articles
{
    public class ArticlesEnvelope
    {
        public List<Article> Articles { get; set; } = new();

        public int ArticlesCount { get; set; }
    }
}

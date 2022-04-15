using BlogApi.Features.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BlogApi.IntegrationTests.Features.Articles
{
    public class EditTests : SliceFixture
    {
        [Fact]
        public async Task Expect_Edit_Article()
        {
            var createCommand = new Create.Command(new Create.ArticleData()
            {
                Title = "Test article dsergiu77",
                Description = "Description of the test article",
                Body = "Body of the test article",
                TagList = new string[] { "tag1", "tag2" }
            });

            var createdArticle = await ArticleHelpers.CreateArticle(this, createCommand);

            var command = new Edit.Command(new Edit.ArticleData()
            {
                Title = "Updated " + createdArticle.Title,
                Description = "Updated" + createdArticle.Description,
                Body = "Updated" + createdArticle.Body,
            }, createdArticle.Slug);

            command.Article.TagList = new string[] { createdArticle.TagList[1], "tag3" };

            var dbContext = GetDbContext();

            var currentAccessor = new StubCurrentUserAccessor(createdArticle.Author.PersonId);

            var articleEditHandler = new Edit.Handler(dbContext, currentAccessor);
            var edited = await articleEditHandler.Handle(command, new System.Threading.CancellationToken());
            
            Assert.NotNull(edited);
            Assert.Equal(edited.Article.Title, command.Article.Title);
            Assert.Equal(edited.Article.TagList.Count(), command.Article.TagList.Count());
            
            Assert.Contains(edited.Article.TagList[0], command.Article.TagList);
            Assert.Contains(edited.Article.TagList[1], command.Article.TagList);
        }
    }
}

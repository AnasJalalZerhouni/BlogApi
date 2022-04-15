using BlogApi.Features.Articles;
using BlogApi.IntegrationTests.Features.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.IntegrationTests.Features.Articles
{
    public class ArticleHelpers
    {
        public static async Task<Domain.Article> CreateArticle(SliceFixture fixture, Create.Command command)
        {
            var user = await UserHelpers.CreateDefaultUser(fixture);
            var person = await fixture
                .ExecuteDbContextAsync<Domain.Person>(x => 
                    x.Persons.FirstOrDefaultAsync(x => x.Username == user.Username, new System.Threading.CancellationToken()));


            var dbcontext = fixture.GetDbContext();
            var currentAccessor = new StubCurrentUserAccessor(person.PersonId);

            var articleCreateHandler = new Create.Handler(dbcontext, currentAccessor);

           var created = await articleCreateHandler.Handle(command, new System.Threading.CancellationToken());

            var wqwq = await dbcontext.Articles.FirstOrDefaultAsync(a => a.ArticleId == created.Article.ArticleId);

            var dbArticle = await fixture.
                ExecuteDbContextAsync(db => db.Articles.Include(x => x.ArticleTags).FirstOrDefaultAsync(a => a.ArticleId == created.Article.ArticleId));

            return dbArticle;
        }
    }
}

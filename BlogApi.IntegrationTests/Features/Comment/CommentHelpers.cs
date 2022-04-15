using BlogApi.Features.Comments;
using BlogApi.IntegrationTests.Features.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.IntegrationTests.Features.Comment
{
    public class CommentHelpers
    {
        public static async Task<Domain.Comment> CreateComment(SliceFixture fixture, Create.Command command, string? userName)
        {
            int userID = -1;
            if (userName == null)
            {
                var user = await UserHelpers.CreateDefaultUser(fixture);
            }
            var person = await fixture
            .ExecuteDbContextAsync(x =>
            x.Persons.FirstOrDefaultAsync(x => x.Username == userName, new System.Threading.CancellationToken()));
            userID = person.PersonId;

            var dbContext = fixture.GetDbContext();
            var currentAccessor = new StubCurrentUserAccessor(userID);

            var commentCreateHandler = new Create.handler(dbContext, currentAccessor);
            var created = await commentCreateHandler.Handle(command, new System.Threading.CancellationToken());

            var dbArticleWithComments = await fixture.ExecuteDbContextAsync(
                db => db.Articles
                    .Include(a => a.Comments).Include(a => a.Author)
                    .Where(a => a.Slug == command.slug)
                    .SingleOrDefaultAsync()
            );

            var dbComment = dbArticleWithComments.Comments
                .Where(c => c.ArticleId == dbArticleWithComments.ArticleId && c.Author == dbArticleWithComments.Author)
                .FirstOrDefault();

            return dbComment;
        }
    }
}

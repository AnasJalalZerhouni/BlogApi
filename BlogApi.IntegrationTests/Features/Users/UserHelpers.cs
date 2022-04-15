using BlogApi.Features.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.IntegrationTests.Features.Users
{
    public class UserHelpers
    {
        public static readonly string DefaultUsername = "gawgqqqawftettweeefaaa1";

        public static async Task<User> CreateDefaultUser(SliceFixture fixture)
        {
            var command = new Create.Command(new Create.UserData
            {
                Email = "ee@t1twqqqtttttttoto.coweeeewmlawafwmmm",
                Password = "gagwqqew1wewa",
                Username = DefaultUsername
            });

            var commandResult = await fixture.SendAsync(command);
            return commandResult.user;
        }
    }
}

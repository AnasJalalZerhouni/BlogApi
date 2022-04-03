using AutoMapper;
using BlogApi.Infrastructure;
using BlogApi.Infrastructure.Errors;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BlogApi.Features.Profiles
{
    public class ProfileReader : IProfileReader
    {

        private readonly BlogContext context;
        private readonly IMapper mapper;

        public ProfileReader(BlogContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public async Task<ProfileEnvolop> ReadProfile(string username, CancellationToken cancellationToken)
        {
            var currentUserName = new object();

            var person = await context.Persons
                .FirstOrDefaultAsync(x=>x.Username == username,cancellationToken);

            if (person == null)
            {
                throw new RestException(HttpStatusCode.NotFound, new { User = Constants.NOT_FOUND });
            }

            var profile = mapper.Map<Domain.Person,Profile>(person);

            if (currentUserName != null)
            {
                var currentPerson = await context.Persons
                    .Include(x => x.Following)
                    .Include(x => x.Followers)
                    .FirstOrDefaultAsync( cancellationToken);

                if (currentPerson.Followers.Any(x => x.TargetId == person.PersonId))
                {
                    profile.IsFollowed = true;
                }
            }
            return new ProfileEnvolop(profile);
        }
    }
}

using BlogApi.Domain;
using BlogApi.Features.Profiles;
using BlogApi.Infrastructure;
using BlogApi.Infrastructure.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BlogApi.Features.Followers
{
    public class Delete 
    {
        public record Command(string username):IRequest<ProfileEnvolop>;
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.username).NotNull().NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, ProfileEnvolop>
        {
            private readonly BlogContext context;
            private readonly IProfileReader _profileReader;
            private readonly ICurrentUserAccessor currentUser;
            public Handler(BlogContext context, IProfileReader _profileReader, ICurrentUserAccessor currentUser)
            {
                this.context = context;
                this._profileReader = _profileReader;
                this.currentUser = currentUser;
            }

            public async Task<ProfileEnvolop> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await context.Persons
                    .FirstOrDefaultAsync(x => x.PersonId == currentUser.GetCurrentUserId(),cancellationToken);

                var userToFollow = await context.Persons
                    .FirstOrDefaultAsync(x=>x.Username==request.username,cancellationToken);

                if (userToFollow is null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { User = Constants.NOT_FOUND });
                }
                var fallowedPeople = await context.FollowedPeople
                    .FirstOrDefaultAsync(x => x.ObserverId == user.PersonId
                    && x.TargetId == userToFollow.PersonId, cancellationToken);

                if (fallowedPeople !=null)
                {
                    context.FollowedPeople.Remove(fallowedPeople);
                    await context.SaveChangesAsync(cancellationToken);
                }

                return await _profileReader.ReadProfile(request.username,cancellationToken);

            }
        }

    }
}

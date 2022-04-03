using BlogApi.Domain;
using BlogApi.Features.Profiles;
using BlogApi.Infrastructure;
using BlogApi.Infrastructure.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Features.Followers
{
    public class Add
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

            public Handler(BlogContext context, IProfileReader _profileReader)
            {
                this.context = context;
                this._profileReader = _profileReader;
            }

            public async Task<ProfileEnvolop> Handle(Command message, CancellationToken cancellationToken)
            {
                var user = await context.Persons.FirstOrDefaultAsync(cancellationToken);

                var userToFollow = await context.Persons
                    .FirstOrDefaultAsync(x=>x.Username==message.username,cancellationToken);

                if(userToFollow == null)
                {
                    throw new RestException(System.Net.HttpStatusCode.NotFound,
                        new {Username=Constants.NOT_FOUND});
                }

                var fallowedPeople = await context.FollowedPeople
                    .FirstOrDefaultAsync(x=>x.ObserverId== user.PersonId 
                    && x.TargetId==userToFollow.PersonId, cancellationToken);
                if (fallowedPeople ==null)
                {
                    FollowedPeople fp = new FollowedPeople
                    {
                        Observer = user,
                        ObserverId = user.PersonId,
                        Target = userToFollow,
                        TargetId = userToFollow.PersonId
                    };

                    context.FollowedPeople.Add(fp);

                    await context.SaveChangesAsync(cancellationToken);
                }

                return await _profileReader.ReadProfile(message.username,cancellationToken);
            }
        }
    }
}

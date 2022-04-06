using AutoMapper;
using BlogApi.Infrastructure;
using BlogApi.Infrastructure.Errors;
using BlogApi.Infrastructure.Security;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Features.Users
{
    public class Edit
    {
        public class UserData
        {
            public string? Username { get; set; }

            public string? Email { get; set; }

            public string? Password { get; set; }

            public string? Bio { get; set; }

            public string? Image { get; set; }
        }

        public record Command(UserData user) :IRequest<UserEnvelope>;

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x=>x.user).NotNull();
            }
        }

        public class Handler : IRequestHandler<Command, UserEnvelope>
        {
            private readonly BlogContext context;
            private readonly IMapper mapper;
            private readonly IPasswordHasher pwHash;
            public Handler(BlogContext context, IMapper mapper, IPasswordHasher pwHash)
            {
                this.context = context;
                this.mapper = mapper;
                this.pwHash = pwHash;
            }
            public async Task<UserEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var person = await context
                    .Persons
                    .FirstOrDefaultAsync(cancellationToken);

                person.Username = message.user.Username ?? person.Username;
                person.Email = message.user.Email ?? person.Email;
                person.Bio = message.user.Bio ?? person.Bio;
                person.Image = message.user.Image ?? person.Image;

                if (!string.IsNullOrWhiteSpace(message.user.Password))
                {
                    var salt = Guid.NewGuid().ToByteArray();
                    person.Hash=await pwHash.Hash(message.user.Password,salt);
                    person.Salt= salt;
                }

                await context.SaveChangesAsync(cancellationToken);

                var user = mapper.Map<Domain.Person,User>(person);
                return new UserEnvelope(user);
            }
        }
    }
}

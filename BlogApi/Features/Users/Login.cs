using AutoMapper;
using BlogApi.Infrastructure;
using BlogApi.Infrastructure.Errors;
using BlogApi.Infrastructure.Security;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Features.Users
{
    public class Login
    {
        public class UserData
        {
            public string? Email { get; set; }
            public string? Password { get; set; }
        }
        public record Command(UserData user) :IRequest<UserEnvelope>;

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.user.Email).NotNull().NotEmpty() ;
                RuleFor(x => x.user.Password).NotNull().NotEmpty() ;

            }
        }

        public class Handler : IRequestHandler<Command, UserEnvelope>
        {
            private readonly BlogContext context;
            private readonly IMapper mapper;
            private readonly IPasswordHasher pwHash;
            private readonly IJwtTokenGenerator tokenGen;
            public Handler(BlogContext context, IMapper mapper, IPasswordHasher pwHash,
                IJwtTokenGenerator tokenGen)
            {
                this.context = context;
                this.mapper = mapper;
                this.pwHash = pwHash;
                this.tokenGen= tokenGen;
            }
            public async Task<UserEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var person = await context
                    .Persons
                    .FirstOrDefaultAsync(x=>x.Email==message.user.Email,cancellationToken);

                if (person == null)
                {
                    throw new RestException(System.Net.HttpStatusCode.NotFound,
                        new {Email=Constants.NOT_FOUND});
                }

                var salt = person.Salt;
                var hash = await pwHash
                    .Hash(message.user.Password ?? throw new InvalidOperationException(), salt);

                if (!person.Hash.SequenceEqual(hash))
                {
                    throw new RestException(System.Net.HttpStatusCode.Unauthorized,
                        new { Error = "Invalid email / password." });
                }

                var user = mapper.Map<Domain.Person,User>(person);
                user.Token = tokenGen.CreateToken(person.PersonId);
                return new UserEnvelope(user);
            }
        }
    }
}

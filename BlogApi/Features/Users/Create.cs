using AutoMapper;
using BlogApi.Domain;
using BlogApi.Infrastructure;
using BlogApi.Infrastructure.Errors;
using BlogApi.Infrastructure.Security;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BlogApi.Features.Users
{
    public class Create
    {
        public class UserData
        {
            public string? Username { get; set; }

            public string? Email { get; set; }

            public string? Password { get; set; }
        }
        public record Command(UserData user) :IRequest<UserEnvelope>;

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.user.Username).NotNull().NotEmpty();
                RuleFor(x => x.user.Email).EmailAddress();
                RuleFor(x => x.user.Password ).NotNull().NotEmpty();
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
                if (await context.Persons
                    .Where(x=>x.Email == message.user.Email)
                    .AnyAsync(cancellationToken))
                {
                    throw new RestException(HttpStatusCode.BadRequest,new {Email=Constants.IN_USE});
                }

                if (await context.Persons
                    .Where(x => x.Username == message.user.Username)
                    .AnyAsync(cancellationToken))
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { Username = Constants.IN_USE });
                }
                var salt = Guid.NewGuid().ToByteArray();

                var person = new Person
                {
                    Username = message.user.Username,
                    Email = message.user.Email,
                    Hash = await pwHash.Hash(message.user.Password 
                        ?? throw new InvalidOperationException(), salt),
                    Salt = salt
                };

                await context.Persons.AddAsync(person, cancellationToken);

                await context.SaveChangesAsync(cancellationToken);

                var user = mapper.Map<Person, User>(person);
                return new UserEnvelope(user);
            }
        }
    }
}

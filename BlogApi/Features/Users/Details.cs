using AutoMapper;
using BlogApi.Infrastructure;
using BlogApi.Infrastructure.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Features.Users
{
    public class Details
    {
        public record Query(string username):IRequest<UserEnvelope>;

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x=>x.username).NotNull().NotEmpty();
            }
        }

        public class QueryHandler : IRequestHandler<Query, UserEnvelope>
        {

            private readonly BlogContext context;
            private readonly IMapper mapper;
            public QueryHandler(BlogContext context, IMapper mapper)
            {
                this.context = context;
                this.mapper = mapper;
            }

            public async Task<UserEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                var person = await context.Persons.FirstOrDefaultAsync(x=>x.Username == message.username,
                    cancellationToken);

                if (person == null)
                {
                    throw new RestException(System.Net.HttpStatusCode.NotFound,
                        new { Email = Constants.NOT_FOUND });
                }

                var user = mapper.Map<Domain.Person,User>(person);
                return new UserEnvelope(user);
            }
        }
    }
}

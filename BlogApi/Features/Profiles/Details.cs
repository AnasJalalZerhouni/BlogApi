using BlogApi.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Features.Profiles
{
    public class Details
    {
        public record Query(string username):IRequest<ProfileEnvolop>;

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.username).NotEmpty();
            }
        }

        public class QueryHandler : IRequestHandler<Query, ProfileEnvolop>
        {

            private readonly IProfileReader _profileReader;

            public QueryHandler(BlogContext context, IProfileReader _profileReader)
            {
                this._profileReader = _profileReader;
            }

            public async Task<ProfileEnvolop> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _profileReader.ReadProfile(request.username,cancellationToken);
            }
        }
    }
}

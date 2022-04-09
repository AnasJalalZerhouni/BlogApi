using MediatR;

namespace BlogApi.Infrastructure
{
    public class DBContextTransactionPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly BlogContext _context;

        public DBContextTransactionPipelineBehavior(BlogContext context)
        {
            _context = context;
        }
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            TResponse? result = default(TResponse);

            try
            {
                _context.BeginTransaction();
                result = await next();
                _context.CommitTransaction();

            }
            catch (Exception)
            {
                _context.RollbackTransaction();
                throw;
            }
            return result;
        }
    }
}

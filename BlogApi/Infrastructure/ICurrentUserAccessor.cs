namespace BlogApi.Infrastructure
{
    public interface ICurrentUserAccessor
    {
        int? GetCurrentUserId();
    }
}

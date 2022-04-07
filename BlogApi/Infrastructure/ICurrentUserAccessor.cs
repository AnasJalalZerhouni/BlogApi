namespace BlogApi.Infrastructure
{
    public interface ICurrentUserAccessor
    {
        string? GetCurrentUserId();
    }
}

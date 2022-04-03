namespace BlogApi.Features.Profiles
{
    public interface IProfileReader
    {
        Task<ProfileEnvolop> ReadProfile(string username, CancellationToken cancellationToken);
    }
}

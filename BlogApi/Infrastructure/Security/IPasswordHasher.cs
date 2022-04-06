namespace BlogApi.Infrastructure.Security
{
    public interface IPasswordHasher
    {
        Task<byte[]> Hash(string password,byte[] salt);
    }
}

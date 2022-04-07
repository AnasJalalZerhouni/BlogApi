namespace BlogApi.Infrastructure.Security
{
    public interface IJwtTokenGenerator
    {
        string CreateToken(int id);
    }
}

using System.Security.Cryptography;
using System.Text;

namespace BlogApi.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {

        private readonly HMACSHA512 x = new(Encoding.UTF8.GetBytes("BlogApiSecret"));
        public Task<byte[]> Hash(string password, byte[] salt)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            var allBytes = new byte[passwordBytes.Length+salt.Length];
           passwordBytes.CopyTo(allBytes, 0);
           salt.CopyTo(allBytes, passwordBytes.Length);

            return x.ComputeHashAsync(new MemoryStream(allBytes));
        }

        public void Dispose() => x.Dispose();
    }
}

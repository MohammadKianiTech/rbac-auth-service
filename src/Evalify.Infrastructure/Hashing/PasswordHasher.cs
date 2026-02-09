using System.Security.Cryptography;
using Evalify.Application.Abstractions.Hashing;

namespace Evalify.Infrastructure.Hashing;

public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100000;

    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;
    public string GetSha256Hash(string input)
    {
        var byteValue = System.Text.Encoding.UTF8.GetBytes(input);
        var byteHash = SHA256.HashData(byteValue);
        return Convert.ToBase64String(byteHash);
    }
    public string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

        return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
    }

    public bool Verify(string password, string hash)
    {
        string[] parts = hash.Split('-');

        byte[] hashBytes = Convert.FromHexString(parts[0]);

        byte[] salt = Convert.FromHexString(parts[1]);

        byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);
        //return hash.SequenceEqual(inputHash);
        return CryptographicOperations.FixedTimeEquals(hashBytes, inputHash);
    }
}
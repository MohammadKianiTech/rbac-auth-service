namespace Evalify.Application.Abstractions.Hashing;

public interface IPasswordHasher
{
    string GetSha256Hash(string input);
    string Hash(string password);
    bool Verify(string password, string hash);
}
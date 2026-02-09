namespace Evalify.Domain.Errors;

public static class TokenErrors
{
    public static readonly Error NotFound = new("Token.NotFound", "This is not our token!");
}
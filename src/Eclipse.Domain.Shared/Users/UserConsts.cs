namespace Eclipse.Domain.Shared.Users;

public static class UserConsts
{
    public static readonly TimeSpan SignInCodeExpiration = TimeSpan.FromMinutes(5);

    private static readonly string _signInCodeChars = "0123456789";

    public static string GenerateSignInCode()
    {
        var chars = Enumerable.Range(0, 6)
            .Select(_ => _signInCodeChars[Random.Shared.Next(0, _signInCodeChars.Length)])
            .ToArray();

        return new string(chars);
    }
}

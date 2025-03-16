namespace Eclipse.Application.Account.SendSignInCode;

internal sealed class SendSignInCodeArgs
{
    public required string SignInCode { get; set; }
    public required string Culture { get; set; }
    public long ChatId { get; set; }
}

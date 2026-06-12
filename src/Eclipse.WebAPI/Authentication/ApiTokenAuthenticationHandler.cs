using Eclipse.Application.Contracts.ApiTokens;
using Eclipse.Domain.ApiTokens;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Eclipse.WebAPI.Authentication;

public sealed class ApiTokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "ApiToken";

    private readonly IApiTokenService _apiTokenService;

    private readonly ApiTokenAuthenticationOptions _authOptions;

    public ApiTokenAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IApiTokenService apiTokenService,
        IOptions<ApiTokenAuthenticationOptions> authOptions)
        : base(options, logger, encoder)
    {
        _apiTokenService = apiTokenService;
        _authOptions = authOptions.Value;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(_authOptions.HeaderName, out var token) || token.IsNullOrEmpty())
        {
            return AuthenticateResult.NoResult();
        }

        var plaintext = token.ToString();

        if (!plaintext.StartsWith(_authOptions.TokenPrefix, StringComparison.Ordinal))
        {
            return AuthenticateResult.Fail("Invalid API token format.");
        }

        var hash = ApiTokenGenerator.Hash(plaintext);

        var principal = await _apiTokenService.AuthenticateAsync(hash, Context.RequestAborted);

        if (principal is null)
        {
            return AuthenticateResult.Fail("Invalid or expired API token.");
        }

        var ticket = new AuthenticationTicket(
            new ClaimsPrincipal(
                new ClaimsIdentity(principal.Claims, SchemeName)
            ),
            SchemeName
        );

        return AuthenticateResult.Success(ticket);
    }
}

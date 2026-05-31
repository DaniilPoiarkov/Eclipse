using Eclipse.Application.Contracts.ApiTokens;
using Eclipse.Common.Clock;
using Eclipse.Common.Results;
using Eclipse.Domain.ApiTokens;
using Eclipse.Domain.Shared.ApiTokens;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Users;

using Microsoft.AspNetCore.Identity;

using System.Security.Claims;

namespace Eclipse.Application.ApiTokens;

internal sealed class ApiTokenService : IApiTokenService
{
    private readonly IApiTokenRepository _apiTokenRepository;

    private readonly IUserRepository _userRepository;

    private readonly IUserClaimsPrincipalFactory<User> _principalFactory;

    private readonly ITimeProvider _timeProvider;

    public ApiTokenService(
        IApiTokenRepository apiTokenRepository,
        IUserRepository userRepository,
        IUserClaimsPrincipalFactory<User> principalFactory,
        ITimeProvider timeProvider)
    {
        _apiTokenRepository = apiTokenRepository;
        _userRepository = userRepository;
        _principalFactory = principalFactory;
        _timeProvider = timeProvider;
    }

    public async Task<Result<CreateApiTokenResult>> CreateAsync(Guid userId, CreateApiTokenDto create, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(userId, cancellationToken);

        if (user is null)
        {
            return DefaultErrors.EntityNotFound<User>();
        }

        var existing = await _apiTokenRepository.GetByUserIdAsync(userId, cancellationToken);

        if (existing.Count >= ApiTokensConsts.MaxCount)
        {
            return Error.Validation("ApiToken.LimitReached", "ApiToken:LimitReached", ApiTokensConsts.MaxCount);
        }

        if (existing.Any(t => t.Name == create.Name))
        {
            return Error.Conflict("ApiToken.NameDuplicate", "ApiToken:NameDuplicate", create.Name);
        }

        var (token, plaintext) = ApiToken.Create(userId, create.Name, _timeProvider.Now);

        await _apiTokenRepository.CreateAsync(token, cancellationToken);

        return new CreateApiTokenResult(token.ToDto(), plaintext);
    }

    public async Task<Result<IReadOnlyList<ApiTokenDto>>> GetListAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokens = await _apiTokenRepository.GetByUserIdAsync(userId, cancellationToken);

        IReadOnlyList<ApiTokenDto> list = [.. tokens.Select(t => t.ToDto())];

        return Result<IReadOnlyList<ApiTokenDto>>.Success(list);
    }

    public async Task<Result> RevokeAsync(Guid userId, Guid tokenId, CancellationToken cancellationToken = default)
    {
        var token = await _apiTokenRepository.FindAsync(tokenId, cancellationToken);

        if (token is null || token.UserId != userId)
        {
            return DefaultErrors.EntityNotFound<ApiToken>();
        }

        await _apiTokenRepository.DeleteAsync(tokenId, cancellationToken);

        return Result.Success();
    }

    public async Task<ClaimsPrincipal?> AuthenticateAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        var token = await _apiTokenRepository.FindByTokenHashAsync(tokenHash, cancellationToken);

        if (token is null || token.IsExpired(_timeProvider.Now))
        {
            return null;
        }

        var user = await _userRepository.FindAsync(token.UserId, cancellationToken);

        if (user is null)
        {
            return null;
        }

        return await _principalFactory.CreateAsync(user);
    }
}

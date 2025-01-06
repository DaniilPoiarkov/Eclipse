using Eclipse.Common.Caching;
using Eclipse.Domain.Users;

namespace Eclipse.Pipelines.Culture;

internal sealed class CultureTracker : ICultureTracker
{
    private readonly ICacheService _cacheService;

    private readonly IUserRepository _userRepository;

    public CultureTracker(ICacheService cacheService, IUserRepository userRepository)
    {
        _cacheService = cacheService;
        _userRepository = userRepository;
    }

    public Task<string?> GetAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var options = new CacheOptions
        {
            Expiration = CacheConsts.ThreeDays,
        };

        return _cacheService.GetOrCreateAsync(
            GetKey(chatId),
            () => GetCultureAsync(chatId, cancellationToken),
            options,
            cancellationToken
        );
    }

    public Task ResetAsync(long chatId, string culture, CancellationToken cancellationToken = default)
    {
        var options = new CacheOptions
        {
            Expiration = CacheConsts.ThreeDays,
        };

        return _cacheService.SetAsync(GetKey(chatId), culture, options, cancellationToken);
    }

    private async Task<string?> GetCultureAsync(long chatId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByChatIdAsync(chatId, cancellationToken);

        return user?.Culture;
    }

    private static string GetKey(long chatId) => $"lang-{chatId}";
}

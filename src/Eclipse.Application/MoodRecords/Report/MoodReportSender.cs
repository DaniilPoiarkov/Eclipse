using Eclipse.Application.Contracts.Reports;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace Eclipse.Application.MoodRecords.Report;

internal sealed class MoodReportSender : IMoodReportSender
{
    private readonly IUserRepository _userRepository;

    private readonly ICurrentCulture _currentCulture;

    private readonly IReportsService _reportsService;

    private readonly ITelegramBotClient _client;

    private readonly IStringLocalizer<MoodReportSender> _localizer;

    private readonly ILogger<MoodReportSender> _logger;

    public MoodReportSender(
        IUserRepository userRepository,
        ICurrentCulture currentCulture,
        IReportsService reportsService,
        ITelegramBotClient client,
        IStringLocalizer<MoodReportSender> localizer,
        ILogger<MoodReportSender> logger)
    {
        _userRepository = userRepository;
        _currentCulture = currentCulture;
        _reportsService = reportsService;
        _client = client;
        _localizer = localizer;
        _logger = logger;
    }

    public async Task Send(Guid userId, MoodReportOptions options, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(userId, cancellationToken);

        if (user is not { IsEnabled: true })
        {
            _logger.LogError("User with id {UserId} not found or disabled.", userId);
            return;
        }

        using var _ = _currentCulture.UsingCulture(user.Culture);

        var message = _localizer["Jobs:SendMoodReport:Caption"];

        using var stream = await _reportsService.GetMoodReportAsync(user.Id, options, cancellationToken);

        try
        {
            await _client.SendPhoto(user.ChatId,
                InputFile.FromStream(stream, $"mood-report.png"),
                caption: message,
                cancellationToken: cancellationToken
            );
        }
        catch (ApiRequestException ex)
        {
            _logger.LogError(ex, "Failed to send mood report for user {UserId}. Disabling user.", user.Id);

            user.SetIsEnabled(false);
            await _userRepository.UpdateAsync(user, cancellationToken);
        }
    }
}

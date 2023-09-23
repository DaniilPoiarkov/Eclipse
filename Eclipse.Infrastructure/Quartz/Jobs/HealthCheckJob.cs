using Eclipse.Infrastructure.Builder;
using Quartz;
using Telegram.Bot;

namespace Eclipse.Infrastructure.Quartz.Jobs;

internal class HealthCheckJob : IJob
{
    private readonly HttpClient _httpClient;

    private readonly ITelegramBotClient _botClient;

    private readonly InfrastructureOptions _options;

    public HealthCheckJob(HttpClient httpClient, ITelegramBotClient botClient, InfrastructureOptions options)
    {
        _httpClient = httpClient;
        _botClient = botClient;
        _options = options;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var response = await _httpClient.GetAsync("/health-checks");

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            await _botClient.SendTextMessageAsync(_options.TelegramOptions.Chat, content);
        }
    }
}

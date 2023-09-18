﻿using Eclipse.Core.Core;
using Eclipse.Core.Validation;
using Telegram.Bot;

namespace Eclipse.Core.Pipelines;

public class AccessDeniedPipeline : PipelineBase, IAccessDeniedPipeline
{
    protected readonly ITelegramBotClient BotClient;

    protected IReadOnlyList<ValidationResult> Errors { get; private set; } = new List<ValidationResult>();

    public AccessDeniedPipeline(ITelegramBotClient botClient)
    {
        BotClient = botClient;
    }

    public virtual void SetResults(IEnumerable<ValidationResult> results)
    {
        Errors = results.Where(r => r.IsFailed).ToList();
    }

    protected override void Initialize()
    {
        RegisterStage(SendErrors);
    }

    protected virtual IResult SendErrors(MessageContext context)
    {
        var infos = Errors.Select(e => e.ErrorMessage);
        return Text(string.Join(Environment.NewLine, infos));
    }
}

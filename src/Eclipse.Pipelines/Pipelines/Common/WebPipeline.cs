﻿using Eclipse.Application.Contracts.Url;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Pipelines.Attributes;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.Common;

[ComingSoonFeature]
[Route("", "/web")]
internal sealed class WebPipeline : EclipsePipelineBase
{
    private readonly IAppUrlProvider _appUrlProvider;

    public WebPipeline(IAppUrlProvider appUrlProvider)
    {
        _appUrlProvider = appUrlProvider;
    }

    protected override void Initialize()
    {
        RegisterStage(SendWebAppButton);
    }

    private IResult SendWebAppButton(MessageContext context)
    {
        InlineKeyboardButton[] buttons = [
            InlineKeyboardButton.WithWebApp(Localizer["OpenApi"], new WebAppInfo
            {
                Url = $"{_appUrlProvider.AppUrl.EnsureEndsWith('/')}swagger/index.html"
            })
        ];

        return Menu(buttons, Localizer["Web:OpenApi"]);
    }
}

﻿using Eclipse.Core.Pipelines;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Application.Telegram.Pipelines;

public abstract class EclipsePipelineBase : PipelineBase
{
    protected static readonly IReadOnlyCollection<KeyboardButton> MainMenuButton = new KeyboardButton[]
    {
        new KeyboardButton("Suggest")
    };
}

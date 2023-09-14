﻿using Eclipse.Application.Contracts.Base;

namespace Eclipse.Application.Contracts.Suggestions;

public class SuggestionDto : EntityDto
{
    public string Text { get; set; } = string.Empty;

    public long TelegramUserId { get; set; }

    public DateTime CreatedAt { get; set; }
}

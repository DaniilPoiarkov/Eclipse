﻿using Eclipse.Domain.Shared.Importing;

namespace Eclipse.Domain.Users.Import;

public class ImportTodoItemDto : ImportEntityBase
{
    public Guid UserId { get; set; }

    public string Text { get; set; } = string.Empty;

    public bool IsFinished { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? FinishedAt { get; set; }
}

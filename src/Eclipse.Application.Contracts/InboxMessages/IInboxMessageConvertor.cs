﻿namespace Eclipse.Application.Contracts.InboxMessages;

public interface IInboxMessageConvertor
{
    Task<OutboxToInboxConversionResult> ConvertAsync(int count, CancellationToken cancellationToken = default);
}

﻿using System.Threading.Channels;

namespace Eclipse.Infrastructure.EventBus;

internal sealed class InMemoryQueue<T>
{
    private readonly Channel<T> _channel = Channel.CreateUnbounded<T>();

    public async Task WriteAsync(T item, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(item, cancellationToken);
    }

    public IAsyncEnumerable<T> ReadAsync(CancellationToken cancellationToken = default)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}

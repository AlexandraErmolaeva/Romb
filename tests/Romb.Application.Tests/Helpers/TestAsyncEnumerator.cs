﻿
namespace Romb.Application.Tests.Helpers;

public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public T Current => _inner.Current;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }
    public ValueTask DisposeAsync()
    {
        _inner.Dispose();

        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return new ValueTask<bool>(_inner.MoveNext());
    }
}

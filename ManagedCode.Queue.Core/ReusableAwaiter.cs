using System.Runtime.CompilerServices;

namespace ManagedCode.Queue.Core;

public sealed class ReusableAwaiter<T> : INotifyCompletion
{
    private Action? _continuation;
    private T? _result;
    private Exception? _exception;

    public bool IsCompleted { get; private set; }

    public T GetResult()
    {
        if (_exception is not null) throw _exception;

        return _result!;
    }

    public void OnCompleted(Action continuation)
    {
        if (_continuation is null)
            throw new InvalidOperationException($"This {nameof(ReusableAwaiter<T>)} instance has already been listened");

        _continuation = continuation;
    }

    public bool TrySetResult(T result)
    {
        if (IsCompleted) return false;

        IsCompleted = true;

        _result = result;
        _continuation?.Invoke();

        return true;
    }

    public bool TrySetCanceled()
    {
        if (IsCompleted) return false;

        IsCompleted = true;

        _exception = new TaskCanceledException();
        _continuation?.Invoke();

        return true;
    }

    public bool TrySetException(Exception exception)
    {
        if (IsCompleted) return false;

        IsCompleted = true;
        _exception = exception;

        _continuation?.Invoke();

        return true;
    }

    public void Reset()
    {
        _result = default;
        _continuation = null;
        _exception = null;
        IsCompleted = false;
    }

    public ReusableAwaiter<T> GetAwaiter()
    {
        return this;
    }
}
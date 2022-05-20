using System.Runtime.CompilerServices;
using ManagedCode.Queue.Core.Models;

namespace ManagedCode.Queue.Core.Abstractions;

public interface IQueueReceiver
{
    IAsyncEnumerable<Message> ReceiveMessages(string topic, CancellationToken cancellationToken = default);

    Task<int> GetMessageCountAsync(CancellationToken cancellationToken = default);

    Task CleanQueue(CancellationToken cancellationToken);
    IAsyncEnumerable<Message> ReceiveMessages([EnumeratorCancellation] CancellationToken cancellationToken = default);
}
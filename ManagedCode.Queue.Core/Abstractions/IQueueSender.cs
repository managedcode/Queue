using System.Runtime.CompilerServices;
using ManagedCode.Queue.Core.Models;

namespace ManagedCode.Queue.Core.Abstractions;

public interface IQueueSender
{
    Task SendMessageAsync(string queue, Message message, CancellationToken cancellationToken = default);
    Task SendMessageAsync(string queue, string topic, Message message, CancellationToken cancellationToken = default);
}

public interface IQueueReceiver
{
    IAsyncEnumerable<Message> ReceiveMessages(string queue, string topic, [EnumeratorCancellation]  CancellationToken cancellationToken = default);
    IAsyncEnumerable<Message> ReceiveMessages(string queue, [EnumeratorCancellation] CancellationToken cancellationToken = default);
}

public interface IQueueManager
{
    Task CreateQueueAsync(string queue, CancellationToken cancellationToken = default);
    Task DeleteQueueAsync(string queue, CancellationToken cancellationToken = default);
    Task CleanQueue(string queue, CancellationToken cancellationToken);
    Task CreateTopicAsync(string queue, string topic, CancellationToken cancellationToken = default);
    Task DeleteTopicAsync(string queue, string topic, CancellationToken cancellationToken = default);
}


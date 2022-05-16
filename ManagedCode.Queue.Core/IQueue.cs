namespace ManagedCode.Queue.Core;

public interface IQueue
{
    Task CreateQueueIfNotExistAsync(CancellationToken cancellationToken = default);
    Task CreateQueueAsync(CancellationToken cancellationToken = default);
    Task DeleteQueueAsync(CancellationToken cancellationToken = default);
    Task<int> GetMessageCountAsync(CancellationToken cancellationToken = default);
    Task CleanQueue(CancellationToken cancellationToken);

    Task<Message?> ReceiveMessageAsync(CancellationToken cancellationToken = default);
    Task<MessageId?> SendMessageAsync(string message, CancellationToken cancellationToken = default);

    Task ProcessMessages(Func<Message, Task> processMessage, Func<MessageError, Task> processError, CancellationToken cancellationToken);
    Task ProcessMessages(Func<Message, Task> processMessage, Func<MessageError, Task> processError, int parallel, CancellationToken cancellationToken);
    Task ProcessMessages(Action<Message> processMessage, CancellationToken cancellationToken);
    Task ProcessMessages(Action<Message> processMessage, int parallel, CancellationToken cancellationToken);

    Task DeleteMessageAsync(MessageId id, CancellationToken cancellationToken = default);
}
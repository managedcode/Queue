namespace ManagedCode.Queue.Core;

public interface IQueueSender
{
    Task SendMessageAsync(Message message, CancellationToken cancellationToken = default);
}
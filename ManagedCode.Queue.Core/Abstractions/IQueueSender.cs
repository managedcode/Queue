using ManagedCode.Queue.Core.Models;

namespace ManagedCode.Queue.Core.Abstractions;

public interface IQueueSender
{
    Task SendMessageAsync(string message, CancellationToken cancellationToken = default);
    Task CreateQueueIfNotExistAsync(CancellationToken cancellationToken = default);
    Task CreateQueueAsync(CancellationToken cancellationToken = default);
    Task DeleteQueueAsync(CancellationToken cancellationToken = default);
    Task InitializeAsync(string topic, CancellationToken cancellationToken = default);
}
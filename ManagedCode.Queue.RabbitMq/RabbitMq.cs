using ManagedCode.Queue.AzureQueue.Options;
using ManagedCode.Queue.Core;
using Microsoft.Extensions.Logging;

namespace ManagedCode.Queue.RabbitMq;

public class RabbitMq : IQueue
{
    private readonly ILogger<RabbitMq> _logger;
    private readonly RabbitMqOptions _options;

    public RabbitMq(ILogger<RabbitMq> logger, RabbitMqOptions options)
    {
        _logger = logger;
        _options = options;
    }

    public Task CreateQueueIfNotExistAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task CreateQueueAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteQueueAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetMessageCountAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task CleanQueue(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Message?> ReceiveMessageAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<MessageId?> SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task ProcessMessages(Func<Message, Task> processMessage, Func<MessageError, Task> processError, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task ProcessMessages(Func<Message, Task> processMessage, Func<MessageError, Task> processError, int parallel, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task ProcessMessages(Func<Message, Task> processMessage, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task ProcessMessages(Func<Message, Task> processMessage, int parallel, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task ProcessMessages(Action<Message> processMessage, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task ProcessMessages(Action<Message> processMessage, int parallel, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteMessageAsync(MessageId id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
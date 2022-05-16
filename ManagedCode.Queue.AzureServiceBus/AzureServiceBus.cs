using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using ManagedCode.Queue.AzureQueue.Options;
using ManagedCode.Queue.Core;
using Microsoft.Extensions.Logging;

namespace ManagedCode.Queue.AzureServiceBus;

public class AzureServiceBus : IQueue
{
    private readonly ILogger<AzureServiceBus> _logger;
    private readonly AzureServiceBusOptions _options;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;
    private readonly ServiceBusProcessor _processor;
    private readonly ServiceBusReceiver _receiver;
    private readonly ServiceBusAdministrationClient _adminClient;

    public AzureServiceBus(ILogger<AzureServiceBus> logger, AzureServiceBusOptions options)
    {
        _logger = logger;
        _options = options;
        _client = new ServiceBusClient(options.ConnectionString);
        _sender = _client.CreateSender(options.Queue);
        _processor = _client.CreateProcessor(options.Queue);
        _receiver = _client.CreateReceiver(options.Queue);
        _adminClient = new ServiceBusAdministrationClient(options.ConnectionString);
    }

    public async Task CreateQueueIfNotExistAsync(CancellationToken cancellationToken = default)
    {
        if (!await _adminClient.QueueExistsAsync(_options.Queue, cancellationToken))
        {
            await _adminClient.CreateQueueAsync(_options.Queue, cancellationToken);
        }
    }

    public Task CreateQueueAsync(CancellationToken cancellationToken = default)
    {
        return _adminClient.CreateQueueAsync(_options.Queue, cancellationToken);
    }

    public Task DeleteQueueAsync(CancellationToken cancellationToken = default)
    {
        return _adminClient.DeleteQueueAsync(_options.Queue, cancellationToken);
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

    public async Task<MessageId?> SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        await _sender.SendMessageAsync(new ServiceBusMessage(message), cancellationToken);
        return null;
    }

    public async Task ProcessMessages(Func<Message, Task> processMessage, Func<MessageError, Task> processError, CancellationToken cancellationToken)
    {
        _processor.ProcessErrorAsync += args => processError.Invoke(new MessageError(args.Exception));

        _processor.ProcessMessageAsync += args => processMessage.Invoke(
            new Message
            {
                Id = new MessageId
                {
                    Id = args.Message.MessageId,
                    ReceiptHandle = args.Message.To
                },
                Body = args.Message.Body.ToString()
            });

        await _processor.StartProcessingAsync(cancellationToken);
    }

    public async Task ProcessMessages(Func<Message, Task> processMessage, Func<MessageError, Task> processError, int parallel,
        CancellationToken cancellationToken)
    {
        _processor.ProcessErrorAsync += args => processError.Invoke(new MessageError(args.Exception));

        _processor.ProcessMessageAsync += args => processMessage.Invoke(
            new Message
            {
                Id = new MessageId
                {
                    Id = args.Message.MessageId,
                    ReceiptHandle = args.Message.To
                },
                Body = args.Message.Body.ToString()
            });

        await _processor.StartProcessingAsync(cancellationToken);
    }

    public async Task ProcessMessages(Action<Message> processMessage, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task ProcessMessages(Action<Message> processMessage, int parallel, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteMessageAsync(MessageId id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
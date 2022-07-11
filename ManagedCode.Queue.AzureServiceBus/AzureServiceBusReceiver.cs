using System.Runtime.CompilerServices;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using ManagedCode.Queue.AzureServiceBus.Options;
using ManagedCode.Queue.Core;
using ManagedCode.Queue.Core.Abstractions;
using ManagedCode.Queue.Core.Models;
using Microsoft.Extensions.Logging;

namespace ManagedCode.Queue.AzureServiceBus;

public class AzureServiceBusQueue: IQueueSender, IQueueReceiver, IQueueManager, IAsyncDisposable
{
    private readonly AzureServiceBusOptions _options;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusAdministrationClient _adminClient;
    private readonly ILogger<AzureServiceBusQueue> _logger;
    private Dictionary<string,ServiceBusSender> _senders = new ();


    public AzureServiceBusQueue(ILogger<AzureServiceBusQueue> logger, AzureServiceBusOptions options)
    {
        _logger = logger;
        _options = options;
        _client = new ServiceBusClient(options.ConnectionString);
        _adminClient = new ServiceBusAdministrationClient(options.ConnectionString);
        _options = options;
        _client = new ServiceBusClient(options.ConnectionString);
        _adminClient = new ServiceBusAdministrationClient(options.ConnectionString);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var _sender in _senders.Values)
        {
            await _sender.DisposeAsync();
        }
        
        await _client.DisposeAsync();
    }
    
    public Task SendMessageAsync(string queue, Message message, CancellationToken cancellationToken = default)
    {
        if (!_senders.ContainsKey(queue))
        {
            _senders[queue] = _client.CreateSender(queue);
        }

        _logger.LogInformation($"SendMessageAsync to queue {queue}");
        
        return _senders[queue].SendMessageAsync(new ServiceBusMessage(message.Body), cancellationToken);
    }

    public Task SendMessageAsync(string queue, string topic, Message message, CancellationToken cancellationToken = default)
    {
        if (!_senders.ContainsKey(topic))
        {
            _senders[topic] = _client.CreateSender(topic);
        }

        _logger.LogInformation($"SendMessageAsync to queue {topic}");
        
        return _senders[topic].SendMessageAsync(new ServiceBusMessage(message.Body), cancellationToken);
    }

    public async IAsyncEnumerable<Message> ReceiveMessages(string queue, string topic, CancellationToken cancellationToken = default)
    {
        var subscriptionName = topic + "Subscription";

        await CreateTopicAsync(queue, topic, cancellationToken);

        if (!await _adminClient.SubscriptionExistsAsync(topic, subscriptionName, cancellationToken))
        {
            await _adminClient.CreateSubscriptionAsync(topic, subscriptionName, cancellationToken);
        }

        await using var processor = _client.CreateProcessor(
            topic,
            subscriptionName,
            new ServiceBusProcessorOptions {ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete});

        await foreach (var message in ProcessMessagesAsync(cancellationToken, processor))
        {
            if (cancellationToken.IsCancellationRequested) yield break;
            yield return message;
        }
    }

    public async IAsyncEnumerable<Message> ReceiveMessages(string queue, CancellationToken cancellationToken = default)
    {
        await using var processor = _client.CreateProcessor(queue,
            new ServiceBusProcessorOptions {ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete});

        await foreach (var message in ProcessMessagesAsync(cancellationToken, processor))
        {
            if (cancellationToken.IsCancellationRequested) yield break;
            yield return message;
        }
    }

    public async Task CreateQueueAsync(string queue, CancellationToken cancellationToken = default)
    {
        if (!await _adminClient.QueueExistsAsync(queue, cancellationToken))
        {
            await _adminClient.CreateQueueAsync(queue, cancellationToken);
        }
    }

    public async Task DeleteQueueAsync(string queue, CancellationToken cancellationToken = default)
    {
        if (await _adminClient.QueueExistsAsync(queue, cancellationToken))
        {
            await _adminClient.DeleteQueueAsync(queue, cancellationToken);
        }
    }

    public Task CleanQueue(string queue, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task CreateTopicAsync(string queue, string topic, CancellationToken cancellationToken = default)
    {
        if (!await _adminClient.TopicExistsAsync(topic, cancellationToken))
        {
            await _adminClient.CreateTopicAsync(topic, cancellationToken);
        }
    }

    public async Task DeleteTopicAsync(string queue, string topic, CancellationToken cancellationToken = default)
    {
        if (await _adminClient.TopicExistsAsync(topic, cancellationToken))
        {
            await _adminClient.DeleteTopicAsync(topic, cancellationToken);
        }
    }
    
    private static async IAsyncEnumerable<Message> ProcessMessagesAsync([EnumeratorCancellation] CancellationToken cancellationToken, ServiceBusProcessor processor)
    {
        var reusableAwaiter = new ReusableAwaiter<Message>();

        cancellationToken.Register(() =>
        {
            // this callback will be executed when token is cancelled
            reusableAwaiter.TrySetCanceled();
        });

        processor.ProcessMessageAsync += OnProcessMessageAsync;
        processor.ProcessErrorAsync += OnProcessErrorAsync;

        await processor.StartProcessingAsync(cancellationToken);

        while (processor.IsProcessing)
        {
            Message? message = null;

            try
            {
                message = await reusableAwaiter;
                reusableAwaiter.Reset();
            }
            catch
            {
                reusableAwaiter.Reset();
            }

            if (message is not null) 
                yield return message;
        }

        Task OnProcessMessageAsync(ProcessMessageEventArgs args)
        {
            reusableAwaiter.TrySetResult(new Message
            {
                Id = new MessageId
                {
                    Id = args.Message.MessageId,
                    ReceiptHandle = args.Message.To,
                },
                
                Body = args.Message.Body.ToString()
            });
              

            return Task.CompletedTask;
        }

        Task OnProcessErrorAsync(ProcessErrorEventArgs args)
        {
            reusableAwaiter.TrySetResult(new Message{
                Id = new MessageId(),
                Body = string.Empty,
                Error = new Error(args.Exception)
            });

            return Task.CompletedTask;
        }
    }
}
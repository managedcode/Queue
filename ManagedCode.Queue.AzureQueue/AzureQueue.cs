using System.Runtime.CompilerServices;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using ManagedCode.Queue.AzureQueue.Options;
using ManagedCode.Queue.Core;
using Microsoft.Extensions.Logging;

namespace ManagedCode.Queue.AzureQueue;

//https://docs.microsoft.com/en-us/azure/storage/queues/storage-dotnet-how-to-use-queues?tabs=dotnet
public class AzureQueue : IQueue
{
    private readonly ILogger<AzureQueue> _logger;
    private readonly AzureQueueOptions _options;
    private readonly QueueClient _queueClient;

    public AzureQueue(ILogger<AzureQueue> logger, AzureQueueOptions options)
    {
        _logger = logger;
        _options = options;
        _queueClient = new QueueClient(options.ConnectionString, options.Queue);
    }

    public async Task CreateQueueIfNotExistAsync(CancellationToken cancellationToken = default)
    {
        await _queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task CreateQueueAsync(CancellationToken cancellationToken = default)
    {
        await _queueClient.CreateAsync(cancellationToken: cancellationToken);
    }

    public async Task DeleteQueueAsync(CancellationToken cancellationToken = default)
    {
        await _queueClient.DeleteIfExistsAsync(cancellationToken);
    }

    public async Task<int> GetMessageCountAsync(CancellationToken cancellationToken = default)
    {
        QueueProperties properties = await _queueClient.GetPropertiesAsync(cancellationToken);
        return properties.ApproximateMessagesCount;
    }

    public Task CleanQueue(CancellationToken cancellationToken)
    {
        return _queueClient.ClearMessagesAsync(cancellationToken);
    }

    public async Task<Message?> ReceiveMessageAsync(CancellationToken cancellationToken)
    {
        var message = await _queueClient.ReceiveMessageAsync(cancellationToken: cancellationToken);
        if (message.Value == null)
        {
            return null;
        }

        return new Message
        {
            Id = new MessageId
            {
                Id = message.Value.MessageId,
                ReceiptHandle = message.Value.PopReceipt
            },
            Body = message.Value.MessageText
        };
    }

    public async Task ProcessMessages(Action<Message> processMessage, CancellationToken cancellationToken)
    {
        await foreach (var message in ReceiveMessagesAsync(cancellationToken))
        {
            await DeleteMessageAsync(message.Id, cancellationToken);
            try
            {
                processMessage(message);
            }
            catch (Exception e)
            {
                _logger.LogError("ProcessMessages failed", e);
                //return message to the queue
                await SendMessageAsync(message.Body, cancellationToken);
            }

            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    public async Task ProcessMessages(Func<Message, Task> processMessage, CancellationToken cancellationToken)
    {
        await foreach (var message in ReceiveMessagesAsync(cancellationToken))
        {
            await DeleteMessageAsync(message.Id, cancellationToken);
            try
            {
                await processMessage(message);
            }
            catch (Exception e)
            {
                _logger.LogError("ProcessMessages failed", e);
                //return message to the queue
                await SendMessageAsync(message.Body, cancellationToken);
            }

            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    public async Task ProcessMessages(Action<Message> processMessage, int parallel, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>(parallel);
        for (var i = 0; i < parallel; i++)
        {
            tasks.Add(Task.Factory.StartNew(o => ProcessMessages(processMessage, cancellationToken),
                null, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current));
        }

        await Task.WhenAll(tasks);
    }

    public async Task ProcessMessages(Func<Message, Task> processMessage, int parallel, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>(parallel);
        for (var i = 0; i < parallel; i++)
        {
            tasks.Add(Task.Factory.StartNew(o => ProcessMessages(processMessage, cancellationToken),
                null, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current));
        }

        await Task.WhenAll(tasks);
    }

    public async Task<MessageId?> SendMessageAsync(string message, CancellationToken cancellationToken)
    {
        var response = await _queueClient.SendMessageAsync(message, cancellationToken);
        if (response.Value == null)
        {
            return null;
        }

        return new MessageId
        {
            Id = response.Value.MessageId,
            ReceiptHandle = response.Value.PopReceipt
        };
    }

    public async Task DeleteMessageAsync(MessageId id, CancellationToken cancellationToken = default)
    {
        await _queueClient.DeleteMessageAsync(id.Id, id.ReceiptHandle, cancellationToken);
    }

    public async IAsyncEnumerable<Message> ReceiveMessagesAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var message = await ReceiveMessageAsync(cancellationToken);
            if (message != null)
            {
                yield return message;
            }
        }
    }
}
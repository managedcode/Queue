using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using ManagedCode.Queue.AzureServiceBus.Options;
using ManagedCode.Queue.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace ManagedCode.Queue.AzureServiceBus;

public class AzureServiceBusSender : IQueueSender
{
    private readonly ILogger<AzureServiceBusSender> _logger;
    private readonly AzureServiceBusOptions _options;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusAdministrationClient _adminClient;

    private ServiceBusSender _sender;

    public AzureServiceBusSender(
        ILogger<AzureServiceBusSender> logger,
        AzureServiceBusOptions options)
    {
        _logger = logger;
        _options = options;
        _client = new ServiceBusClient(options.ConnectionString);
        _sender = _client.CreateSender(options.Queue);
        _adminClient = new ServiceBusAdministrationClient(options.ConnectionString);
    }

    public async Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        await _sender.SendMessageAsync(new ServiceBusMessage(message), cancellationToken);
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

    public async Task AddTopicAsync(string topic, CancellationToken cancellationToken = default)
    {
        if (!await _adminClient.TopicExistsAsync(topic, cancellationToken))
        {
            await _adminClient.CreateTopicAsync(topic, cancellationToken);
        }

        await _sender.DisposeAsync();

        _sender = _client.CreateSender(topic);
    }
}
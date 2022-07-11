using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using ManagedCode.Queue.AzureServiceBus.Options;
using ManagedCode.Queue.Core.Abstractions;
using ManagedCode.Queue.Core.Models;
using Microsoft.Extensions.Logging;

namespace ManagedCode.Queue.AzureServiceBus;

public class AzureServiceBusSender : IQueueSender, IAsyncDisposable
{


    

  

    public Task SendMessageAsync(string queue, Message message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SendMessageAsync(string queue, string topic, Message message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }
}

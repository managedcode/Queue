using ManagedCode.Queue.Core;
using ManagedCode.Queue.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ManagedCode.Queue.Tests;

public class InMemoryQueueTests
{
    private readonly IQueueSender _queueSender;
    private readonly IQueueReceiver _queueReceiver;
    private readonly IQueueManager _queueManager;
    
    public InMemoryQueueTests()
    {
        var services = new ServiceCollection();

        services.AddScoped<InMemoryQueue>();
        services.AddScoped<IQueueSender, InMemoryQueue>();
        services.AddScoped<IQueueReceiver, InMemoryQueue>();
        services.AddScoped<IQueueManager, InMemoryQueue>();
        
        var provider = services.BuildServiceProvider();

        _queueSender = provider.GetService<IQueueSender>();
        _queueReceiver = provider.GetService<IQueueReceiver>();
        _queueManager = provider.GetService<IQueueManager>();
    }
}
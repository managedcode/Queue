using System.Collections.Concurrent;
using ManagedCode.Queue.Core.Abstractions;
using ManagedCode.Queue.Core.Models;

namespace ManagedCode.Queue.Core;

public class InMemoryQueue: IQueueSender, IQueueReceiver, IQueueManager
{
   private readonly Dictionary<string, Queue<Message>> _queues = new ();
   
    public async Task SendMessageAsync(string queue, Message message, CancellationToken cancellationToken = default)
    {
       await CreateQueueAsync(queue, cancellationToken);
       _queues[queue].Enqueue(message);
    }

    public async Task SendMessageAsync(string queue, string topic, Message message, CancellationToken cancellationToken = default)
    {
       await CreateTopicAsync(queue, topic, cancellationToken);
       _queues[queue+topic].Enqueue(message);
    }

    public async IAsyncEnumerable<Message> ReceiveMessages(string queue, string topic, CancellationToken cancellationToken = default)
    {
       await CreateTopicAsync(queue, topic, cancellationToken);
       
       while (!cancellationToken.IsCancellationRequested)
       {
          if (_queues[queue+topic].TryDequeue(out var message))
          {
             yield return message;
          }
          else
          {
             await Task.Delay(100, cancellationToken);
          }
       }
    }

    public async IAsyncEnumerable<Message> ReceiveMessages(string queue, CancellationToken cancellationToken = default)
    {
       while (!cancellationToken.IsCancellationRequested)
       {
          if (_queues[queue].TryDequeue(out var message))
          {
             yield return message;
          }
          else
          {
             await Task.Delay(100, cancellationToken);
          }
       }
    }
    
    public Task CreateQueueAsync(string queue, CancellationToken cancellationToken = default)
    {
       if (!_queues.ContainsKey(queue))
          _queues[queue] = new Queue<Message>();

       return Task.CompletedTask;
    }

    public Task DeleteQueueAsync(string queue, CancellationToken cancellationToken = default)
    {
       _queues.Remove(queue);
       return Task.CompletedTask;
    }

    public Task CleanQueue(string queue, CancellationToken cancellationToken)
    {
       if (_queues.ContainsKey(queue))
         _queues[queue].Clear();
       
       return Task.CompletedTask;
    }

    public Task CreateTopicAsync(string queue, string topic, CancellationToken cancellationToken = default)
    {
       if (!_queues.ContainsKey(queue+topic))
          _queues[queue+topic] = new Queue<Message>();
       
       return Task.CompletedTask;
    }

    public Task DeleteTopicAsync(string queue, string topic, CancellationToken cancellationToken = default)
    {
       if (_queues.ContainsKey(queue+topic))
         _queues.Remove(queue+topic);
       
       return Task.CompletedTask;
    }
}
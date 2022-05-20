namespace ManagedCode.Queue.AzureServiceBus.Options;

public record AzureServiceBusOptions(string ConnectionString, string Queue)
{
    public string ConnectionString { get; set; } = ConnectionString;
    public string Queue { get; set; } = Queue;
}
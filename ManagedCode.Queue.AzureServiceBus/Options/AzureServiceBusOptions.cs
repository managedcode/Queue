namespace ManagedCode.Queue.AzureServiceBus.Options;

public record AzureServiceBusOptions(
    string ConnectionString,
    string Queue);
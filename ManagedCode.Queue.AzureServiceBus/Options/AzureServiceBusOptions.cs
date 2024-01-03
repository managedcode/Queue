using Azure.Identity;

namespace ManagedCode.Queue.AzureServiceBus.Options;

public class AzureServiceBusOptions
{
    public string ConnectionString { get; set; }
    public string FullyQualifiedNamespace { get; set; }
    public DefaultAzureCredential DefaultAzureCredential { get; set; }
}
    
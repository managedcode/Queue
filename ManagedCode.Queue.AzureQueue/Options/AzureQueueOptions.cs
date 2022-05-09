namespace ManagedCode.Queue.AzureQueue.Options;

public class AzureQueueOptions
{
    public string? ConnectionString { get; set; }
    public string? Queue { get; set; }
    public bool ShouldCreateIfNotExists { get; set; } = true;
}
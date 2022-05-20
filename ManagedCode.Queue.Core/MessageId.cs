namespace ManagedCode.Queue.Core;

public record MessageId(
    string Id,
    string? ReceiptHandle = null);
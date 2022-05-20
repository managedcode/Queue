namespace ManagedCode.Queue.Core.Models;

public record MessageId(
    string Id,
    string? ReceiptHandle = null);
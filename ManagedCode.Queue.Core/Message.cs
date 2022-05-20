namespace ManagedCode.Queue.Core;

public record Message(
    MessageId Id,
    string? Body,
    string? Topic = null);
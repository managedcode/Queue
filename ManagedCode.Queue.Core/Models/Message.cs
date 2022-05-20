namespace ManagedCode.Queue.Core.Models;

public record Message(
    MessageId Id,
    string Body,
    Error? Error = null);
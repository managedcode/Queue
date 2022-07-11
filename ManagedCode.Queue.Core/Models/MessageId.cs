using System.Collections;

namespace ManagedCode.Queue.Core.Models;

public class MessageId
{
    public string Id { get; set; } = string.Empty;
    public string? ReceiptHandle { get; set; }
}
   
    
namespace ManagedCode.Queue.Core;

public class MessageError
{
    public MessageError(Exception exception)
    {
        Exception = exception;
    }

    public Exception Exception { get; set; }
}
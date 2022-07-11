using System.Collections;

namespace ManagedCode.Queue.Core.Models;

public class Message 
{
    public MessageId Id
    {
        get;
        set;
    }
    public string Body
    {
        get;
        set;
    }
    public Error? Error
    {
        get;
        set;
    }


}
    

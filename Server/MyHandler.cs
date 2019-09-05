using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Server.Core.Exceptions;

#region MyHandler
public class MyHandler :
    IHandleMessages<MyMessage>
{
    static ILog log = LogManager.GetLogger<MyHandler>();

    public Task Handle(MyMessage message, IMessageHandlerContext context)
    {
        log.Info($"Message received. Id: {message.Id} - value: {message.Value}");

        try
        {
            if (message.Value < 25)
                throw new Exception("Uh oh - something went wrong, retry!....");
            else if (message.Value > 75)
                throw new ArgumentException("Contract Type cannot be null");
            else
                log.Info($"Messaged processed ok. Id: {message.Id}");

        }
        catch (ArgumentException ex)
        {
            // Dont retry this, it wont work!
            var wrappedEx = new PoisonedMessageException("Contract Type: none", ex);
            log.Error($"POSIONED MESSAGE FAIL - Id: {message.Id} - Exception: {ex.Message}" );
            return Task.FromException(wrappedEx);
        }
        catch (Exception ex)
        {
            log.Error($"TRANSIENT FAIL - RETRY on. Id: {message.Id}");
            return Task.FromException(ex);
        }
        return Task.CompletedTask;
    }
}
#endregion

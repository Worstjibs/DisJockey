using DisJockey.Shared.Messaging.Contracts;
using Wolverine;

namespace DisJockey.Messaging;

public class MessageSender : IMessageSender
{
    private readonly IMessageBus _messageBus;

    public MessageSender(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task SendAsync<T>(T message)
    {
        await _messageBus.SendAsync(message);
    }
}

using System.Threading.Tasks;

namespace DisJockey.Shared.Messaging.Contracts;

public interface IMessageSender
{
    Task SendAsync<T>(T message);
}

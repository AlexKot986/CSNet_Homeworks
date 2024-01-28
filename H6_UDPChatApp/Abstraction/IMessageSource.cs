
using System.Net;

namespace H6_UDPChatApp.Abstraction
{
    public interface IMessageSource
    {
        void SendMessage(MessageUDP message, IPEndPoint endPoint);
        MessageUDP ReceiveMessage(ref IPEndPoint endPoint);
    }
}

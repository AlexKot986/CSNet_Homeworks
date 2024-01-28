
using H6_UDPChatApp.Abstraction;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace H6_UDPChatApp
{
    public class MessageSource : IMessageSource
    {
        private readonly UdpClient udpClient;
        public MessageSource(int port)
        {
            udpClient = new(port);
        }
        public MessageUDP ReceiveMessage(ref IPEndPoint endPoint)
        {
            var buffer = udpClient.Receive(ref endPoint);
            string json = Encoding.ASCII.GetString(buffer);

            return MessageUDP.FromJson(json);
        }
        public void SendMessage(MessageUDP message, IPEndPoint endPoint)
        {
            string json = message.ToJson();
            byte[] buffer = Encoding.ASCII.GetBytes(json);
            udpClient.Send(buffer, endPoint);
        }
    }
}

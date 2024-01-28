
using H6_UDPChatApp;
using H6_UDPChatApp.Abstraction;
using System.Net;

namespace H6_NUnitUDPChatAppTests
{
    public class MockMessageSourceClient : IMessageSource
    {
        private Queue<MessageUDP> receivedMessages = new();
        public Queue<MessageUDP> sentMessages = new();
        private IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);

        public MockMessageSourceClient()
        {
            receivedMessages.Enqueue(new MessageUDP { Command = Command.Message, FromName = "Юля", ToName = "Вася", Text = "От Юли" });
            receivedMessages.Enqueue(new MessageUDP { Command = Command.Message, FromName = "Петя", ToName = "Вася", Text = "От Пети" });
        }

        public MessageUDP ReceiveMessage(ref IPEndPoint ep)
        {
            if (receivedMessages.Count == 0)
            {
                return null;
            }
            var msg = receivedMessages.Dequeue();
            return msg;
        }

        public void SendMessage(MessageUDP message, IPEndPoint ep)
        {
            sentMessages.Enqueue(message);
        }
    }
    public class ClientTest
    {
        [SetUp] 
        public void SetUp() 
        { 
        }
        [Test]
        public void Test()
        {
            var mock = new MockMessageSourceClient();
            var cln = new ClientUDP("Вася", mock, new IPEndPoint(IPAddress.Any, 0));

            cln.ClientListener();

            Assert.IsTrue(mock.sentMessages.Count > 0, "Сообщения не отправляются!");

            for (int i = 0;  i < mock.sentMessages.Count; i++)
            {
                var msg = mock.sentMessages.Dequeue();
                if (i == 0)
                {
                    Assert.IsTrue(msg.Command == Command.Register, "Сообщение о регистрации не отправлено!");
                }
                else
                {
                    Assert.IsTrue(msg.Command == Command.Confirmation, "Подтверждение получения сообщения не отправлено!");
                }
            }
        }
        
    }
}

using H6_UDPChatApp.Abstraction;
using H6_UDPChatApp.Model;
using H6_UDPChatApp;
using System.Net;

namespace H6_NUnitUDPChatAppTests
{
    public class MockMessageSource : IMessageSource
    {
        public Queue<MessageUDP> sentMessages = new();
        private Queue<MessageUDP> receivedMessages = new();

        public MockMessageSource()
        {
            receivedMessages.Enqueue(new MessageUDP { Command = Command.Register, FromName = "Вася" });
            receivedMessages.Enqueue(new MessageUDP { Command = Command.Register, FromName = "Юля" });
            receivedMessages.Enqueue(new MessageUDP { Command = Command.Message, FromName = "Юля", ToName = "Вася", Text = "От Юли" });
            receivedMessages.Enqueue(new MessageUDP { Command = Command.Message, FromName = "Вася", ToName = "Юля", Text = "От Васи" });
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

    public class ServerTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [TearDown]
        public void TeatDown()
        {
            using (var ctx = new H6_UDPChatApp.Model.TestContext())
            {
                ctx.Messages.RemoveRange(ctx.Messages);
                ctx.Users.RemoveRange(ctx.Users);
                ctx.SaveChanges();
            }
        }
        [Test]
        public void Test1()
        {
            var mock = new MockMessageSource();
            var srv = new ServerUDP(mock);
            srv.Work();

            using (var ctx = new H6_UDPChatApp.Model.TestContext())
            {
                Assert.IsTrue(ctx.Users.Count() == 2, "Пользователи не созданы");

                var user1 = ctx.Users.FirstOrDefault(x => x.Name == "Вася");
                var user2 = ctx.Users.FirstOrDefault(x => x.Name == "Юля");

                Assert.IsNotNull(user1, "Пользователь не создан");
                Assert.IsNotNull(user2, "Пользователь не создан");

                Assert.IsTrue(user1.FromMessages.Count == 1);
                Assert.IsTrue(user2.FromMessages.Count == 1);

                Assert.IsTrue(user1.ToMessages.Count == 1);
                Assert.IsTrue(user2.ToMessages.Count == 1);


                var msg1 = ctx.Messages.FirstOrDefault(x => x.FromUser == user1 && x.ToUser == user2);
                var msg2 = ctx.Messages.FirstOrDefault(x => x.FromUser == user2 && x.ToUser == user1);

                Assert.AreEqual("От Юли", msg2.Text);
                Assert.AreEqual("От Васи", msg1.Text);
                          
            }
            Assert.IsTrue(mock.sentMessages.Count == 2, "Сервер не отправил сообщения");
        }
    }
}

using H5_EntityHomeWork.Model;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace H5_EntityHomeWork
{
    public class ClientUDP
    {
        public string Name { get; set; }
        UdpClient client;
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
        public ClientUDP(string name, int port)
        {
            Name = name;
            client = new UdpClient(port);
        }
        private async Task SendMessageUDP(MessageUDP message, Command command)
        {
            message.Command = command;
            var msg = message.ToJson();

            await client.SendAsync(Encoding.ASCII.GetBytes(msg), remoteEP);
        }
        private async Task ReceiveMessageUDP()
        {
            try
            {
                var buffer = await client.ReceiveAsync();
                MessageUDP msg = MessageUDP.FromJson(Encoding.ASCII.GetString(buffer.Buffer));

                await SendMessageUDP(msg, Command.Confirmation);
                Console.WriteLine($"Получено сообщение!\n{msg}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task Work()
        {
            MessageUDP hiMessage = new MessageUDP { FromName = Name, ToName = "Server", Text = "register", Command = Command.Register };
            await client.SendAsync(Encoding.ASCII.GetBytes(hiMessage.ToJson()), remoteEP);

            /** Метод 'GetMessages' проверяет наличие непрочитанных сообщений и возвращает 'bool' и список 'MessageUDP'
             * если 'bool = true' выводит каждое непрочитанное сообщение на консоль
             * и отправляет каждое сообщение на 'Server' с командой 'Command = Confirmation' для подтверждения получения сообщения**/

            if (GetMessages(out List<MessageUDP> msgs))
            {
                Console.Write("Есть непрочитанные сообщения!\nпрочитать 'yes' или 'no': ");
                string change = Console.ReadLine();

                if (change == "yes")
                {
                    msgs.ForEach(async m =>
                    {
                        Console.WriteLine(m);
                        await SendMessageUDP(m, Command.Confirmation);
                    });
                }                                
                else
                {
                    Console.WriteLine("В другой раз");
                }
            }

            Task sender = new Task(async () =>
            {
                while (true)
                {
                    Console.Write("Имя получателя: ");
                    string msgToName = Console.ReadLine();
                    Console.Write("Текст сообщения: ");
                    string msgText = Console.ReadLine();

                    if (!string.IsNullOrEmpty(msgText) && !string.IsNullOrEmpty(msgText))
                    {
                        MessageUDP message = new MessageUDP { FromName = Name, ToName = msgToName, Text = msgText };
                        await SendMessageUDP(message, Command.Message);
                    }
                }
            });

            Task recipient = new Task(async () =>
            {
                while (true)
                {
                    try
                    {
                        await ReceiveMessageUDP();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            });

            sender.Start();
            recipient.Start();
            sender.Wait();
            recipient.Wait();

        }
        /** Метод 'GetMessages' проверяет наличие непрочитанных сообщений и возвращает 'bool' и список 'MessageUDP'**/
        private bool GetMessages(out List<MessageUDP> msgs)
        {
            bool result = false;
            msgs = new List<MessageUDP>();
            using (var ctx = new Context())
            {
                var someuser = ctx.Users.First(u => u.Name == Name);
                var messages = ctx.Messages.Where(m => m.Received == false && m.ToUserId == someuser.Id).ToList();

                if (messages.Count > 0)
                {
                    result = true;

                    foreach (Message message in messages)
                    {
                        msgs.Add(new MessageUDP { Id = message.Id, ToName = message.ToUser.Name, FromName = message.FromUser.Name, Text = message.Text });
                    }
                }
                else
                    result = false;
            }
            return result;
        }       
    }
}

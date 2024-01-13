using System.Net;
using System.Net.Sockets;
using System.Text;

namespace H1_ClientServerApp
{
    internal class Chat
    {
        public static void Server()
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient udpClient = new UdpClient(12345);
            Console.WriteLine("Сервер ожидает сообщение от клюента");

            while (true)
            {
                try
                {
                    byte[] buffer = udpClient.Receive(ref remoteEP);
                    string jsonText = Encoding.UTF8.GetString(buffer);

                    if (jsonText is "Exit")
                    {
                        Console.WriteLine("Сервер завершил работу");
                        break;
                    }

                    new Thread(() =>
                    {
                        Message? message = Message.FromJson(jsonText);
                        if (message != null)
                        {
                            Console.WriteLine(message);
                            Message serverMessage = new Message("Server", "Сообщение получено!");
                            string js = serverMessage.ToJson();
                            byte[] bytes = Encoding.UTF8.GetBytes(js);
                            udpClient.Send(bytes, remoteEP);
                        }                      
                    }).Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public static void Client(string nickName)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
            UdpClient udpClient = new UdpClient(12346);

            udpClient.Send(Encoding.UTF8.GetBytes("hi"), remoteEP);

            while (true)
            {
                Console.Write("Введите сообщение: ");
                string text = Console.ReadLine();
                if (text is "Exit")
                {
                    udpClient.Send(Encoding.UTF8.GetBytes(text), remoteEP);
                    break;
                }
                Message message = new Message(nickName, text);
                string js = message.ToJson();
                byte[] bytes = Encoding.UTF8.GetBytes(js);
                udpClient.Send(bytes, remoteEP);


                byte[] buffer = udpClient.Receive(ref remoteEP);
                string jsonText = Encoding.UTF8.GetString(buffer);

                Message? serverMessage = Message.FromJson(jsonText);

                Console.WriteLine(serverMessage);

            }
        }
    }
}

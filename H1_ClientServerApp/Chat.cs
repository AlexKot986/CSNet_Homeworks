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

                    Message? message = Message.FromJson(jsonText);
                    if (message != null)
                    {
                        Console.WriteLine(message);
                        Message serverMessage = new Message("Server", "Сообщение получено!");
                        string js = serverMessage.ToJson();
                        byte[] bytes = Encoding.UTF8.GetBytes(js);
                        udpClient.Send(bytes, remoteEP);
                    }
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

            while (true)
            {
                Console.Write("Введите сообщение: ");
                string text = Console.ReadLine();
                if (string.IsNullOrEmpty(text))
                {
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

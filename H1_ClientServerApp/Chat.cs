using System.Net;
using System.Net.Sockets;
using System.Text;

namespace H1_ClientServerApp
{
    internal class Chat
    {
        public static CancellationTokenSource cts = new CancellationTokenSource();
        public static CancellationToken ct = cts.Token;
        public static async Task Server()
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            UdpClient udpClient = new UdpClient(12345);
            Console.WriteLine("Сервер ожидает сообщение от клиента");

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    byte[] buffer = udpClient.Receive(ref remoteEP);             
                    string jsonText = Encoding.UTF8.GetString(buffer);

                    if (jsonText is "Exit")
                    {
                        byte[] sayOff = Encoding.UTF8.GetBytes("serverIsOff");
                        await udpClient.SendAsync(sayOff, remoteEP);
                        Console.WriteLine("Сервер завершил работу");
                        cts.Cancel();
                        ct.ThrowIfCancellationRequested();
                    }

                    await Task.Run(async () =>
                    {
                        Message? message = Message.FromJson(jsonText);
                        if (message != null)
                        {
                            Console.WriteLine(message);
                            Message serverMessage = new Message("Server", "Сообщение получено!");
                            string js = serverMessage.ToJson();
                            byte[] bytes = Encoding.UTF8.GetBytes(js);
                            await udpClient.SendAsync(bytes, bytes.Length, remoteEP);
                        }
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }           
            }
        }

        public static async Task Client(string nickName)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
            UdpClient udpClient = new UdpClient(12346);
            
            udpClient.Send(Encoding.UTF8.GetBytes("hi"), remoteEP);

            while (true)
            {
                Console.Write("Введите сообщение: ");
                string text = Console.ReadLine();

                if (text is "Exit")
                    await udpClient.SendAsync(Encoding.UTF8.GetBytes(text), remoteEP, ct);
                   
                Message message = new Message(nickName, text);
                string js = message.ToJson();
                byte[] bytes = Encoding.UTF8.GetBytes(js);
                await udpClient.SendAsync(bytes, bytes.Length, remoteEP);

          
                var buffer = await udpClient.ReceiveAsync();
                string jsonText = Encoding.UTF8.GetString(buffer.Buffer);

                if (jsonText is "serverIsOff") 
                    break;

                Message? serverMessage = Message.FromJson(jsonText);
                Console.WriteLine(serverMessage);
            }
        }
    }
}

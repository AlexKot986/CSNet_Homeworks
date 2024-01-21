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
            Dictionary<string, IPEndPoint> clients = new();
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
                        Message goodbye = new Message("Server", "All", "Сервер завершил работу");
                        byte[] sayOff = Encoding.UTF8.GetBytes(goodbye.ToJson());

                        foreach (IPEndPoint client in clients.Values)
                        {
                            await udpClient.SendAsync(sayOff, client);

                        }
                        Console.WriteLine("Сервер завершил работу");

                        cts.Cancel();
                        ct.ThrowIfCancellationRequested();
                    }

                    await Task.Run(async () =>
                    {
                        Message? message = Message.FromJson(jsonText);
                        if (message is not null)
                        {
                            if (message.ToName.ToLower().Equals("all"))
                            {
                                foreach (IPEndPoint client in clients.Values)
                                {
                                    await udpClient.SendAsync(buffer, buffer.Length, client);
                                }
                                Console.WriteLine($"Клиент '{message.FromName}' отправил сообщение '{message.ToName}'");
                            }
                            else
                            {
                                Message serverMessage = message.Clone() as Message ?? new Message("Server", "Back message", "EROR!");
                                if (message.ToName.Equals("Server"))
                                {
                                    if (message.Text.ToLower().Equals("register"))
                                    {
                                        if (clients.TryAdd(message.FromName, remoteEP))
                                        {
                                            serverMessage.Text = $"Добавлен Клиент '{message.FromName}'";
                                            Console.WriteLine($"Регистрация Клиента '{message.FromName}'");
                                        }
                                        else
                                        {
                                            serverMessage.Text = $"Добавить Клиента '{message.FromName}' неудалось";
                                            Console.WriteLine($"Регистрация Клиента '{message.FromName}' неудалась");
                                        }
                                    }
                                    else if (message.Text.ToLower().Equals("delete"))
                                    {
                                        if (clients.Remove(message.FromName))
                                        {
                                            serverMessage.Text = $"Клиент '{message.FromName}' удален";
                                        }
                                        else
                                        {
                                            serverMessage.Text = $"Удалить Клиента '{message.FromName}' неудалось";
                                            Console.WriteLine($"Удалить Клиента '{message.FromName}' неудалось");
                                        }
                                    }
                                    else if (message.Text.ToLower().Equals("list"))
                                    {
                                        StringBuilder stringBuilder = new StringBuilder();
                                        stringBuilder.Append("Список клиентов:");
                                        foreach (var client in clients.Keys)
                                        {
                                            stringBuilder.Append("\n" + client);
                                        }
                                        serverMessage.Text = stringBuilder.ToString();
                                        Console.WriteLine($"Клиенту '{message.FromName}' отправлен список Клиентов");
                                    }
                                    else
                                        serverMessage.Text = "Текст сообщения должен быть 'reqister' или 'delete'";

                                }

                                else
                                {
                                    if (clients.TryGetValue(message.ToName, out IPEndPoint? value))
                                    {
                                        remoteEP = value;
                                        serverMessage = message;
                                        Console.WriteLine($"Клиент '{message.FromName}' отправил сообщение '{message.ToName}'");
                                    }
                                    else
                                        serverMessage.Text = $"Клиента '{message.ToName}' нет в базе";
                                }

                                string js = serverMessage.ToJson();
                                byte[] bytes = Encoding.UTF8.GetBytes(js);
                                await udpClient.SendAsync(bytes, bytes.Length, remoteEP);
                            }
                        }
                    });

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public static async Task Client(string nickName, string port)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
            UdpClient udpClient = new UdpClient(int.Parse(port));

            //udpClient.Send(Encoding.UTF8.GetBytes("hi"), remoteEP);

            await Task.Delay(100);


            Task sender = new Task(async () =>
            {

                while (!ct.IsCancellationRequested)
                {
                    Task.Delay(100).Wait();

                    Console.Write("Введите имя получателя: ");
                    string toClient = Console.ReadLine() ?? string.Empty;
                    Console.Write("Введите сообщение: ");
                    string text = Console.ReadLine() ?? string.Empty;

                    if (text is "Exit")
                        await udpClient.SendAsync(Encoding.UTF8.GetBytes(text), remoteEP);

                    Message message = new Message(nickName, toClient, text);
                    string js = message.ToJson();
                    byte[] bytes = Encoding.UTF8.GetBytes(js);
                    await udpClient.SendAsync(bytes, bytes.Length, remoteEP);
                }
            }, ct);


            Task recipient = new Task(async () =>
            {

                while (true)
                {
                    try
                    {
                        var buffer = await udpClient.ReceiveAsync();
                        string jsonText = Encoding.UTF8.GetString(buffer.Buffer);

                        Message? serverMessage = Message.FromJson(jsonText);
                        Console.WriteLine(serverMessage);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        break;
                    }
                }
                cts.Cancel();

            });


            sender.Start();
            recipient.Start();
            sender.Wait();
            recipient.Wait();

        }
    }
}


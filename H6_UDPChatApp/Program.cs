

using H6_UDPChatApp;
using System.Net;


internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            MessageSource messageSourceServer = new MessageSource(12345);
            ServerUDP server = new ServerUDP(messageSourceServer);
            server.Work();
        }
        else if (args.Length == 2)
        {
            MessageSource messageSourceClient = new MessageSource(int.Parse(args[1]));
            ClientUDP client = new ClientUDP(args[0], messageSourceClient, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345));

            Task sendler = new Task(() =>
            {
                client.ClientSendler();
            });

            Task listener = new Task(() =>
            {
                client.ClientListener();
            });


           
            sendler.Start();
            listener.Start();
            sendler.Wait();
            listener.Wait();
        }
    }
}
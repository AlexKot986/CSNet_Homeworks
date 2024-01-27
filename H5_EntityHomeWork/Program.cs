using H5_EntityHomeWork;
using System.Net;

Dictionary<string, IPEndPoint> clients = new()
{
    { "Mike", new IPEndPoint(IPAddress.Parse("127.0.0.1"), 55501) },
    { "John", new IPEndPoint(IPAddress.Parse("127.0.0.1"), 55502) },
    { "Masha", new IPEndPoint(IPAddress.Parse("127.0.0.1"), 55503) }
};

if (args.Length == 0)
{
    ServerUDP server = new ServerUDP(clients);
    server.Work();
}
else if (args.Length == 2)
{
    ClientUDP client = new ClientUDP(args[0], int.Parse(args[1]));
    await client.Work();
}
using H1_ClientServerApp;


if (args.Length == 0)
{
    Chat.Server();
}
else
{
    Chat.Client(args[0]);
}
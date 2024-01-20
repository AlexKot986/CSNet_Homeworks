using H1_ClientServerApp;


if (args.Length == 0)
{
    await Chat.Server();
}
else
{
    await Chat.Client(args[0]);
}



using System;

public class Program
{
    public static void Main(String[] args)
    {
        HandleCallServerOrClient(args);
    }

    public static void HandleCallServerOrClient(string[] args)
    {
        try
        {
            var serverOrClient = args[0];

            if (serverOrClient == "client")
                RunClient();
            else if (serverOrClient == "server")
                RunServer();
            else
                throw new();
        }
        catch (Exception)
        {
            RunClient();
        }
    }

    public static void RunServer() => new Server().StartListening();

    public static void RunClient()
    {
        var client = new Client();
        var remoteAdress = Menu.GetConnection();
        client.EstabilishConnection(remoteAdress);
        Menu.CommandsMenu();
        while (true)
        {
            var (isLocal, option, argument) = Menu.GetCommand();
            client.HandleClientCommand(option, argument, isLocal);
            Console.WriteLine();
        }
    }


}
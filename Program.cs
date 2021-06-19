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
            Console.WriteLine("Se o argumento de {client|server} não for informado, por default será aberta uma conexão de client");
            RunClient();
        }
    }

    public static void RunServer() => new Server().StartListening();

    public static void RunClient()
    {
        var client = new Client();
        var (ipAddres, port) = client.GetRemoteAddress();
        client.EstabilishConnection(ipAddres, port);
        client.Menu();
        while (true)
        {
            var (isLocal, option, argument) = client.GetCommand();
            client.HandleCommand(option, argument, isLocal);
            Console.WriteLine();
        }
    }


}
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class Program
{
    public static void Main(String[] args)
    {
        try
        {
            if (args[0] == "client")
                RunClient();
            else if (args[0] == "server")
                new Server().StartListening();
            else
                throw new();
        }
        catch (Exception)
        {
            Console.WriteLine("Se o argumento de {client|server} não for informado, por default será aberta uma conexão de client");
            RunClient();
        }
    }

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
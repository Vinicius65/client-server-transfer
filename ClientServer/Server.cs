using System;
using System.Net;
using System.Net.Sockets;

public class Server
{
    private Socket socketServer;
    private CommandManager commandManager = new CommandManager();

    public void StartListening()
    {
        Linten();
        ConnectionLoop();
    }

    public void Linten()
    {
        var port = 7777;
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

        socketServer = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        socketServer.Bind(localEndPoint);
        socketServer.Listen(10);
        Console.WriteLine($"Esperando conexão no ip {ipAddress} e porta {port}...");
    }

    public void ConnectionLoop()
    {
        Socket socket = socketServer.Accept();
        Console.WriteLine("Conexão estabelecida");
        while (true)
        {
            try
            {
                CommunicationManager.HandleServerToClient(socket, commandManager);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Server
{
    private Socket socketServer;
    private CommandManager commandManager = new CommandManager();

    string usuario = "admin";
    string senha = "123";
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

        //solicitar aqui usuario e senha
        Socket socket = socketServer.Accept();
        var bytesReceived = CommunicationManager.HandleReceivedBytes(socket);
        var usernameSenha = Encoding.ASCII.GetString(bytesReceived);
        string[] acesso = usernameSenha.Split("||");

        if (acesso[0] == usuario && acesso[1] == senha)
        {
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


        else
        {
            Console.WriteLine("Usuario ou senha invalida");
        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Server
{
    private Socket socketServer;
    private SessionAuth sessionAuth = new();
    public void StartListening()
    {
        Linten();
        var socket = AuthLoop();
        ConnectionLoop(socket);
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

    public Socket AuthLoop()
    {
        var isAuth = false;
        do
        {
            Socket socket = socketServer.Accept();
            var bytesReceived = CommunicationManager.ReceivedBytes(socket, 512);
            var token = Encoding.ASCII.GetString(bytesReceived);
            isAuth = sessionAuth.IsAuth(token);
            if (!isAuth)
            {
                socket.Send(Encoding.ASCII.GetBytes("not authorization"));
                Console.WriteLine("not authorization");
                socket.Dispose();
            }
            else
            {
                Console.WriteLine("authorization");
                socket.Send(Encoding.ASCII.GetBytes("authorization"));
                return socket;
            }
        } while (!isAuth);
        return null;
    }


    public void ConnectionLoop(Socket socket)
    {
        while (true)
        {
            try
            {
                HandleServerCommand(socket);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }

    public static void HandleServerCommand(Socket socket)
    {
        var bytesReceived = CommunicationManager.ReceivedBytes(socket, 512);
        var command = Encoding.ASCII.GetString(bytesReceived);
        var option = command.Split(" ")[0];
        var argument = command.Split(" ").Skip(1).FirstOrDefault();

        if (option == "exit")
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            System.Environment.Exit(0);
        }
        else if (option == "up")
        {
            socket.Send(Encoding.ASCII.GetBytes("Aguardando arquivo"));
            var bytesFile = CommunicationManager.ReceivedBytes(socket, 10240);
            string pathFile = Util.GetFilePathToSave(argument);
            File.WriteAllBytes(pathFile, bytesFile);
            socket.Send(Encoding.ASCII.GetBytes("Feito upload do arquivo"));
        }
        else
        {
            try
            {
                var resultBytes = CommandManager.RunCommand(option, argument);
                CommunicationManager.SendBytes(socket, resultBytes, 10240);

            }
            catch (Exception)
            {
                socket.Send(Encoding.ASCII.GetBytes("error"));
            }
        }
    }
}
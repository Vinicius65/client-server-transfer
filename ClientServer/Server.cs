using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Server
{
    private Socket socketServer;
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
        var bytesReceived = CommunicationManager.ReceivedBytes(socket);
        var usernameSenha = Encoding.ASCII.GetString(bytesReceived);
        string[] acesso = usernameSenha.Split("||");

        if (acesso[0] == usuario && acesso[1] == senha)
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
        else
        {
            Console.WriteLine("Usuario ou senha invalida");
        }
    }

    public static void HandleServerCommand(Socket socket)
    {
        var bytesReceived = CommunicationManager.ReceivedBytes(socket);
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
            var bytesFile = CommunicationManager.ReceivedBytes(socket);
            string pathFile = Util.GetFilePathToSave(argument);
            File.WriteAllBytes(pathFile, bytesFile);
            socket.Send(Encoding.ASCII.GetBytes("Feito upload do arquivo"));
        }
        else
        {
            try
            {
                var resultBytes = CommandManager.RunCommand(option, argument);
                socket.Send(resultBytes);

            }
            catch (Exception)
            {
                socket.Send(Encoding.ASCII.GetBytes("error"));
            }
        }
    }
}
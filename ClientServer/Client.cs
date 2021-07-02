using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static Util;


public class Client
{
    private Socket socket;

    public void HandleClientCommand(string option, string argument, bool isLocal)
    {
        if (isLocal)
        {

            var bytes = CommandManager.RunCommand(option, argument);
            Console.WriteLine(Encoding.ASCII.GetString(bytes));
        }
        else if (option == "up")
        {
            try
            {
                var bytesFileLocal = CommandManager.RunCommand(option, argument);
                CommunicationManager.SendMessageAndReceivedMessage(socket, $"{option} {argument}");
                var bytesResponse = CommunicationManager.SendMessageAndReceivedMessage(socket, bytesFileLocal);
                Console.WriteLine(Encoding.ASCII.GetString(bytesResponse));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        else
        {
            if (option == "exit")
                CommunicationManager.ExitClientAndSendExitServer(socket);
            else
            {
                var bytesResponse = CommunicationManager.SendMessageAndReceivedMessage(socket, $"{option} {argument}");
                var stringRespose = Encoding.ASCII.GetString(bytesResponse);
                if (option == "down")
                {
                    if (stringRespose == "error")
                        Console.WriteLine("Erro ao baixar o arquivo (verifique se o nome esta correto)");
                    else
                    {
                        var filePath = GetFilePathToSave(argument);
                        File.WriteAllBytes(filePath, bytesResponse);
                        Console.WriteLine("Arquivo baixado com sucesso");
                    }
                }
                else
                {
                    Console.WriteLine(stringRespose);
                }
            }
        }
    }


    public void EstabilishConnection(ConectionServer cnserver)
    {
        Console.WriteLine("Estabelecendo conexão...");
        var remoteAddress = new IPEndPoint(cnserver.IPAddress, cnserver.Port.Value);
        socket = new Socket(cnserver.IPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            socket.Connect(remoteAddress);
            var retorno = CommunicationManager.SendMessageAndReceivedMessage(socket, cnserver.username + "||" + cnserver.senha);
            Console.WriteLine("Você se conectou ao endereço {0}", socket.RemoteEndPoint.ToString());
        }
        catch (SocketException se)
        {
            Console.WriteLine("SocketException : {0}", se.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine("Unexpected exception : {0}", e.ToString());
        }
    }

}
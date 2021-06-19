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
    private CommandManager CommandManager = new CommandManager();


    public void HandleCommand(string option, string argument, bool isLocal)
    {
        if (isLocal)
        {
            if (option == "exit")
                CommunicationManager.HandleClientToServer(socket, $"{option} {argument}");
            var bytes = CommandManager.RunCommand(option, argument);
            Console.WriteLine(Encoding.ASCII.GetString(bytes));
        }
        else if (option == "up")
        {
            try
            {
                var bytesFileLocal = CommandManager.RunCommand(option, argument);
                CommunicationManager.HandleClientToServer(socket, $"{option} {argument}");
                var bytesResponse = CommunicationManager.HandleClientToServer(socket, bytesFileLocal);
                Console.WriteLine(Encoding.ASCII.GetString(bytesResponse));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        else
        {
            var bytesResponse = CommunicationManager.HandleClientToServer(socket, $"{option} {argument}");
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


    public void EstabilishConnection(IPAddress ipAddress, Port port)
    {
        Console.WriteLine("Estabelecendo conexão...");
        var remoteAddress = new IPEndPoint(ipAddress, port.Value);
        socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            socket.Connect(remoteAddress);
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
        Console.WriteLine("Conexão estabelecida");
    }


    public void Exit()
    {
        socket.Send(Encoding.ASCII.GetBytes("exit"));
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
        System.Environment.Exit(0);
    }

    public (bool, string, string) GetCommand()
    {
        while (true)
        {
            var commandOption = RPrint("Digite o comando: ");
            try
            {
                if (commandOption.Split(" ").FirstOrDefault().Trim() == "exit")
                    Exit();
                return CommandManager.GetCommandTupla(commandOption);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public (IPAddress, Port) GetRemoteAddress()
    {
        var message = "Informe o endreço remoto (ex: 192.168.0.1) ou host name (ex: www.enderecoremoto.com) ou click 'enter' para localhost (endereço local): ";
        var addressOrDns = RPrint(message, Dns.GetHostName());
        try
        {
            var ipAddress = Dns.GetHostEntry(addressOrDns).AddressList[0];

            var port = RPrint("Informe a porta ou enter para porta 7777", "7777");
            return (ipAddress, new Port(port));
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new ArgumentOutOfRangeException("Endereço ip ou host name inválido");
        }
        catch (SocketException)
        {
            throw new Exception("Não foi possível conferir o endereço informado, verifique se a rede está conectada e se o endereço é válido");
        }
        catch (FormatException)
        {
            throw new Exception("Informe uma porta válida (inteiro de 1 a 65535)");
        }
    }

    public void Menu()
    {
        Console.WriteLine($@"
COMANDOS:

local ls                   -- lista o diretório atual
remote ls                  -- listar o diretório remoto
local pwd                  -- ver caminho completo do diretório atual
remote pwd                 -- ver caminho completo do diretório remoto
remote up (argument)        -- fazer upload de um arquivo. Ex: local up file.txt
remote down (argument)     -- fazer download de um arquivo. Ex: remote down foto.png
exit                        
            ");

    }
}
using System;
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
            var bytesResponse = CommandManager.RunCommand(option, argument);
            Console.WriteLine(Encoding.ASCII.GetString(bytesResponse));
        }
        else
        {
            var bytesReceived = CommunicationManager.HandleClientToServer(socket, $"{option} {argument}");
            Console.WriteLine("Echoed test = {0}", Encoding.ASCII.GetString(bytesReceived));
        }
    }

    public void EstabilishConnection(IPAddress ipAddress, Port port)
    {
        Console.WriteLine("Estabelecendo conexão...");
        var remoteAddress = new IPEndPoint(ipAddress, port.Value);
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
        var commandManager = new CommandManager();
        var messageOptions = string.Join("", commandManager.Options().Select(opt => $"\n{opt.Item1,-20} -- {opt.Item2}"));
        var messageHosts = $"{"local",20} {"\nremote",20}\n";
        var messageEx = "local ls\nremote ls\nlocal down arquivo.png\nremote pwd\n";
        var messageTemplate = "{host} {option} {argument}\n";
        Console.WriteLine($@"
    *TEMPLATE*
{messageTemplate}

    *HOSTS*
{messageHosts}

    *OPTION*
{messageOptions}

    *EX:*
{messageEx}

Ou escreva 'exit' para sair do sistema...
            ");

    }
}
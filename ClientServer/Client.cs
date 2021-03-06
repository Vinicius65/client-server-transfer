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


    public void EstabilishConnection(conectionserver cnserver)
    {
        Console.WriteLine("Estabelecendo conex??o...");
        var remoteAddress = new IPEndPoint(cnserver.IPAddress, cnserver.Port.Value);
        socket = new Socket(cnserver.IPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            socket.Connect(remoteAddress);
            var retorno = CommunicationManager.HandleClientToServer(socket, cnserver.username + "||" + cnserver.senha);
            Console.WriteLine("Voc?? se conectou ao endere??o {0}", socket.RemoteEndPoint.ToString());
        }
        catch (SocketException se)
        {
            Console.WriteLine("SocketException : {0}", se.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine("Unexpected exception : {0}", e.ToString());
        }
        Console.WriteLine("Conex??o estabelecida");
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

    public class conectionserver{

        public IPAddress IPAddress { get; set; }
        public Port Port { get; set; }

        public String username { get; set; }

        public String senha { get; set; }
    }
    public conectionserver GetRemoteAddress()
    {
        var conectionserver = new conectionserver();
        var message = "Informe o endere??o remoto (ex: 192.168.0.1) ou host name (ex: www.enderecoremoto.com) ou click 'enter' para localhost (endere??o local): ";
        var addressOrDns = RPrint(message, Dns.GetHostName());
        try
        {
            conectionserver.IPAddress = Dns.GetHostEntry(addressOrDns).AddressList[0];

            var port = RPrint("Informe a porta ou enter para porta 7777", "7777");
            conectionserver.Port = new Port(port);

            conectionserver.username = RPrint("informe o usuario:");

            conectionserver.senha = RPrint("Informe a senha:");

            return conectionserver;
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new ArgumentOutOfRangeException("Endere??o ip ou host name inv??lido");
        }
        catch (SocketException)
        {
            throw new Exception("N??o foi poss??vel conferir o endere??o informado, verifique se a rede est?? conectada e se o endere??o ?? v??lido");
        }
        catch (FormatException)
        {
            throw new Exception("Informe uma porta v??lida (inteiro de 1 a 65535)");
        }
    }

    public void Menu()
    {
        Console.WriteLine($@"
COMANDOS:

local ls                   -- lista o diret??rio atual
remote ls                  -- listar o diret??rio remoto
local pwd                  -- ver caminho completo do diret??rio atual
remote pwd                 -- ver caminho completo do diret??rio remoto
remote up (argument)        -- fazer upload de um arquivo. Ex: local up file.txt
remote down (argument)     -- fazer download de um arquivo. Ex: remote down foto.png
exit                        
            ");

    }
}
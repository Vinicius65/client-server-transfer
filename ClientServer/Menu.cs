using System;
using System.Net;
using System.Net.Sockets;
using static Util;
public static class Menu

{
    public static void Commands()
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

    public static ConectionServer Connection()
    {
        var ConectionServer = new ConectionServer();
        var message = "Informe o endereço remoto (ex: 192.168.0.1) ou host name (ex: www.enderecoremoto.com) ou click 'enter' para localhost (endereço local): ";
        var addressOrDns = RPrint(message, Dns.GetHostName());
        try
        {
            ConectionServer.IPAddress = Dns.GetHostEntry(addressOrDns).AddressList[0];

            var port = RPrint("Informe a porta ou enter para porta 7777", "7777");
            ConectionServer.Port = new Port(port);

            ConectionServer.username = RPrint("informe o usuario:");

            ConectionServer.senha = RPrint("Informe a senha:");

            return ConectionServer;
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
}

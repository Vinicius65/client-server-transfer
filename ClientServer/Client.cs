using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using static Util;

public class Client
{
    public static void StartClient()
    {
        byte[] bytes = new byte[1024];

        var (ipAddress, port) = GetRemoteAddress();
        var remoteAddress = new IPEndPoint(ipAddress, port.Value);

        var socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            socket.Connect(remoteAddress);
            Console.WriteLine("Você se conectou ao endereço {0}", socket.RemoteEndPoint.ToString());

            byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");

            int bytesSent = socket.Send(msg);

            int bytesRec = socket.Receive(bytes);
            Console.WriteLine("Echoed test = {0}",
                Encoding.ASCII.GetString(bytes, 0, bytesRec));

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();

        }
        catch (ArgumentNullException ane)
        {
            Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
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

    public static (IPAddress, Port) GetRemoteAddress()
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
}
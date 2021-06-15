using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using static Util;

public class Client
{
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

    public static void Menu()
    {

    }


    public static void StartClient()
    {
        byte[] bytes = new byte[1024];

        var (ipAddress, port) = GetRemoteAddress();
        var remoteAddress = new IPEndPoint(ipAddress, port.Value);

        var socketSender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            socketSender.Connect(remoteAddress);
            Console.WriteLine("Você se conectou ao endereço {0}", socketSender.RemoteEndPoint.ToString());

            // Encode the data string into a byte array.  
            byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");

            // Send the data through the socket.  
            int bytesSent = socketSender.Send(msg);

            // Receive the response from the remote device.  
            int bytesRec = socketSender.Receive(bytes);
            Console.WriteLine("Echoed test = {0}",
                Encoding.ASCII.GetString(bytes, 0, bytesRec));

            // Release the socket.  
            socketSender.Shutdown(SocketShutdown.Both);
            socketSender.Close();

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
}
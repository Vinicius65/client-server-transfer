using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
public static class CommunicationManager
{
    public static byte[] HandleClientToServer(Socket socket, string stringSend)
    {
        var bytesSend = Encoding.ASCII.GetBytes(stringSend);
        socket.Send(bytesSend);
        return HandleReceivedBytes(socket);
    }

    public static void HandleServerToClient(Socket socket, CommandManager commandManager)
    {
        var bytesReceived = HandleReceivedBytes(socket);
        var command = Encoding.ASCII.GetString(bytesReceived);
        if (command == "exit")
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            System.Environment.Exit(0);
        }
        else
        {
            var option = command.Split(" ").First();
            var argument = command.Split(" ").LastOrDefault();
            var resultBytes = commandManager.RunCommand(option, argument);
            socket.Send(resultBytes);
        }
    }



    public static Socket CreateSocket(IPAddress ipAddress) => new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
    private static byte[] HandleReceivedBytes(Socket socket)
    {
        var lenArray = 512;
        var byteList = new List<byte[]>();
        byte[] receivedByte = new byte[lenArray];

        var countBytes = socket.Receive(receivedByte, receivedByte.Length, 0);
        byteList.Add(receivedByte);

        while (countBytes > 0)
        {
            countBytes = socket.Receive(receivedByte, receivedByte.Length, 0);
            byteList.Add(receivedByte.Take(countBytes).ToArray());
        }
        return byteList.SelectMany(b => b.ToArray()).ToArray();
    }

}
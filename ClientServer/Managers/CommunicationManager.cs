using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
public static class CommunicationManager
{
    public static byte[] SendMessageAndReceivedMessage(Socket socket, string stringSend)
    {
        var bytesSend = Encoding.ASCII.GetBytes(stringSend);
        socket.Send(bytesSend);
        return ReceivedBytes(socket);
    }
    public static byte[] SendMessageAndReceivedMessage(Socket socket, byte[] bytesSend)
    {
        socket.Send(bytesSend);
        return ReceivedBytes(socket);
    }

    public static byte[] ReceivedBytes(Socket socket)
    {
        var lenArray = 10240;
        var byteList = new List<byte[]>();
        byte[] receivedByte = new byte[lenArray];

        int totalBytes = 0;

        int countBytes = 0;
        Console.ForegroundColor = ConsoleColor.DarkGreen;

        do
        {
            countBytes = socket.Receive(receivedByte, receivedByte.Length, 0);
            totalBytes += countBytes;
            byteList.Add(receivedByte.Take(countBytes).ToArray());
            PrintTotal(totalBytes);
        } while (countBytes == lenArray);

        Console.ResetColor();
        Console.WriteLine();
        return byteList.SelectMany(b => b.ToArray()).ToArray();
    }



    public static void ExitClientAndSendExitServer(Socket socket)
    {
        socket.Send(Encoding.ASCII.GetBytes("exit"));
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
        System.Environment.Exit(0);
    }

    private static void PrintTotal(int total)
    {
        Console.Write("                                                             ");
        var (left, top) = Console.GetCursorPosition();
        Console.SetCursorPosition(0, top);
        Console.Write($"Download ({total}) bytes...");
    }

}
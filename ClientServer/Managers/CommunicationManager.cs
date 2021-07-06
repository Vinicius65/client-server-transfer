using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
public static class CommunicationManager
{
    public static byte[] SendMessageAndReceivedMessage(Socket socket, string stringSend, int bytesLen)
    {
        var bytesSend = Encoding.ASCII.GetBytes(stringSend);
        SendBytes(socket, bytesSend, bytesLen);
        return ReceivedBytes(socket, bytesLen);
    }
    public static byte[] SendMessageAndReceivedMessage(Socket socket, byte[] bytesSend, int bytesLen)
    {
        SendBytes(socket, bytesSend, bytesLen);
        return ReceivedBytes(socket, bytesLen);
    }

    public static void SendBytes(Socket socket, byte[] byteArray, int sendLenBytes)
    {
        var skip = 0;
        var partialByte = byteArray.Skip(skip).Take(sendLenBytes);
        Console.WriteLine("Iniciando envio");
        do
        {
            socket.Send(partialByte.ToArray());
            skip += sendLenBytes;
            partialByte = byteArray.Skip(skip).Take(sendLenBytes);
        } while (partialByte.Count() != 0);
        Console.WriteLine("Envio finalizado");
    }


    public static byte[] ReceivedBytes(Socket socket, int len)
    {
        var lenArray = len;
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
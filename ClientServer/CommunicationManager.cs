using System;
using System.Collections.Generic;
using System.IO;
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
    public static byte[] HandleClientToServer(Socket socket, byte[] bytesSend)
    {
        socket.Send(bytesSend);
        return HandleReceivedBytes(socket);
    }

    public static void HandleServerToClient(Socket socket, CommandManager commandManager)
    {
        var bytesReceived = HandleReceivedBytes(socket);
        var command = Encoding.ASCII.GetString(bytesReceived);
        var option = command.Split(" ")[0];
        var argument = command.Split(" ").Skip(1).FirstOrDefault();

        if (option == "exit")
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            System.Environment.Exit(0);
        }
        else if (option == "up")
        {
            socket.Send(Encoding.ASCII.GetBytes("Aguardando arquivo"));
            var bytesFile = HandleReceivedBytes(socket);
            string pathFile = Util.GetFilePathToSave(argument);
            File.WriteAllBytes(pathFile, bytesFile);
            socket.Send(Encoding.ASCII.GetBytes("Feito upload do arquivo"));
        }
        else
        {
            try
            {
                var resultBytes = commandManager.RunCommand(option, argument);
                socket.Send(resultBytes);

            }
            catch (Exception)
            {
                socket.Send(Encoding.ASCII.GetBytes("error"));
            }
        }
    }


    private static byte[] HandleReceivedBytes(Socket socket)
    {
        var lenArray = 512;
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

    private static void PrintTotal(int total)
    {
        Console.Write("                                                             ");
        var (left, top) = Console.GetCursorPosition();
        Console.SetCursorPosition(0, top);
        Console.Write($"Download ({total}) bytes...");
    }

}
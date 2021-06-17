using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class Program
{
    public static void Main(String[] args)
    {
        var commandManager = new CommandManager();
        var remoteCommand = commandManager.Menu();
    }
}
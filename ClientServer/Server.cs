using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Server
{
    private CommandManager commandManager = new CommandManager();

    public void StartListening()
    {
        byte[] bytes = new Byte[1024];

        var port = 7777;
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

        Socket listener = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(10);

            while (true)
            {
                Console.WriteLine($"Waiting for a connection on ip {ipAddress} and port {port}...");
                Socket handler = listener.Accept();

                int bytesRec = handler.Receive(bytes);
                var command = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                if (command == "exit")
                {
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    System.Environment.Exit(0);
                }
                else
                {
                    var option = command.Split(" ").First();
                    var argument = command.Split(" ").LastOrDefault();
                    var resultBytes = commandManager.RunCommand(option, argument);
                    handler.Send(resultBytes);
                }
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}
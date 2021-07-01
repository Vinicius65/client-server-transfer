using System;

public class Port
{
    public int Value { get; }
    public Port(string port) : this(int.Parse(port))
    {
    }
    public Port(int port)
    {
        if (!isValidPort(port)) throw new FormatException();
        Value = port;
    }

    private bool isValidPort(int port) => port > 0 && port < 65535;
}
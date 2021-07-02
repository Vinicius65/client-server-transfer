using System;
using System.Security.Cryptography;
using System.Text;

public class SessionAuth
{
    public string Username { get; } = "master";
    public readonly string _password = "sa";
    public string Token { get; }
    public SessionAuth() => Token = GerateTokenString(Username, _password);

    public static string GerateTokenString(string username, string password)
    {
        using var mySHA256 = SHA256.Create();
        var bytesToken = mySHA256.ComputeHash(Encoding.ASCII.GetBytes($"{username}{password}"));
        return Encoding.ASCII.GetString(bytesToken);
    }
    public static byte[] GerateTokenByte(string username, string password)
    {
        using var mySHA256 = SHA256.Create();
        return mySHA256.ComputeHash(Encoding.ASCII.GetBytes($"{username}{password}"));
    }
    public bool IsAuth(string token) => token == Token;
}
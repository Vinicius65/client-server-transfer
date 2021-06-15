using System;
using System.Security.Cryptography;
using System.Text;

public class Session
{
    public string Username { get; }
    public readonly string _password;
    public byte[] Token { get; }

    public Session(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException("Informe um usuário e senha válidos");
        Username = username;
        _password = password;

        using var mySHA256 = SHA256.Create();
        Token = mySHA256.ComputeHash(Encoding.ASCII.GetBytes($"{Username}{_password}")); ;
    }
}
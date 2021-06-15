using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

public class Commands
{
    public const string LS = "ls";
    public const string PWD = "pwd";
    public const string DOWN = "down";
    public const string UP = "up";
    public const string EXIT = "exit";

    private Dictionary<string, Func<string>> _commandMap = new();

    public Commands() => Init();

    private void Init()
    {
        // LS
        _commandMap.Add(LS, () => string.Join("\n", Directory.GetFiles(Directory.GetCurrentDirectory())));

        // PWD
        _commandMap.Add(PWD, () => Directory.GetCurrentDirectory());

        // DOWN
        _commandMap.Add(DOWN, () => "Aguarde o download...");

        // UP
        _commandMap.Add(UP, () => "Aguarde o upload...");

        // EXIT
        _commandMap.Add(EXIT, () =>
        {
            Console.WriteLine("Saindo...");
            Thread.Sleep(3);
            System.Environment.Exit(0);
            return "";
        });
    }

    public void RunCommand(string command)
    {
        if (_commandMap.TryGetValue(command, out var func))
        {
            func.Invoke();
        }
        else
        {
            Console.WriteLine("Commando inválido...");
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using static Util;



public class CommandManager
{
    public static readonly string RegexValue = "ls|pwd|down|up|exit";
    public static readonly string RegexHost = "local|remote";
    public static readonly Regex MountCommandRegex = new Regex($@"(^|\s+)({RegexValue})\s+({RegexHost}) (?:\S+?)($| )");


    private Dictionary<string, Command> _commandMap = new();

    public CommandManager() => Init();

    private void Init()
    {
        // LS
        _commandMap.Add("ls", new Command
        {
            Value = "ls",
            Describe = "Liste o diretório atual",
            Execute = (arg) => Encoding.ASCII.GetBytes(string.Join("\n", Directory.GetFiles(Directory.GetCurrentDirectory())))
        });

        // PWD
        _commandMap.Add("pwd", new Command
        {
            Value = "pwd",
            Describe = "Mostrar o diretório completo",
            Execute = (arg) => Encoding.ASCII.GetBytes(Directory.GetCurrentDirectory())
        });

        // DOWN
        _commandMap.Add("down", new Command
        {
            Value = "down {file}",
            Describe = "Fazer download do arquivo informando o nome entre chaves. Ex: down meuarquivo.png",
            Execute = (arg) => File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), arg))
        });

        // UP
        _commandMap.Add("up", new Command
        {
            Value = "up {file}",
            Describe = "Fazer upĺoad do arquivo informando o nome entre chaves. Ex: up meuarquivo.png",
            Execute = (arg) => File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), arg))
        });

        // EXIT
        _commandMap.Add("exit", new Command
        {
            Value = "exit",
            Describe = "Fechar conexão",
            Execute = (arg) => null
        });
    }

    public List<(string, string)> Options() => _commandMap.Select(c => (c.Value.Value, c.Value.Describe)).ToList();

    public byte[] RunCommand(string key, string arg)
    {
        if (_commandMap.TryGetValue(key, out var command))
        {
            return command.Execute(arg);
        }
        throw new ArgumentException("Commando informado incorreto...");
    }

    public (string, string, string) GetCommandTupla(string unformatedCommand)
    {
        var commandList = MountCommandRegex.Match(unformatedCommand).Groups.Values.Select(v => v.Value).Skip(1).Take(3).ToArray();
        return (commandList[0], commandList[1], commandList[3]);
    }

    public Byte[] Menu()
    {
        var commandManager = new CommandManager();
        var messageOptions = string.Join("", commandManager.Options().Select(opt => $"\n{opt.Item1,-20} -- {opt.Item2}"));
        var messageHosts = $"{"\nlocal",20} {"\nremote",20}";
        var messageEx = "\nlocal ls\nremote ls\nlocal down arquivo.png\nremote pwd";
        while (true)
        {
            var commandOption = RPrint($@"
                -*- INFORME A OPÇÃO DESEJADA -*-
                
                *HOSTS*
                    {messageHosts}

                *OPTION*
                    {messageOptions}

                EX:
                    {messageEx}
                -*-                          -*-
            ");

            var (host, command, argument) = commandManager.GetCommandTupla(commandOption);
            if (host == "local")
            {
                try
                {
                    return commandManager.RunCommand(command, argument);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                return Encoding.ASCII.GetBytes($"{host} {command} {argument}");
            }
        }
    }
}

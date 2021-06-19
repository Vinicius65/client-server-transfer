using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public class CommandManager
{
    public static readonly Regex MountCommandRegex = new(@"^(local|remote)\s+(ls|pwd|down|up|exit) ?(\S+)?$");
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
            Execute = (arg) =>
            {
                var bytesfile = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), arg));
                var commandWithFile = new List<byte[]> { Encoding.ASCII.GetBytes($"up {arg} "), bytesfile };
                return commandWithFile.SelectMany(b => b).ToArray();
            }
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

    public (bool, string, string) GetCommandTupla(string unformatedCommand)
    {
        var commandList = MountCommandRegex.Match(unformatedCommand).Groups.Values.Select(v => v.Value).Skip(1).Take(3).ToArray();
        var argumentErrorMessage = "Informe o comando corretamente: ex: {host} {option} {argument}";

        var isIncorretNumberArgument = commandList.Count() != 3;
        if (isIncorretNumberArgument)
            throw new ArgumentException(argumentErrorMessage);


        var host = commandList[0];
        var option = commandList[1];
        var argument = commandList[2];

        var isInvalidCommand = string.IsNullOrWhiteSpace(host);
        var lackOfArgument = new string[] { "down", "up" }.Contains(option) && string.IsNullOrWhiteSpace(argument);

        if (isInvalidCommand || lackOfArgument)
            throw new ArgumentException(argumentErrorMessage);

        if (host == "local" && option == "down")
            throw new ArgumentException("Quando selecionado down, o host deve ser 'remote'. Ex: remote down file.txt");
        else if (host == "remote" && option == "up")
            throw new ArgumentException("Quando selecionado up, o host deve ser 'local'. Ex: local up file.txt");

        var isLocal = commandList[0] == "local";
        return (isLocal, commandList[1], commandList[2]);
    }


}

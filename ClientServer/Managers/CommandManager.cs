using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public static class CommandManager
{
    public static readonly Regex MountCommandRegex = new(@"^(local|remote)\s+(ls|pwd|down|up|exit) ?(\S+)?$");
    private static Dictionary<string, Command> _commandMap = CreateDict();


    private static Dictionary<string, Command> CreateDict()
    {
        var commandMap = new Dictionary<string, Command>();
        // LS
        commandMap.Add("ls", new Command
        {
            Value = "ls",
            Describe = "Liste o diretório atual",
            Execute = (arg) => Encoding.ASCII.GetBytes(string.Join("\n", Directory.GetFiles(Directory.GetCurrentDirectory())))
        });

        // PWD
        commandMap.Add("pwd", new Command
        {
            Value = "pwd",
            Describe = "Mostrar o diretório completo",
            Execute = (arg) => Encoding.ASCII.GetBytes(Directory.GetCurrentDirectory())
        });

        // DOWN
        commandMap.Add("down", new Command
        {
            Value = "down {file}",
            Describe = "Fazer download do arquivo informando o nome entre chaves. Ex: down meuarquivo.png",
            Execute = (arg) => File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), arg))
        });

        // UP
        commandMap.Add("up", new Command
        {
            Value = "up {file}",
            Describe = "Fazer upĺoad do arquivo informando o nome entre chaves. Ex: up meuarquivo.png",
            Execute = (arg) => File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), arg))
        });
        return commandMap;
    }

    public static List<(string, string)> Options() => _commandMap.Select(c => (c.Value.Value, c.Value.Describe)).ToList();

    public static byte[] RunCommand(string key, string arg)
    {
        if (_commandMap.TryGetValue(key, out var command))
        {
            return command.Execute(arg);
        }
        throw new ArgumentException("Commando informado incorreto...");
    }

    public static (bool, string, string) GetCommandTupla(string unformatedCommand)
    {
        if (unformatedCommand.Trim().ToLower() == "exit")
            return (false, "exit", "");

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

        if (host == "local" && (option == "down" || option == "up"))
            throw new ArgumentException("Quando selecionado 'down' ou 'up', o host deve ser 'remote'. Ex: remote down file.txt");

        var isLocal = host == "local";
        return (isLocal, option, argument);
    }


}

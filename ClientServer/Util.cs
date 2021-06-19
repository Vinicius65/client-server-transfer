using System;
using System.IO;

public static class Util
{
    public static string RPrint(string print, string defaultValue = null)
    {
        Console.WriteLine(print);
        var read = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(read) && defaultValue != null)
            return defaultValue;
        else
            return read;
    }

    public static string GetFilePathToSave(string argument) => Path.Combine(Directory.GetCurrentDirectory(), $"{DateTime.Now.ToFileTime().ToString()}{argument}");
}
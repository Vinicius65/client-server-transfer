using System;

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
}
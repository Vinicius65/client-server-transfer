using System;

public class Command
{
    public bool isValid { get; set; }
    public string Host { get; set; }
    public string Value { get; set; }
    public string Argument { get; set; }
    public string Describe { get; set; }
    public Func<string, byte[]> Execute { get; set; }
}
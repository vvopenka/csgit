using CommandLine;

namespace experiment;

public enum Commands
{
    ListRefs,
    ListObjects,
    PrintObject
}

public class Arguments
{
    [Option('g', "git-folder", Required = true, HelpText = "Git folder")]
    public string GitFolder { get; init; }
    
    [Option('c', "command", Required = true, HelpText = "Command to execute")]
    public Commands Command { get; init; }
    
    [Option('o', "object", Required = false, HelpText = "Object to print")]
    public string Object { get; init; }
}
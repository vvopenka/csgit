using System.IO.Compression;
using System.Text;
using CommandLine;
using csgit.Lib.Model;
using csgit.Lib.Parser.File;
using experiment;
using StreamReader = csgit.Lib.Parser.StreamReader;

namespace csgit.Experiment;

public static class Program
{
    public static async Task Main(string[] args)
    {
        await Parser.Default.ParseArguments<Arguments>(args)
            .WithParsedAsync(Run);
    }

    static async Task Run(Arguments arguments)
    {
        switch (arguments.Command)
        {
            case Commands.ListRefs:
                await ListRefs(arguments.GitFolder);
                break;
            case Commands.ListObjects:
                await ListObjects(arguments.GitFolder);
                break;
            case Commands.PrintObject:
                await PrintObject(arguments.GitFolder, arguments.Object);
                break;
            default:
                throw new NotImplementedException();
        }
    }
    
    static async Task ListRefs(string gitFolder)
    {
        foreach (Reference reference in await LoadRefs(gitFolder))
        {
            Console.WriteLine(reference);
        }
    }
    
    private static async Task<IList<Reference>> LoadRefs(string gitFolder)
    {
        string refsFolder = Path.Combine(gitFolder, "refs");
        List<Reference> refs = [];
        foreach (string file in Directory.EnumerateFiles(refsFolder, "*", SearchOption.AllDirectories))
        {
            try
            {
                string name = Path.GetRelativePath(refsFolder, file).Replace(Path.DirectorySeparatorChar, '/');
                string hash = await File.ReadAllTextAsync(file);
                if (hash.StartsWith("ref: "))
                {
                    continue;
                }
                refs.Add(new Reference
                {
                    Name = name, Hash = Sha1.Parse(hash.Trim())
                });
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load ref: " + file);
                throw;
            }
        }
        return refs;
    }
    
    static async Task ListObjects(string gitFolder)
    {
        foreach (Sha1 hash in await LoadObjects(gitFolder))
        {
            Console.WriteLine(hash);
        }
    }
    
    private static async Task<List<Sha1>> LoadObjects(string gitFolder)
    {
        string objectsFolder = Path.Combine(gitFolder, "objects");
        List<Sha1> objects = [];
        foreach (string file in Directory.EnumerateFiles(objectsFolder, "*", SearchOption.AllDirectories))
        {
            try
            {
                string name = Path.GetRelativePath(objectsFolder, file);
                if (name.StartsWith("info") || name.StartsWith("pack"))
                {
                    continue;
                }
                objects.Add(Sha1.Parse(name.Replace(Path.DirectorySeparatorChar.ToString(), "")));
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load object: " + file);
                throw;
            }
        }
        return objects;
    }
    
    static async Task PrintObject(string gitFolder, string objectName)
    {
        string file = Path.Combine(gitFolder, "objects", objectName[..2], objectName[2..]);
        await using FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read);
        await using ZLibStream deflate = new ZLibStream(stream, CompressionMode.Decompress);
        await using StreamReader reader = new StreamReader(deflate);
        Sha1 hash = Sha1.Parse(objectName);
        GitObjectParser parser = new GitObjectParser();
        
        GitObject obj = await parser.ParseGitObject(reader, hash);
        
        Console.WriteLine(obj);
        
        // using MemoryStream memory = new MemoryStream();
        // await deflate.CopyToAsync(memory);
        // Console.WriteLine(Encoding.UTF8.GetString(memory.ToArray()));
        // Console.WriteLine(string.Join(" ", memory.ToArray().Select(b => b.ToString("X2"))));
    }

    private static async Task Loadobject(string gitFolder, string objectName)
    {
        string file = Path.Combine(gitFolder, "objects", objectName[..2], objectName[2..]);
        await using FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read);
        await using ZLibStream deflate = new ZLibStream(stream, CompressionMode.Decompress);
    }
}
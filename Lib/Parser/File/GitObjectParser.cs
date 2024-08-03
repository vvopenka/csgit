using System.Text;
using csgit.Lib.Model;

namespace csgit.Lib.Parser.File;

public class GitObjectParser
{
    public async Task<GitObject> ParseGitObject(StreamReader reader, Sha1 hash)
    {
        (GitObjectType type, int size) = await ParseHeader(reader);
        return type switch
        {
            GitObjectType.Blob => await ParseBlobObject(reader, hash),
            GitObjectType.Tree => await ParseTreeObject(reader, hash),
            GitObjectType.Commit => await ParseCommitObject(reader, hash),
            GitObjectType.Tag => await ParseTagObject(reader, hash),
            _ => throw new ParserException("Unsupported object type")
        };
    }

    private async Task<TreeObject> ParseTreeObject(StreamReader reader, Sha1 objectHash)
    {
        List<TreeEntry> entries = [];
        string? line = await reader.ReadStringUntilNul();
        while (line != null)
        {
            string[] parts = line.Split(' ');
            Sha1 hash = await reader.ReadSha1();
            entries.Add(new TreeEntry(parts[0], parts[1], hash));
            line = await reader.ReadStringUntilNul();
        }

        return new TreeObject(objectHash, entries);
    }
    
    private async Task<BlobObject> ParseBlobObject(StreamReader reader, Sha1 objectHash)
    {
        byte[] data = await reader.ReadToEnd();
        return new BlobObject(objectHash, data);
    }
    
    private static async Task<string[]> ReadCommitParts(StreamReader reader)
    {
        string? line = await reader.ReadStringUntilNl();
        if (line == null)
            throw new ParserException("Invalid commit object");
        return line.Split(' ');
    }
    
    private async Task<CommitObject> ParseCommitObject(StreamReader reader, Sha1 objectHash)
    {
        string[] parts = await ReadCommitParts(reader);
        
        if (parts[0] != "tree")
            throw new ParserException("Invalid commit object");
        
        Sha1 tree = Sha1.Parse(parts[1]);

        parts = await ReadCommitParts(reader);
        List<Sha1> parents = [];
        while (parts[0] == "parent")
        {
            parents.Add(Sha1.Parse(parts[1]));
            parts = await ReadCommitParts(reader);
        }
        if (parts[0] != "author")
            throw new ParserException("Invalid commit object");
        string author = parts[1];
        
        parts = await ReadCommitParts(reader);
        if (parts[0] != "committer")
            throw new ParserException("Invalid commit object");
        string committer = parts[1];
        
        string? line = await reader.ReadStringUntilNl();
        if (line == null || line != "")
            throw new ParserException("Invalid commit object");
        
        string message = Encoding.UTF8.GetString(await reader.ReadToEnd());
        
        return new CommitObject(objectHash, tree, parents, author, committer, message);
    }
    
    private async Task<TagObject> ParseTagObject(StreamReader reader, Sha1 objectHash)
    {
        string[] parts = await ReadCommitParts(reader);
        
        if (parts[0] != "object")
            throw new ParserException("Invalid tag object");
        
        Sha1 target = Sha1.Parse(parts[1]);
        
        parts = await ReadCommitParts(reader);
        if (parts[0] != "type")
            throw new ParserException("Invalid tag object");
        
        GitObjectType type = parts[1] switch
        {
            "blob" => GitObjectType.Blob,
            "tree" => GitObjectType.Tree,
            "commit" => GitObjectType.Commit,
            "tag" => GitObjectType.Tag,
            _ => throw new ParserException("Unknown object type")
        };
        
        parts = await ReadCommitParts(reader);
        if (parts[0] != "tag")
            throw new ParserException("Invalid tag object");
        
        string tag = parts[1];
        
        parts = await ReadCommitParts(reader);
        if (parts[0] != "tagger")
            throw new ParserException("Invalid tag object");
        
        string tagger = parts[1];
        
        string? line = await reader.ReadStringUntilNl();
        if (line == null || line != "")
            throw new ParserException("Invalid tag object");
        
        string message = Encoding.UTF8.GetString(await reader.ReadToEnd());
        
        return new TagObject(objectHash, type, target, tag, tagger, message);
    }
    
    private async Task<(GitObjectType type, int size)> ParseHeader(StreamReader reader)
    {
        string? header = await reader.ReadStringUntilNul();
        if (header == null)
        {
            throw new ParserException("Invalid header");
        }
        string[] parts = header.Split(' ');
        GitObjectType type = parts[0] switch
        {
            "blob" => GitObjectType.Blob,
            "tree" => GitObjectType.Tree,
            "commit" => GitObjectType.Commit,
            "tag" => GitObjectType.Tag,
            _ => throw new ParserException("Unknown object type")
        };
        int size = int.Parse(parts[1]);
        return (type, size);
    }
}
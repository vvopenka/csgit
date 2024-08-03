namespace csgit.Lib.Model;

public class CommitObject : GitObject
{
    public Sha1 Tree { get; }
    public IReadOnlyList<Sha1> Parents { get; }
    public string Author { get; }
    public string Committer { get; }
    public string Message { get; }
    
    public CommitObject(Sha1 hash, Sha1 tree, IReadOnlyList<Sha1> parents, string author, string committer, string message) 
        : base(hash, GitObjectType.Commit)
    {
        Tree = tree;
        Parents = parents;
        Author = author;
        Committer = committer;
        Message = message;
    }

    public override string ToString()
    {
        return $"Commit {Hash}\n" +
               $"Tree: {Tree}\n" +
               $"Parents: {string.Join(' ', Parents)}\n" +
               $"Author: {Author}\n" +
               $"Committer: {Committer}\n" +
               $"Message: {Message}";
    }
}
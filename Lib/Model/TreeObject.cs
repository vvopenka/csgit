namespace csgit.Lib.Model;

public class TreeObject : GitObject
{
    public IReadOnlyList<TreeEntry> Entries { get; }

    public TreeObject(Sha1 hash, IReadOnlyList<TreeEntry> entries) : base(hash, GitObjectType.Tree)
    {
        Entries = entries;
    }

    public override string ToString()
    {
        return $"Tree {Hash}\n" + string.Join('\n', Entries);
    }
}
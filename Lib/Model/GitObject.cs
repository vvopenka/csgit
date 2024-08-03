namespace csgit.Lib.Model;

public abstract class GitObject
{
    public Sha1 Hash { get; }
    public GitObjectType Type { get; }

    protected GitObject(Sha1 hash, GitObjectType type)
    {
        Hash = hash;
        Type = type;
    }
}
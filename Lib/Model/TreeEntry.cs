namespace csgit.Lib.Model;

public class TreeEntry
{
    public string Mode { get; }
    public string Name { get; }
    public Sha1 Hash { get; }
    
    public bool IsFolder => Mode == "40000";
    
    public TreeEntry(string mode, string name, Sha1 hash)
    {
        Mode = mode;
        Name = name;
        Hash = hash;
    }
    
    public override string ToString()
    {
        return $"{Mode} {Name} {Hash}";
    }
}
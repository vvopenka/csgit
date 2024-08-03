using csgit.Lib.Model;

namespace experiment;

public class Reference
{
    public string Name { get; init; }
    public Sha1 Hash { get; init; }
    
    public override string ToString()
    {
        return $"{Name} {Hash}";
    }
}
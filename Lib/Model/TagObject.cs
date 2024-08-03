namespace csgit.Lib.Model;

public class TagObject : GitObject
{
    public GitObjectType TagType { get; }
    public Sha1 Object { get; }
    public string Tag { get; }
    public string Tagger { get; }
    public string Message { get; }
    
    public TagObject(Sha1 hash, GitObjectType tagType, Sha1 obj, string tag, string tagger, string message) 
        : base(hash, GitObjectType.Tag)
    {
        TagType = tagType;
        Object = obj;
        Tag = tag;
        Tagger = tagger;
        Message = message;
    }
    
    public override string ToString()
    {
        return $"Tag {Hash}\n" +
               $"Type: {TagType}\n" +
               $"Object: {Object}\n" +
               $"Tag: {Tag}\n" +
               $"Tagger: {Tagger}\n" +
               $"Message: {Message}";
    }
}
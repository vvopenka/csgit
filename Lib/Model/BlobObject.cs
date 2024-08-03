using System.Text;

namespace csgit.Lib.Model;

public class BlobObject : GitObject
{
    private byte[] data;
    
    public BlobObject(Sha1 hash, byte[] data) : base(hash, GitObjectType.Blob)
    {
        this.data = data;
    }
    
    public override string ToString()
    {
        return $"Blob {Hash}\n" + Encoding.UTF8.GetString(data);
    }
}